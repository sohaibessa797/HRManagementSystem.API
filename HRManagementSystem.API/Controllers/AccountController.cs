using AutoMapper;
using HRManagementSystem.API.Identity;
using HRManagementSystem.Application.Dtos.Account;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AccountController(UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists.");
            }
            var user = _mapper.Map<ApplicationUser>(dto);
            user.UserName = dto.Email; // Set UserName to Email

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
               
                var employee = new Employee
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    ApplicationUserId = user.Id,
                    IsActive = true
                };
                _unitOfWork.Employees.Add(employee);
                _unitOfWork.Complete();
                return Ok(new { message = "User registered successfully" });
            }
            else
            {
                return BadRequest(new { message = "User registration failed", errors = result.Errors });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }


            var result = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (result)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var claimList = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach (var role in userRoles)
                {
                    claimList.Add(new Claim(ClaimTypes.Role, role));
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                    claims: claimList,
                    signingCredentials: creds
                );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            else
            {
                return Unauthorized("Invalid email or password.");
            }
        }
    }
}

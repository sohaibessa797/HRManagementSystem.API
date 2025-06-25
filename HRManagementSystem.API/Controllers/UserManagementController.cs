using HRManagementSystem.API.Identity;
using HRManagementSystem.Application.Dtos.UserManagement;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //[HttpGet("all")]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var users = _userManager.Users
        //        .Where(u => u.Id.ToString() != currentUserId)
        //        .ToList();
        //    if (users == null)
        //    {
        //        return NotFound();
        //    }
        //    var userList = users.Select(u => new
        //    {
        //        u.Id,
        //        u.FullName,
        //        u.Email,
        //        Roles = _userManager.GetRolesAsync(u).Result
        //    });
        //    return Ok(userList);
        //}


        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetUserById(Guid id)
        //{
        //    var user = await _userManager.FindByIdAsync(id.ToString());

        //    if (user == null)
        //        return NotFound("User not found.");

        //    var roles = await _userManager.GetRolesAsync(user);

        //    return Ok(new
        //    {
        //        user.Id,
        //        user.FullName,
        //        user.Email,
        //        Roles = roles
        //    });
        //}
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserManagementForRoleResponse dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");

            if (!IdentitySeeder.Roles.Contains(dto.Role))
                return BadRequest(new { message = "Invalid role specified." });

            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
                return BadRequest("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Role assigned successfully.");
        }


        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] UserManagementForNewRoleResponse dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");
            if (!IdentitySeeder.Roles.Contains(dto.NewRole))
                return BadRequest(new { message = "Invalid role specified." });

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all existing roles
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var roleExists = await _roleManager.RoleExistsAsync(dto.NewRole);
            if (!roleExists)
                return BadRequest("New role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, dto.NewRole);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Role changed successfully.");
        }


    }                   
}

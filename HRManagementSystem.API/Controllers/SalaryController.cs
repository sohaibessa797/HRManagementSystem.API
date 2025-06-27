using AutoMapper;
using HRManagementSystem.Application.Dtos.Salary;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalaryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ===================== POST METHODS =====================

        // POST: api/salary/adjustment
        // [POST] Add salary adjustment (bonus or deduction) for an employee
        [HttpPost("adjustment")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult AddSalaryAdjustment([FromBody] CreateSalaryAdjustmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == request.EmployeeId);
            if (employee == null)
                return NotFound(new { success = false, message = "Employee not found" });

            var adjustment = _mapper.Map<SalaryAdjustment>(request);

            _unitOfWork.SalaryAdjustments.Add(adjustment);
            _unitOfWork.Complete();

            return Ok(new { success = true, message = "Salary adjustment added successfully." });
        }

        // POST: api/salary/generate
        // [POST] Generate salary for a specific employee for a month
        [HttpPost("generate")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GenerateSalary([FromQuery] Guid employeeId, [FromQuery] int month, [FromQuery] int year)
        {
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
                return NotFound(new { success = false, message = "Employee not found." });

            var existingSalary = _unitOfWork.Salaries.FirstOrDefault(
                s => s.EmployeeId == employeeId && s.Month == month && s.Year == year);

            if (existingSalary != null)
                return BadRequest(new { success = false, message = "Salary already generated for this month." });

            var adjustments = _unitOfWork.SalaryAdjustments.GetAll(
                a => a.EmployeeId == employeeId &&
                     a.EffectiveDate.Month == month &&
                     a.EffectiveDate.Year == year);

            decimal totalBonus = adjustments.Where(a => a.Type == AdjustmentType.Bonus).Sum(a => a.Amount);
            decimal totalDeduction = adjustments.Where(a => a.Type == AdjustmentType.Deduction).Sum(a => a.Amount);
            decimal finalAmount = (employee.Salary ?? 0) + totalBonus - totalDeduction;

            var salary = new Salary
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year,
                Amount = finalAmount,
                IsPaid = false
            };

            _unitOfWork.Salaries.Add(salary);
            _unitOfWork.Complete();

            return Ok(new
            {
                success = true,
                message = "Salary generated successfully.",
                data = new GenerateSalaryResponse
                {
                    BaseSalary = employee.Salary ?? 0,
                    TotalBonus = totalBonus,
                    TotalDeduction = totalDeduction,
                    FinalAmount = finalAmount
                }
            });
        }

        // PATCH: api/salary/mark-paid/{salaryId}
        // [PATCH] Mark a salary as paid
        [HttpPatch("mark-paid/{salaryId}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult MarkSalaryAsPaid(Guid salaryId)
        {
            var salary = _unitOfWork.Salaries.FirstOrDefault(s => s.Id == salaryId);
            if (salary == null)
                return NotFound(new { success = false, message = "Salary not found." });

            if (salary.IsPaid)
                return BadRequest(new { success = false, message = "Salary is already marked as paid." });
    
            salary.IsPaid = true;

            _unitOfWork.Salaries.Update(salary);
            _unitOfWork.Complete();

            return Ok(new { success = true, message = "Salary marked as paid successfully." });
        }

        // ===================== GET METHODS =====================

        // GET: api/salary/details
        // [GET] Get salary details for a specific employee and month
        [HttpGet("details")]
        [Authorize(Roles = "HR,Admin,Employee")]
        public IActionResult GetSalaryDetails([FromQuery] Guid employeeId, [FromQuery] int month, [FromQuery] int year)
        {
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
                return NotFound(new { success = false, message = "Employee not found." });

            var salary = _unitOfWork.Salaries.FirstOrDefault(s => s.EmployeeId == employeeId && s.Month == month && s.Year == year);
            if (salary == null)
                return NotFound(new { success = false, message = "Salary record not found for this month." });

            var adjustments = _unitOfWork.SalaryAdjustments.GetAll(
                a => a.EmployeeId == employeeId &&
                     a.EffectiveDate.Month == month &&
                     a.EffectiveDate.Year == year);

            var bonuses = adjustments.Where(a => a.Type == AdjustmentType.Bonus).ToList();
            var deductions = adjustments.Where(a => a.Type == AdjustmentType.Deduction).ToList();

            var response = new SalaryDetailsResponse
            {
                EmployeeName = employee.FullName,
                Month = month,
                Year = year,
                BaseSalary = employee.Salary ?? 0,
                TotalBonus = bonuses.Sum(b => b.Amount),
                TotalDeduction = deductions.Sum(d => d.Amount),
                FinalAmount = salary.Amount,
                IsPaid = salary.IsPaid,
                Bonuses = _mapper.Map<List<SalaryAdjustmentItemDto>>(bonuses),
                Deductions = _mapper.Map<List<SalaryAdjustmentItemDto>>(deductions)
            };

            return Ok(new { success = true, message = "Salary details retrieved successfully.", data = response });
        }

        // GET: api/salary/all
        // [GET] Get all salaries
        [HttpGet("all")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetAllSalaries()
        {
            var salaries = _unitOfWork.Salaries.GetAll(null, s => s.Employee, s => s.Employee.Department);

            if (!salaries.Any())
                return NotFound(new { success = false, message = "No salary records found." });

            var result = _mapper.Map<List<SalaryListResponse>>(salaries)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .ToList();

            return Ok(new { success = true, message = "Salaries retrieved successfully.", data = result });
        }

        // ===================== DELETE METHODS =====================

        //DELETE: api/salary/{id}
        // [DELETE] Delete a specific salary
        [HttpDelete("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult DeleteSalary(Guid id)
        {
            var salary = _unitOfWork.Salaries.FirstOrDefault(a => a.Id == id);
            if (salary == null)
                return NotFound(new { success = false, message = "Salary not found." });

            _unitOfWork.Salaries.Remove(salary);
            _unitOfWork.Complete();

            return Ok(new { success = true, message = "Salary deleted successfully." });
        }

        // DELETE: api/salary/adjustment/{id}
        // [DELETE] Delete a salary adjustment
        [HttpDelete("adjustment/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult DeleteAdjustment(Guid id)
        {
            var adjustment = _unitOfWork.SalaryAdjustments.FirstOrDefault(a => a.Id == id);
            if (adjustment == null)
                return NotFound(new { success = false, message = "Adjustment not found." });

            _unitOfWork.SalaryAdjustments.Remove(adjustment);
            _unitOfWork.Complete();

            return Ok(new { success = true, message = "Adjustment deleted successfully." });
        }
    }
}

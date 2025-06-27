using AutoMapper;
using HRManagementSystem.Application.Dtos.PerformanceReview;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PerformanceReviewController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ===================== GET Methods =====================

        // Get: api/PerformanceReview/employee/{employeeId}
        // [GET] Retrieve all performance reviews for a specific employee
        [HttpGet("employee/{employeeId}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetReviewsForEmployee(Guid employeeId)
        {
            var reviews = _unitOfWork.PerformanceReviews
                .GetAll(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
            if (!reviews.Any())
                return NotFound(new { success = false, message = "No performance reviews found for this employee." });

            var result = _mapper.Map<List<PerformanceReviewResponse>>(reviews);
            return Ok(new
            {
                success = true,
                message = "Performance reviews retrieved successfully.",
                data = result
            });
        }

        // ===================== POST METHODS =====================

        // POST: api/PerformanceReview/add
        // [POST] Add a new performance review for an employee
        [HttpPost("add")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult AddPerformanceReview([FromBody] PerformanceReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == request.EmployeeId);
            if (employee == null)
                return NotFound("Employee not found.");

            var review = _mapper.Map<PerformanceReview>(request);

            _unitOfWork.PerformanceReviews.Add(review);
            _unitOfWork.Complete();

            var response = _mapper.Map<PerformanceReviewResponse>(review);
            return Ok(new
            {
                message = "Review added successfully.",
                data = response
            });
        }

        // ===================== PUT METHODS =====================

        // PUT: api/PerformanceReview/update/{id}
        // [PUT] Update an existing performance review
        [HttpPut("update/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult UpdateReview(Guid id, [FromBody] PerformanceReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState });

            var review = _unitOfWork.PerformanceReviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return NotFound(new { success = false, message = "Performance review not found." });

            _mapper.Map(request, review);

            _unitOfWork.PerformanceReviews.Update(review);
            _unitOfWork.Complete();

            var response = _mapper.Map<PerformanceReviewResponse>(review);
            return Ok(new
            {
                success = true,
                message = "Performance review updated successfully.",
                data = response
            });
        }

        // ===================== DELETE METHODS =====================

        // DELETE: api/PerformanceReview/{id}
        // [DELETE] Delete a specific performance review
        [HttpDelete("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult DeleteReview(Guid id)
        {
            var review = _unitOfWork.PerformanceReviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return NotFound(new { success = false, message = "Performance review not found." });

            _unitOfWork.PerformanceReviews.Remove(review);
            _unitOfWork.Complete();

            return Ok(new
            {
                success = true,
                message = "Performance review deleted successfully."
            });
        }
    }
}

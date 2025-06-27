using AutoMapper;
using HeyRed.Mime;
using HRManagementSystem.Application.Dtos.Department;
using HRManagementSystem.Application.Dtos.EmployeeDocument;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDocumentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmployeeDocumentController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ===================== POST Method =====================

        // Post: api/EmployeeDocument/add
        // [POST] Upload new document for a specific employee
        [HttpPost("add")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> UploadDocument([FromForm] EmployeeDocumentRequest request, IFormFile file)
        {
            // Check if file is uploaded
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Verify employee existence
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == request.EmployeeId);
            if (employee == null)
                return NotFound(new { success = false, message = "Employee not found." });

            // Prepare storage folder path
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique file name and save the file
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Save metadata in database
            var document = new EmployeeDocument
            {
                EmployeeId = request.EmployeeId,
                FileName = request.FileName,
                Description = request.Description,
                FilePath = "/documents/" + uniqueFileName
            };

            _unitOfWork.EmployeeDocuments.Add(document);
            _unitOfWork.Complete();

            return Ok(new
            {
                success = true,
                message = "Document uploaded successfully.",
                data = _mapper.Map<EmployeeDocumentResponse>(document)
            });

        }

        // ===================== PUT METHOD =====================

        // Put: api/EmployeeDocument/update/{id}
        // [PUT] Update existing document for a specific employee or replace the file
        [HttpPut("update/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> UpdateDocument(Guid id, [FromForm] EmployeeDocumentRequest request, IFormFile? file)
        {
            // Check if document exists
            var document = _unitOfWork.EmployeeDocuments.FirstOrDefault(d => d.Id == id);
            if (document == null)
                return NotFound(new { success = false, message = "Document not found." });

            // Update document info
            document.FileName = request.FileName;
            document.Description = request.Description;

            // If file is replaced, delete old and upload new
            if (file != null && file.Length > 0)
            {
                // Get the path of the old file stored on the server
                // Delete the old file if it exists
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);

                // Define the folder path where documents are stored
                // Create the folder if it doesn't exist
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate a unique file name using a GUID and preserve the original file extension
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                // Build the full path for the new file 
                var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the new file to the server
                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Update the document's file path in the database entity
                document.FilePath = "/documents/" + uniqueFileName;
            }

            _unitOfWork.EmployeeDocuments.Update(document);
            _unitOfWork.Complete();

            return Ok(new
            {
                success = true,
                message = "Document updated successfully.",
                data = _mapper.Map<EmployeeDocumentResponse>(document)
            });
        }

        // ===================== GET METHODS =====================

        // Get: api/EmployeeDocument/employee/{employeeId}
        // [GET] Get all documents for a specific employee by HR/Admin
        [HttpGet("employee/{employeeId}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetDocumentsByEmployee(Guid employeeId)
        {
            var documents = _unitOfWork.EmployeeDocuments
                .GetAll(d => d.EmployeeId == employeeId);

            if (!documents.Any())
                return NotFound(new { success = false, message = "No documents found for this employee." });

            var response = _mapper.Map<List<EmployeeDocumentResponse>>(documents);

            return Ok(new
            {
                success = true,
                message = "Documents retrieved successfully.",
                data = response
            });
        }

        // Get: api/EmployeeDocument/my-documents
        // [GET] Get all documents uploaded by the current employee
        [HttpGet("my-documents")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMyDocuments()
        {
            var employeeIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (employeeIdClaim == null)
                return Unauthorized(new { success = false, message = "Invalid token." });

            Guid employeeId = Guid.Parse(employeeIdClaim.Value);

            var documents = _unitOfWork.EmployeeDocuments.GetAll(d => d.EmployeeId == employeeId);

            if (!documents.Any())
                return NotFound(new { success = false, message = "No documents found for this employee." });

            var response = _mapper.Map<List<EmployeeDocumentResponse>>(documents);
            return Ok(new
            {
                success = true,
                message = "Documents retrieved successfully.",
                data = response
            });
        }

        // Get: api/EmployeeDocument/download/{id}
        // [GET] Download a specific document by ID
        [HttpGet("download/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult DownloadDocument(Guid id)
        {
            var document = _unitOfWork.EmployeeDocuments.FirstOrDefault(d => d.Id == id);
            if (document == null)
                return NotFound(new { success = false, message = "Document not found." });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { success = false, message = "File not found on the server." });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            string contentType = MimeTypesMap.GetMimeType(document.FileName);
            return File(fileBytes, contentType, document.FileName);
        }

        // ===================== DELETE METHOD =====================

        // Delete: api/EmployeeDocument/{id}
        // [DELETE] Delete a specific document by ID
        [HttpDelete("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult DeleteDocument(Guid id)
        {
            var document = _unitOfWork.EmployeeDocuments.FirstOrDefault(d => d.Id == id);
            if (document == null)
                return NotFound(new { success = false, message = "Document not found." });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _unitOfWork.EmployeeDocuments.Remove(document);
            _unitOfWork.Complete();

            return Ok(new { success = true, message = "Document deleted successfully." });
        }
    }
}

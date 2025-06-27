
using AutoMapper;
using HRManagementSystem.Application.Dtos;
using HRManagementSystem.Application.Dtos.Account;
using HRManagementSystem.Application.Dtos.Attendance;
using HRManagementSystem.Application.Dtos.Department;
using HRManagementSystem.Application.Dtos.Employee;
using HRManagementSystem.Application.Dtos.EmployeeDocument;
using HRManagementSystem.Application.Dtos.LeaveRequest;
using HRManagementSystem.Application.Dtos.Notification;
using HRManagementSystem.Application.Dtos.PerformanceReview;
using HRManagementSystem.Application.Dtos.Project;
using HRManagementSystem.Application.Dtos.Salary;
using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Account
            CreateMap<RegisterRequest, ApplicationUser>();
            CreateMap<LoginRequest, ApplicationUser>();

            //Department
            CreateMap<Department, DepartmentResponse>();
            CreateMap<DepartmentRequest, Department>();

            //Employee
            CreateMap<Employee, EmployeeResponse>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.PerformanceReviews, opt => opt.MapFrom(src => src.PerformanceReviews));


            CreateMap<EmployeeRequest, Employee>()
                .ForMember(dest => dest.Salary, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) => srcMember != null));
            CreateMap<UpdateEmployeeByHRRequest, Employee>();


            // CreateSalaryAdjustmentRequest → SalaryAdjustment
            CreateMap<CreateSalaryAdjustmentRequest, SalaryAdjustment>();

            // Salary → SalaryListResponse
            CreateMap<Salary, SalaryListResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Employee.Department.Name))
                .ForMember(dest => dest.BaseSalary, opt => opt.MapFrom(src => src.Employee.Salary ?? 0));


            // SalaryAdjustment → SalaryAdjustmentItemDto
            CreateMap<SalaryAdjustment, SalaryAdjustmentItemDto>();


            //LeaveRequest
            CreateMap<LeaveRequest, LeaveRequestResponse>()
               .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


            CreateMap<LeaveRequestRequest, LeaveRequest>();



            //Attendance
            CreateMap<Attendance, AttendanceResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
                
            CreateMap<AttendanceRequest, Attendance>();


            //EmployeeDocument
            CreateMap<EmployeeDocumentRequest, EmployeeDocument>();
            CreateMap<EmployeeDocument, EmployeeDocumentResponse>();

            // Notification 
            CreateMap<NotificationRequest, Notification>();
            CreateMap<Notification, NotificationResponse>();


            //PerformanceReview
            CreateMap<PerformanceReviewRequest, PerformanceReview>();
            CreateMap<PerformanceReview, PerformanceReviewResponse>();



            // Project <-> ProjectResponse
            CreateMap<Project, ProjectResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


            // CreateProjectRequest -> Project
            CreateMap<ProjectRequest, Project>();

            // ProjectAssignment -> ProjectAssignmentResponse
            CreateMap<ProjectAssignment, ProjectAssignmentResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name));

            // AssignEmployeeToProjectRequest -> ProjectAssignment
            CreateMap<AssignEmployeeToProjectRequest, ProjectAssignment>();


        }
    }
}

using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.DTOs;

public record UserDto(
    string Id,
    string FullName,
    string Email,
    UserRole Role,
    int? DepartmentId,
    string? DepartmentName,
    bool CanViewAllDepartments
);

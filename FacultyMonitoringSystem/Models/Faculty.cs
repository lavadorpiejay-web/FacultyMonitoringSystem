namespace FacultyMonitoringSystem.Models;

public class Faculty
{
    public string Id { get; set; } =
        Guid.NewGuid().ToString();

    public string EmployeeId { get; set; } = "";

    public string FullName { get; set; } = "";

    public string Department { get; set; } = "";

    public string Position { get; set; } = "";

    public bool IsActive { get; set; } = true;
}
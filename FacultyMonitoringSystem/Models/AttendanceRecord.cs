namespace FacultyMonitoringSystem.Models;

public class AttendanceRecord
{
    public string Id { get; set; } =
        Guid.NewGuid().ToString();

    public string FacultyId { get; set; } = "";

    public string EmployeeId { get; set; } = "";

    public string FacultyName { get; set; } = "";

    public DateTime Date { get; set; }

    public DateTime? TimeIn { get; set; }

    public DateTime? TimeOut { get; set; }

    public string Status { get; set; } = "Absent";

    public double TotalHours { get; set; }
}
using System.Text.Json;
using FacultyMonitoringSystem.Models;

namespace FacultyMonitoringSystem.Services;

public class JsonDataService
{
    private readonly string _facultyFile;
    private readonly string _attendanceFile;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonDataService()
    {
        _facultyFile = Path.Combine(
            FileSystem.AppDataDirectory,
            "faculties.json");

        _attendanceFile = Path.Combine(
            FileSystem.AppDataDirectory,
            "attendance.json");
    }

    // =========================================
    // FACULTY
    // =========================================

    public async Task<List<Faculty>> GetFacultiesAsync()
    {
        if (!File.Exists(_facultyFile))
            return new List<Faculty>();

        try
        {
            string json =
                await File.ReadAllTextAsync(_facultyFile);

            return JsonSerializer.Deserialize<List<Faculty>>(
                       json,
                       _options)
                   ?? new List<Faculty>();
        }
        catch
        {
            return new List<Faculty>();
        }
    }

    public async Task SaveFacultiesAsync(
        List<Faculty> faculties)
    {
        string json =
            JsonSerializer.Serialize(
                faculties,
                _options);

        await File.WriteAllTextAsync(
            _facultyFile,
            json);
    }

    public async Task AddFacultyAsync(
        Faculty faculty)
    {
        var faculties =
            await GetFacultiesAsync();

        faculties.Add(faculty);

        await SaveFacultiesAsync(faculties);
    }

    public async Task DeleteFacultyAsync(
        string facultyId)
    {
        var faculties =
            await GetFacultiesAsync();

        var faculty =
            faculties.FirstOrDefault(
                x => x.Id == facultyId);

        if (faculty != null)
        {
            faculty.IsActive = false;

            await SaveFacultiesAsync(faculties);
        }
    }

    // =========================================
    // ATTENDANCE
    // =========================================

    public async Task<List<AttendanceRecord>>
        GetAttendanceAsync()
    {
        if (!File.Exists(_attendanceFile))
            return new List<AttendanceRecord>();

        try
        {
            string json =
                await File.ReadAllTextAsync(
                    _attendanceFile);

            return JsonSerializer.Deserialize<
                       List<AttendanceRecord>>(
                           json,
                           _options)
                   ?? new List<AttendanceRecord>();
        }
        catch
        {
            return new List<AttendanceRecord>();
        }
    }

    public async Task SaveAttendanceAsync(
        List<AttendanceRecord> attendance)
    {
        string json =
            JsonSerializer.Serialize(
                attendance,
                _options);

        await File.WriteAllTextAsync(
            _attendanceFile,
            json);
    }

    public async Task<AttendanceRecord?>
        GetTodayAttendanceAsync(
            string facultyId)
    {
        var attendance =
            await GetAttendanceAsync();

        return attendance.FirstOrDefault(x =>
            x.FacultyId == facultyId &&
            x.Date.Date == DateTime.Today);
    }

    public async Task TimeInAsync(
        Faculty faculty)
    {
        var attendance =
            await GetAttendanceAsync();

        var record =
            attendance.FirstOrDefault(x =>
                x.FacultyId == faculty.Id &&
                x.Date.Date == DateTime.Today);

        if (record == null)
        {
            record = new AttendanceRecord
            {
                FacultyId = faculty.Id,
                EmployeeId = faculty.EmployeeId,
                FacultyName = faculty.FullName,
                Date = DateTime.Today,
                TimeIn = DateTime.Now,
                Status = "Present"
            };

            attendance.Add(record);
        }
        else
        {
            record.TimeIn = DateTime.Now;
            record.Status = "Present";
        }

        await SaveAttendanceAsync(attendance);
    }

    public async Task TimeOutAsync(
        Faculty faculty)
    {
        var attendance =
            await GetAttendanceAsync();

        var record =
            attendance.FirstOrDefault(x =>
                x.FacultyId == faculty.Id &&
                x.Date.Date == DateTime.Today);

        if (record == null ||
            record.TimeIn == null)
            return;

        record.TimeOut = DateTime.Now;

        record.TotalHours =
            (record.TimeOut.Value -
             record.TimeIn.Value)
            .TotalHours;

        await SaveAttendanceAsync(attendance);
    }
}
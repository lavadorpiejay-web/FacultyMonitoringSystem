using FacultyMonitoringSystem.Models;
using FacultyMonitoringSystem.Services;

namespace FacultyMonitoringSystem.Pages;

public partial class AttendancePage : ContentPage
{
    private readonly JsonDataService _dataService;

    private readonly bool _showHistory;

    private List<AttendanceViewItem> _items = new();

    public AttendancePage(
        JsonDataService dataService,
        bool showHistory = false)
    {
        InitializeComponent();

        _dataService = dataService;

        _showHistory = showHistory;

        Title = showHistory
            ? "Attendance History"
            : "Today's Attendance";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadAttendanceAsync();
    }

    private async Task LoadAttendanceAsync()
    {
        var faculties =
            await _dataService.GetFacultiesAsync();

        var attendance =
            await _dataService.GetAttendanceAsync();

        _items = new List<AttendanceViewItem>();

        if (_showHistory)
        {
            foreach (var record in attendance
                .OrderByDescending(x => x.Date))
            {
                var faculty =
                    faculties.FirstOrDefault(
                        x => x.Id == record.FacultyId);

                _items.Add(
                    new AttendanceViewItem
                    {
                        Id = record.Id,

                        FacultyId =
                            record.FacultyId,

                        FacultyName =
                            record.FacultyName,

                        EmployeeId =
                            record.EmployeeId,

                        Department =
                            faculty?.Department ?? "",

                        Status =
                            record.Status,

                        TimeIn =
                            record.TimeIn,

                        TimeOut =
                            record.TimeOut,

                        TotalHours =
                            record.TotalHours
                    });
            }
        }
        else
        {
            foreach (var faculty in
                faculties.Where(x => x.IsActive))
            {
                var record =
                    attendance.FirstOrDefault(x =>
                        x.FacultyId == faculty.Id &&
                        x.Date.Date == DateTime.Today);

                _items.Add(
                    new AttendanceViewItem
                    {
                        Id =
                            record?.Id ?? "",

                        FacultyId =
                            faculty.Id,

                        FacultyName =
                            faculty.FullName,

                        EmployeeId =
                            faculty.EmployeeId,

                        Department =
                            faculty.Department,

                        Status =
                            record?.Status ?? "Absent",

                        TimeIn =
                            record?.TimeIn,

                        TimeOut =
                            record?.TimeOut,

                        TotalHours =
                            record?.TotalHours ?? 0
                    });
            }
        }

        AttendanceCollection.ItemsSource =
            _items;
    }

    // ========================================
    // TIME IN
    // ========================================

    private async void TimeIn_Clicked(
        object sender,
        EventArgs e)
    {
        if (_showHistory)
            return;

        if (sender is not Button button)
            return;

        string? facultyId =
            button.CommandParameter?.ToString();

        if (string.IsNullOrWhiteSpace(facultyId))
            return;

        var faculties =
            await _dataService.GetFacultiesAsync();

        var faculty =
            faculties.FirstOrDefault(
                x => x.Id == facultyId);

        if (faculty == null)
            return;

        var existing =
            await _dataService
                .GetTodayAttendanceAsync(
                    faculty.Id);

        if (existing?.TimeIn != null)
        {
            await DisplayAlert(
                "Already Timed In",
                $"{faculty.FullName} already timed in today.",
                "OK");

            return;
        }

        await _dataService.TimeInAsync(
            faculty);

        await LoadAttendanceAsync();
    }

    // ========================================
    // TIME OUT
    // ========================================

    private async void TimeOut_Clicked(
        object sender,
        EventArgs e)
    {
        if (_showHistory)
            return;

        if (sender is not Button button)
            return;

        string? facultyId =
            button.CommandParameter?.ToString();

        if (string.IsNullOrWhiteSpace(facultyId))
            return;

        var faculties =
            await _dataService.GetFacultiesAsync();

        var faculty =
            faculties.FirstOrDefault(
                x => x.Id == facultyId);

        if (faculty == null)
            return;

        var existing =
            await _dataService
                .GetTodayAttendanceAsync(
                    faculty.Id);

        if (existing?.TimeIn == null)
        {
            await DisplayAlert(
                "No Time In",
                "Faculty member has not timed in yet.",
                "OK");  

            return;
        }

        if (existing.TimeOut != null)
        {
            await DisplayAlert(
                "Already Timed Out",
                "Faculty member already timed out.",
                "OK");

            return;
        }

        await _dataService.TimeOutAsync(
            faculty);

        await LoadAttendanceAsync();
    }

    // ========================================
    // SEARCH
    // ========================================

    private void SearchBar_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        string search =
            e.NewTextValue?.ToLower() ?? "";

        AttendanceCollection.ItemsSource =
            _items
                .Where(x =>
                    x.FacultyName
                        .ToLower()
                        .Contains(search) ||

                    x.EmployeeId
                        .ToLower()
                        .Contains(search) ||

                    x.Department
                        .ToLower()
                        .Contains(search))
                .ToList();
    }
}


// ============================================
// ATTENDANCE VIEW ITEM
// ============================================

public class AttendanceViewItem
{
    public string Id { get; set; } = "";

    public string FacultyId { get; set; } = "";

    public string FacultyName { get; set; } = "";

    public string EmployeeId { get; set; } = "";

    public string Department { get; set; } = "";

    public string Status { get; set; } = "Absent";

    public DateTime? TimeIn { get; set; }

    public DateTime? TimeOut { get; set; }

    public double TotalHours { get; set; }
}
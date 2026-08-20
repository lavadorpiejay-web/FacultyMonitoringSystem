using FacultyMonitoringSystem.Models;
using FacultyMonitoringSystem.Services;

namespace FacultyMonitoringSystem.Pages;

public partial class FacultyPage : ContentPage
{
    private readonly JsonDataService _dataService;

    private List<Faculty> _faculties = new();

    public FacultyPage(
        JsonDataService dataService)
    {
        InitializeComponent();

        _dataService = dataService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadFacultyAsync();
    }

    private async Task LoadFacultyAsync()
    {
        _faculties =
            await _dataService.GetFacultiesAsync();

        FacultyCollection.ItemsSource =
            _faculties
                .Where(x => x.IsActive)
                .ToList();
    }

    private async void AddFaculty_Clicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
                EmployeeIdEntry.Text))
        {
            await DisplayAlert(
                "Required",
                "Please enter an Employee ID.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(
                NameEntry.Text))
        {
            await DisplayAlert(
                "Required",
                "Please enter the faculty name.",
                "OK");

            return;
        }

        var faculty = new Faculty
        {
            EmployeeId =
                EmployeeIdEntry.Text.Trim(),

            FullName =
                NameEntry.Text.Trim(),

            Department =
                DepartmentEntry.Text?.Trim() ?? "",

            Position =
                PositionEntry.Text?.Trim() ?? "",

            IsActive = true
        };

        await _dataService.AddFacultyAsync(
            faculty);

        EmployeeIdEntry.Text = "";
        NameEntry.Text = "";
        DepartmentEntry.Text = "";
        PositionEntry.Text = "";

        await LoadFacultyAsync();

        await DisplayAlert(
            "Success",
            "Faculty member added successfully.",
            "OK");
    }

    private async void Delete_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;

        string? facultyId =
            button.CommandParameter?.ToString();

        if (string.IsNullOrWhiteSpace(facultyId))
            return;

        bool confirm =
            await DisplayAlert(
                "Delete Faculty",
                "Are you sure you want to remove this faculty member?",
                "Yes",
                "No");

        if (!confirm)
            return;

        await _dataService.DeleteFacultyAsync(
            facultyId);

        await LoadFacultyAsync();
    }

    private void SearchBar_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        string search =
            e.NewTextValue?.ToLower() ?? "";

        FacultyCollection.ItemsSource =
            _faculties
                .Where(x => x.IsActive)
                .Where(x =>
                    x.FullName
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
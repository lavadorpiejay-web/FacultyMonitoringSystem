using FacultyMonitoringSystem.Services;

namespace FacultyMonitoringSystem;

public partial class MainPage : ContentPage
{
    private readonly JsonDataService _dataService;

    public MainPage(
        JsonDataService dataService)
    {
        InitializeComponent();

        _dataService = dataService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        var faculties =
            await _dataService.GetFacultiesAsync();

        var attendance =
            await _dataService.GetAttendanceAsync();

        FacultyCountLabel.Text =
            faculties.Count(x => x.IsActive)
            .ToString();

        PresentCountLabel.Text =
            attendance.Count(x =>
                x.Date.Date == DateTime.Today &&
                x.TimeIn != null)
            .ToString();

        DateLabel.Text =
            DateTime.Now.ToString(
                "dddd, MMMM dd, yyyy");
    }

    private async void FacultyManagement_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new Pages.FacultyPage(_dataService));
    }

    private async void Attendance_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new Pages.AttendancePage(_dataService));
    }

    private async void History_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new Pages.AttendancePage(
                _dataService,
                true));
    }
}
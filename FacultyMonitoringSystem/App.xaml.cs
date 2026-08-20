namespace FacultyMonitoringSystem;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        var dataService =
            serviceProvider.GetRequiredService<
                Services.JsonDataService>();

        MainPage =
            new NavigationPage(
                new MainPage(dataService));
    }
}
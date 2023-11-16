using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;
using SquirrelNext.Domain;

namespace SquirrelNext.Wpf;

public partial class MainWindow
{
    private const string RepoUrl = "https://github.com/nicolas-ratushniak/SquirrelNext";
    private UpdateService _updateService;

    public MainWindow()
    {
        UpdateService.BackupSettings("appsettings.json");
        UpdateService.RestoreSettings("appsettings.json");
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private static async Task<IUpdateManager> GetUpdateManager()
    {
        try
        {
            return await UpdateManager.GitHubUpdateManager(RepoUrl);
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show(ex.Message);
            throw;
        }
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _updateService = new UpdateService(await GetUpdateManager(), "appsettings.json");
        CurrentVersionTextBox.Text = _updateService.GetCurrentVersionInstalled();
    }

    private async void CheckForUpdatesButton_Click(object sender, RoutedEventArgs e)
    {
        UpdateButton.IsEnabled = await _updateService.HasReleasesToApply();
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        await _updateService.UpdateApp();
        Application.Current.Shutdown();
    }

    private void SumButton_OnClick(object sender, RoutedEventArgs e)
    {
        var sum = new Summator();
        MessageBox.Show(sum.Sum(4, 5).ToString());
    }
}
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;
using SquirrelNext.Domain;

namespace SquirrelNext.Wpf;

public partial class MainWindow
{
    private const string RepoUrl = "https://github.com/nicolas-ratushniak/SquirrelNext";

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        using var manager = await UpdateManager.GitHubUpdateManager(RepoUrl);
        CurrentVersionTextBox.Text = manager.CurrentlyInstalledVersion()?.ToString() ?? "0.0.0";
    }

    private async void CheckForUpdatesButton_Click(object sender, RoutedEventArgs e)
    {
        using var manager = await UpdateManager.GitHubUpdateManager(RepoUrl);
        var updateInfo = await manager.CheckForUpdate();

        UpdateButton.IsEnabled = updateInfo.ReleasesToApply.Any();
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        await Update();
        Application.Current.Shutdown();
    }

    private void SumButton_OnClick(object sender, RoutedEventArgs e)
    {
        var sum = new Summator();
        MessageBox.Show(sum.Sum(4, 4).ToString());
    }

    private async Task Update()
    {
        using var manager = await UpdateManager.GitHubUpdateManager(RepoUrl);
        try
        {
            await manager.UpdateApp();
            
            var newVersion = manager.CurrentlyInstalledVersion().ToString()!;
            CopyUserSettingToNewRelease("appsettings.json", newVersion);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void CopyUserSettingToNewRelease(string configFileName, string newVersion)
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sourceFile = Path.Combine(exeDir, configFileName);

        if (!File.Exists(sourceFile))
        {
            return;
        }

        var destFile = Path.Combine(
            Directory.GetParent(exeDir)!.FullName, 
            $"app-{newVersion}",
            configFileName);
        File.Copy(sourceFile, destFile, true);
    }
}
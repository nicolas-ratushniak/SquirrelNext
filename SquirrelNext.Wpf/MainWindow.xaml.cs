using System;
using System.Linq;
using System.Windows;
using Squirrel;

namespace SquirrelNext.Wpf;

public partial class MainWindow
{
    private const string RepositoryUrl = "https://github.com/meJevin/WPFFrameworkTest";

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        using var manager = await UpdateManager.GitHubUpdateManager(RepositoryUrl);
        CurrentVersionTextBox.Text = manager.CurrentlyInstalledVersion().ToString();
    }

    private async void CheckForUpdatesButton_Click(object sender, RoutedEventArgs e)
    {
        using var manager = await UpdateManager.GitHubUpdateManager(RepositoryUrl);
        var updateInfo = await manager.CheckForUpdate();

        UpdateButton.IsEnabled = updateInfo.ReleasesToApply.Any();
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        using var manager = await UpdateManager.GitHubUpdateManager(RepositoryUrl);

        try
        {
            await manager.UpdateApp();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        MessageBox.Show("Updated successfully!");
    }
}
using System;
using System.Linq;
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
        using var manager = await UpdateManager.GitHubUpdateManager(RepoUrl);

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

    private void SumButton_OnClick(object sender, RoutedEventArgs e)
    {
        var sum = new Summator();
        MessageBox.Show(sum.Sum(2, 2).ToString());
    }
}
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Squirrel;

namespace SquirrelNext.Wpf;

public class UpdateService
{
    private readonly IUpdateManager _manager;
    private readonly string _configFileName;

    public UpdateService(IUpdateManager manager, string configFileName)
    {
        _manager = manager;
        _configFileName = configFileName;
    }

    public string GetCurrentVersionInstalled()
    {
        return _manager.CurrentlyInstalledVersion(Assembly.GetExecutingAssembly().Location).ToString()!;
    }

    public async Task<bool> HasReleasesToApply()
    {
        var updateInfo = await _manager.CheckForUpdate();
        return updateInfo.ReleasesToApply.Any();
    }

    public async Task UpdateApp()
    {
        BackupSettings(_configFileName);
        await _manager.UpdateApp();
    }
    
    public static void BackupSettings(string configFileName)
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sourceFile = Path.Combine(exeDir, configFileName);

        if (!File.Exists(sourceFile))
        {
            return;
        }

        var destDir = Path.Combine(Directory.GetParent(exeDir)!.FullName, "settings_backup");

        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }
        
        var destFile = Path.Combine(destDir, configFileName);
        File.Copy(sourceFile, destFile, true);
    }

    public static void RestoreSettings(string configFileName)
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var sourceFile = Path.Combine(Directory.GetParent(exeDir)!.FullName, "settings_backup", configFileName);

        if (!File.Exists(sourceFile))
        {
            return;
        }

        var destFile = Path.Combine(exeDir, configFileName);
        File.Copy(sourceFile, destFile, false);
    }
}
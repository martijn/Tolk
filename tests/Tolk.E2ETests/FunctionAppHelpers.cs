using System.Diagnostics;

namespace Tolk.E2ETests;

internal static class FunctionAppHelpers
{
    
    internal static async Task StartFunctionApp()
    {
        var funcProcess = new Process();

        var rootDir = Path.GetFullPath(@"../../../../..");
        var e2eAppBinPath = Path.Combine(rootDir, @"src/Tolk.FunctionApp/bin/Debug/net7.0");
        string e2eHostJson = Directory.GetFiles(e2eAppBinPath, "host.json", SearchOption.AllDirectories).FirstOrDefault();

        if (e2eHostJson == null)
        {
            throw new InvalidOperationException($"Could not find a built worker app under '{e2eAppBinPath}'");
        }

        var e2eAppPath = Path.GetDirectoryName(e2eHostJson);

        var cliPath = "/usr/local/bin/func";

        if (!File.Exists(cliPath))
        {
            throw new InvalidOperationException($"Could not find '{cliPath}'.");
        }
        
        funcProcess.StartInfo.UseShellExecute = false;
        funcProcess.StartInfo.RedirectStandardError = true;
        funcProcess.StartInfo.RedirectStandardOutput = true;
        funcProcess.StartInfo.CreateNoWindow = true;
        funcProcess.StartInfo.WorkingDirectory = e2eAppPath;
        funcProcess.StartInfo.FileName = cliPath;
        funcProcess.StartInfo.ArgumentList.Add("host");
        funcProcess.StartInfo.ArgumentList.Add("start");
        funcProcess.StartInfo.ArgumentList.Add("--csharp");
        funcProcess.StartInfo.ArgumentList.Add("--verbose");
        
        funcProcess.Start();

        // TODO Wait until app is responsive, with timeout
        await Task.Delay(2500);
    }

    internal static void StopFunctionApp()
    {
        foreach (var func in Process.GetProcessesByName("func"))
        {
            try
            {
                func.Kill();
            }
            catch
            {
                // Best effort
            }
        }
    }
}

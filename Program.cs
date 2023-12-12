using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace Lethal_Company_Mod_Manager;

internal class Program
{
    private static string localVersion = "1.02";

    private static async Task Main() => await LoadMenu();

    private static async Task<bool> LoadMenu()
    {
        Console.Title = $"Lethal Company Mod Manager v{localVersion} | Made by Nitro";

        Console.Clear();
        Console.WriteLine("[Mod Manager] Checking Mod Manager Version");
        CheckForUpdate();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Clear();
        Console.WriteLine("██╗     ███████╗████████╗██╗  ██╗ █████╗ ██╗          ██████╗ ██████╗ ███╗   ███╗██████╗  █████╗ ███╗   ██╗██╗   ██╗");
        Console.WriteLine("██║     ██╔════╝╚══██╔══╝██║  ██║██╔══██╗██║         ██╔════╝██╔═══██╗████╗ ████║██╔══██╗██╔══██╗████╗  ██║╚██╗ ██╔╝");
        Console.WriteLine("██║     █████╗     ██║   ███████║███████║██║         ██║     ██║   ██║██╔████╔██║██████╔╝███████║██╔██╗ ██║ ╚████╔╝ ");
        Console.WriteLine("██║     ██╔══╝     ██║   ██╔══██║██╔══██║██║         ██║     ██║   ██║██║╚██╔╝██║██╔═══╝ ██╔══██║██║╚██╗██║  ╚██╔╝  ");
        Console.WriteLine("███████╗███████╗   ██║   ██║  ██║██║  ██║███████╗    ╚██████╗╚██████╔╝██║ ╚═╝ ██║██║     ██║  ██║██║ ╚████║   ██║   ");
        Console.WriteLine("╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝     ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝   ╚═╝   ");
        Console.WriteLine("███╗   ███╗ ██████╗ ██████╗     ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ ");
        Console.WriteLine("████╗ ████║██╔═══██╗██╔══██╗    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝██╔══██╗");
        Console.WriteLine("██╔████╔██║██║   ██║██║  ██║    ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝");
        Console.WriteLine("██║╚██╔╝██║██║   ██║██║  ██║    ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗");
        Console.WriteLine("██║ ╚═╝ ██║╚██████╔╝██████╔╝    ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║");
        Console.WriteLine("╚═╝     ╚═╝ ╚═════╝ ╚═════╝     ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝");

        PrintColoredMessage("\n[Mod Manager] ", $"Welcome to Lethal Company Mod Manager v{localVersion}", ConsoleColor.Blue, ConsoleColor.White);
        PrintColoredMessage("\n\n[Mod Manager] ", $"Press 'Enter' To Start Mod Installation", ConsoleColor.Blue, ConsoleColor.White);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[WARNING] This will uninstall all current mods before installing new ones\n\n");

        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);
        } while (keyInfo.Key != ConsoleKey.Enter);

        return await InstallMods();
    }

    private static async Task<bool> InstallMods()
    {
        try
        {
            // Checks if Steam is installed
            string steamPath = GetSteamPath();
            PrintColoredMessage("\r[Mod Manager] ", "Checking Steam installation" +
                "                              ", ConsoleColor.Blue, ConsoleColor.White);
            if (steamPath == null) return await ErrorHandler.HandleNoneEXError("Steam installation was not found");

            // Checks if Lethal Company is installed
            string lethalComapnyPath = GetSteamLethalCompanyPath(steamPath);
            PrintColoredMessage("\r[Mod Manager] ", "Checking Lethal Company installation" +
                "                              ", ConsoleColor.Blue, ConsoleColor.White);
            if (lethalComapnyPath == null) return await ErrorHandler.HandleNoneEXError("Lethal Company installation was not found");

            // Checks if BepinEx Mod Pack for Lethal Company is installed
            string bepInPath = Path.Combine(lethalComapnyPath, "BepInEx");
            PrintColoredMessage("\r[Mod Manager] ", "Checking BepinEx Mod Manager installation" +
                "                              ", ConsoleColor.Blue, ConsoleColor.White);
            if (!Directory.Exists(bepInPath)) return await ErrorHandler.HandleNoneEXError("BepInEx Mod Manager was not found. You can install it here => https://thunderstore.io/c/lethal-company/p/BepInEx/BepInExPack/");

            // Checks if you have a plugins folder within BepinEx to store the mods
            string pluginsPath = Path.Combine(bepInPath, "plugins");
            PrintColoredMessage("\r[Mod Manager] ", "Uninstalling all current plugins" +
                "                              ", ConsoleColor.Blue, ConsoleColor.White);
            if (!Directory.Exists(pluginsPath)) return await ErrorHandler.HandleNoneEXError("BepInEx Plugins Folder was not found");

            // Delete all previous mods within plugins folder
            await DeleteOldMods(pluginsPath);

            // Download new Plugins from github repo
            PrintColoredMessage("\r[Mod Manager] ", "Installing plugins" +
                "                              ", ConsoleColor.Blue, ConsoleColor.White);
            await HttpHandler.DownloadFilesFromRepo(Path.GetFullPath(pluginsPath), "Plugins");
            await HttpHandler.DownloadFilesFromRepo(Path.GetFullPath(pluginsPath), "Plugins Extra");

            // Installation complete message
            PrintColoredMessage("\r[Mod Manager] ", "Mod Installation is Complete" +
                "                              ", ConsoleColor.Blue, ConsoleColor.Green);
            PrintColoredMessage("\n[Mod Manager] ", "Press Any Key to Exit", ConsoleColor.Blue, ConsoleColor.White);
            Console.ReadKey();
            Environment.Exit(0);
            return true;
        }
        catch (Exception ex) { return await ErrorHandler.HandleError(ex, "Fatal Error During Installation"); }
    }
    public static void PrintColoredMessage(string prefixMessage, string message, ConsoleColor prefixColor, ConsoleColor messageColor)
    {
        Console.ForegroundColor = prefixColor;
        Console.Write(prefixMessage);
        Console.ForegroundColor = messageColor;
        Console.Write(message);
        Console.ResetColor();
    }
    private static async Task<bool> DeleteOldMods(string path)
    {
        string[] filesAndDirs = Directory.GetFileSystemEntries(path);
        foreach (string fileOrDir in filesAndDirs)
        {
            if (File.Exists(fileOrDir))
            {
                File.Delete(fileOrDir);
            }
            else if (Directory.Exists(fileOrDir))
            {
                Directory.Delete(fileOrDir, true);
            }
        }
        await Task.Delay(100);
        return true;
    }

    private static string GetSteamPath()
    {
        // Check correct Steam reg key if 32 bit or 64 bit op sys
        string registryPath = Environment.Is64BitOperatingSystem
            ? @"SOFTWARE\WOW6432Node\Valve\Steam"
            : @"SOFTWARE\Valve\Steam";

        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryPath);


        if (registryKey != null)
        {
            // Find the steam installed path via reg key
            object steamPath = registryKey.GetValue("InstallPath");

            // If the Steam installed path is real return it
            if (steamPath != null) return steamPath.ToString();
        }

        // If no Steam installation found return null
        return null;
    }

    private static string GetSteamLethalCompanyPath(string steamPath)
    {
        string lethalCompanyPath = Path.Combine(steamPath, "steamapps", "common", "Lethal Company");

        // If Lethal Company exists on main drive return it
        if (Directory.Exists(lethalCompanyPath)) return lethalCompanyPath;

        // Check other drives for Lethal Company installation
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (drive.DriveType == DriveType.Fixed && drive.RootDirectory.FullName.ToLower() != Path.GetPathRoot(steamPath).ToLower())
            {
                lethalCompanyPath = Path.Combine(drive.RootDirectory.FullName, "SteamLibrary", "steamapps", "common", "Lethal Company");

                if (Directory.Exists(lethalCompanyPath)) return lethalCompanyPath;
            }
        }

        // If Lethal Company is not found within main drive steam library or other drive steam librarys return null
        return null;
    }
    private static bool CheckForUpdate()
    {
        string cloudVersion = GetCloudVersion().Result;
        if (!cloudVersion.Contains(localVersion))
        {
            Console.WriteLine("\n[Mod Manager] You appear to have an outdated verion of Lethal Company Mod Manager");
            Console.WriteLine($"[Mod Manager] Your current version is {localVersion}, you can update to version {cloudVersion} by redownloading the Mod Manager");
            Console.WriteLine("[Mod Manager] Press any key to continue");
            Console.ReadKey();
            Environment.Exit(0);
            return false;
        }
        return true;
    }
    private static async Task<string> GetCloudVersion()
    {
        string jsonString = await HttpHandler.GetStringAsync("https://raw.githubusercontent.com/Dev-Nitro/Lethal-Company-Mods/main/Manager.json");
        JObject jsonObject = JObject.Parse(jsonString);
        string? version = jsonObject["Version"]?.ToString();
        return version;
    }
}

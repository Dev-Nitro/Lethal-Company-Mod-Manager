
namespace Lethal_Company_Mod_Manager;

public static class ErrorHandler
{
    public static async Task<bool> HandleError(Exception ex, string Message)
    {
        await Task.Delay(100);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n[Mod Manager] Lethal Company Mod Manager Error Occurred..." +
            $"\nMessage: {Message}" +
            $"\nError: {ex.Message}");
        await Task.Delay(100);
        Console.WriteLine("Press Any Key to Continue...");
        Console.ReadKey();
        Environment.Exit(0);
        return true;
    }
    public static async Task<bool> HandleNoneEXError(string Message)
    {
        await Task.Delay(100);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n[Mod Manager] Lethal Company Mod Manager Error Occurred..." +
            $"\nError: {Message}\n\n");
        await Task.Delay(100);
        Console.WriteLine("Press Any Key to Continue...");
        Console.ReadKey();
        Environment.Exit(0);
        return true;
    }
}

// Old debug stuff
// Not used
using AlkaOS.Kernel;
using Godot;

internal class WriteProcessInfo
{
    public static void PrintAll(Kernel kernel)
    {
        foreach (var process in kernel.GetAllProcesses())
        {
            GD.Print(
                $"Process ID: {process.ProcessID}, " +
                $"Name: {process.ProcessName}, " +
                $"State: {process.State}, " +
                $"Priority: {process.Priority} "
            );
        }
    }
}
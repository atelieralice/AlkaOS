using Godot;

public partial class TerminateProcess : Button
{
    public override void _Pressed()
    {
        var main = GetNodeOrNull<Main>("%Main");
        var pidEdit = GetNodeOrNull<TextEdit>("%TerminatePID");
        if (main != null && pidEdit != null && int.TryParse(pidEdit.Text, out int pid))
        {
            main.TerminateProcessByPid(pid);
        }
    }
}
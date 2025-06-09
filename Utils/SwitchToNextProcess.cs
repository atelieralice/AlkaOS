using Godot;

public partial class SwitchToNextProcess : Button
{
    public override void _Pressed()
    {
        var main = GetNodeOrNull<Main>("%Main");
        if (main != null)
        {
            main.SwitchToNextProcess();
        }
    }
}
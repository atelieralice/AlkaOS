using Godot;
using System;

public partial class SwitchProcess : Button {
    public override void _Pressed ( ) {
        var main = GetNodeOrNull<Main> ( "%Main" );
        if ( main != null ) {
            main.SwitchToNextProcess ( );
        }
    }
}
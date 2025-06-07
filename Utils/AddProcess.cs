using Godot;
using System;

public partial class AddProcess : Button {
    public override void _Pressed ( ) {
        var main = GetNodeOrNull<Main> ( "%Main" );
        if ( main != null ) {
            main.AddRandomProcess ( );
        }
    }
}
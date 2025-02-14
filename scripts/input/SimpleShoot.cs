using Godot;
using System;

public partial class SimpleShoot : ControlSchemeBase {
    public override void _Process(double delta) {
        base._Process(delta);
        if (Input.IsActionJustPressed("ui_select")) {
            BubbleGame.Game.Player.Call("Shoot");
        }
    }
}

using System;
using Godot;

public partial class PauseGame : ControlSchemeBase {

    public override void _Process(double delta) {
        base._Process(delta);
        if (Input.IsActionJustPressed("toggle_pause")) {
            BubbleGame.Game.TogglePause();
        }
    }
}

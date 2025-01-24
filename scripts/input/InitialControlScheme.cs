using Godot;
using System;

public partial class InitialControlScheme : ControlSchemeBase {
    
    public override void _Process(double delta) {
        base._Process(delta);

        if (Input.IsActionPressed("ui_left")) {
            BubbleGame.Game.Player.TurnTurret = -1;
        } else if (Input.IsActionPressed("ui_right")) {
            BubbleGame.Game.Player.TurnTurret = 1;
        } else {
            BubbleGame.Game.Player.TurnTurret = 0;
        }

        if (Input.IsActionPressed("track_left")) {
            BubbleGame.Game.Player.SetStrafe(-1);
        } else if (Input.IsActionPressed("track_right")) {
            BubbleGame.Game.Player.SetStrafe(1);
        } else {
            BubbleGame.Game.Player.SetStrafe(0);
        }
    }
}

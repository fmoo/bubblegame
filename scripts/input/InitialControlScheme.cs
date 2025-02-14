using Godot;
using System;

public partial class InitialControlScheme : ControlSchemeBase {
    [Export] MouseControls MouseControls = null;

    void MaybeDisableMouseAim() {
        if (MouseControls != null) {
            MouseControls.DisableMouseAim = true;
        }
    }

    public override void _Process(double delta) {
        base._Process(delta);

        if (Input.IsActionPressed("ui_left")) {
            BubbleGame.Game.Player.Set("TurnTurret", -1);
            MaybeDisableMouseAim();
        } else if (Input.IsActionPressed("ui_right")) {
            BubbleGame.Game.Player.Set("TurnTurret", 1);
            MaybeDisableMouseAim();
        } else {
            BubbleGame.Game.Player.Set("TurnTurret", 0);
        }

        if (Input.IsActionPressed("track_left")) {
            BubbleGame.Game.Player.Call("SetStrafe", -1);
        } else if (Input.IsActionPressed("track_right")) {
            BubbleGame.Game.Player.Call("SetStrafe", 1);
        } else {
            BubbleGame.Game.Player.Call("SetStrafe", 0);
        }
    }
}

using Godot;
using System;

public partial class MouseControls : ControlSchemeBase {
    [Export] PathFollow2D moveTargetPath;

    public override void _Input(InputEvent @event) {
        base._Input(@event);
    }


    public override void _Process(double delta) {
        base._Process(delta);

        var InputDirection = GetLocalMousePosition();
        moveTargetPath.ProgressRatio = (InputDirection.AngleTo(Vector2.Up) - Mathf.Pi) / Mathf.Tau;

    }

}

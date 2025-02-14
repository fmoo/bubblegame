using Godot;
using System;

public partial class MouseControls : ControlSchemeBase {
    [Export] PathFollow2D moveTargetPath;

    bool moveMouseDown = false;

    public bool DisableMouseAim = false;

    public override void _Input(InputEvent @event) {
        base._Input(@event);
        if (!IsVisibleInTree()) return;
        // Right click means shoot
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed) {
            BubbleGame.Game.Player.Call("Shoot");
        }
        // Left click means SetStrafePosition
        if (@event is InputEventMouseButton mouseButton2 && mouseButton2.ButtonIndex == MouseButton.Left) {
            if (mouseButton2.Pressed) {
                moveMouseDown = true;
                var InputDirection = GetLocalMousePosition();
                BubbleGame.Game.Player.Call("SetTrackDestination", InputDirection);
            } else {
                moveMouseDown = false;
            }
        }
        // If mouse moves, re-enable mouse aim
        if (@event is InputEventMouseMotion) {
            DisableMouseAim = false;
        }
    }


    public override void _Process(double delta) {
        base._Process(delta);

        var InputDirection = GetLocalMousePosition();
        if (!DisableMouseAim) {
            moveTargetPath.ProgressRatio = (InputDirection.AngleTo(Vector2.Up) - Mathf.Pi) / Mathf.Tau;
            BubbleGame.Game.Player.Call("TurretLookAt", GetGlobalMousePosition());
        }

        // If the player is holding the button down, update the targeting to reflect
        if (moveMouseDown) {
            BubbleGame.Game.Player.Call("SetTrackDestination", InputDirection);
        }

    }

}

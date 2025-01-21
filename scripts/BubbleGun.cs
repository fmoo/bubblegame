using Godot;
using System;

public partial class BubbleGun : Node2D {
    [Export] PackedScene bubbleScene;
    [Export] PathFollow2D pathFollow;
    [Export] float angleLimit = 80;
    [Export] float bubbleSpeed = 200;
    [Export] float rotationSpeed = 2;
    [Export] float trackSpeed = 20;
    
    public override void _Process(double delta) {
        base._Process(delta);

        // If left/right arrow keys are pressed, rotate the gun
        if (Input.IsActionPressed("ui_left")) {
            RotateGun(-1, delta);
        } else if (Input.IsActionPressed("ui_right")) {
            RotateGun(1, delta);
        }
        if (Input.IsActionPressed("track_left")) {
            pathFollow.ProgressRatio = (pathFollow.ProgressRatio - (float)(delta * trackSpeed) + 1f) % 1f;
        } else if (Input.IsActionPressed("track_right")) {
            pathFollow.ProgressRatio = (pathFollow.ProgressRatio + (float)(delta * trackSpeed)) % 1f;
        }
        if (Input.IsActionJustPressed("ui_select")) {
            Shoot();
        }
    }

    void RotateGun(int direction, double delta) {
        Rotation += direction * (float)delta * rotationSpeed;
        Rotation = Mathf.Clamp(Rotation, -Mathf.DegToRad(angleLimit), Mathf.DegToRad(angleLimit));
    }

    void Shoot() {
        Bubble bubble = bubbleScene.Instantiate<Bubble>();
        BubbleGame.Game.RegisterBubble(bubble);

        // Set the color
        bubble.Modulate = BubbleGame.Game.BubbleQueue.DequeueColor();

        // Trajectory and position        
        bubble.GlobalPosition = GlobalPosition;
        bubble.LinearVelocity = new Vector2(0, -1).Rotated(GlobalRotation) * bubbleSpeed;
    }

}

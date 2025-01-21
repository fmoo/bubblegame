using Godot;
using System;

public partial class BubbleGun : Node2D {
    [Export] PackedScene bubbleScene;
    [Export] PathFollow2D pathFollow;
    [Export] float angleLimit = 80;
    [Export] float bubbleSpeed = 200;
    [Export] float rotationSpeed = 2;
    [Export] float trackSpeed = 20;
    [Export] AnimatedSprite2D animatedSprite;
    [Export] Sprite2D previewSprite;

    public override void _Ready() {
        base._Ready();
        animatedSprite.SpeedScale = 0f;
        animatedSprite.Play();
    }

    public override void _Process(double delta) {
        base._Process(delta);
        previewSprite.GlobalRotation = 0;

        float speedScale = 0f;

        // If left/right arrow keys are pressed, rotate the gun
        if (Input.IsActionPressed("ui_left")) {
            if (RotateGun(-1, delta)) {
                speedScale -= 0.6f;
            }
        } else if (Input.IsActionPressed("ui_right")) {
            if (RotateGun(1, delta)) {
                speedScale += 0.6f;
            }
        }
        if (Input.IsActionPressed("track_left")) {
            pathFollow.ProgressRatio = (pathFollow.ProgressRatio - (float)(delta * trackSpeed) + 1f) % 1f;
            speedScale += 1f;
        } else if (Input.IsActionPressed("track_right")) {
            pathFollow.ProgressRatio = (pathFollow.ProgressRatio + (float)(delta * trackSpeed)) % 1f;
            speedScale -= 1f;
        }
        animatedSprite.SpeedScale = speedScale;
        if (Input.IsActionJustPressed("ui_select")) {
            Shoot();
        }
    }

    bool RotateGun(int direction, double delta) {
        var oldRotation = Rotation;
        Rotation += direction * (float)delta * rotationSpeed;
        Rotation = Mathf.Clamp(Rotation, -Mathf.DegToRad(angleLimit), Mathf.DegToRad(angleLimit));
        return oldRotation != Rotation;
    }

    void Shoot() {
        Bubble bubble = bubbleScene.Instantiate<Bubble>();
        BubbleGame.Game.RegisterBubble(bubble);

        // Set the color
        bubble.Sprite.Texture = BubbleGame.Game.BubbleQueue.DequeueColor();

        // Trajectory and position        
        bubble.GlobalPosition = GlobalPosition;
        bubble.LinearVelocity = new Vector2(0, -1).Rotated(GlobalRotation) * bubbleSpeed;
    }

}

using Godot;
using System;

public partial class BubbleGun : Node2D {
    [Export] PackedScene bubbleScene;
    [Export] PathFollow2D pathFollow;
    [Export] float angleLimit = 80;
    [Export] float bubbleSpeed = 200;
    [Export] float rotationSpeed = 2;
    [Export] float trackSpeed = 20;
    [Export] AnimatedSprite2D animatedBody;
    [Export] Sprite2D turretSprite;
    [Export] Sprite2D bubblePreviewSprite;
    [Signal] public delegate void OnShootEventHandler();

    public int Strafe = 0;
    public int TurnTurret = 0;

    public override void _Ready() {
        base._Ready();
        animatedBody.SpeedScale = 0f;
        animatedBody.Play();
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        bubblePreviewSprite.GlobalRotation = 0;
    }

    public override void _Process(double delta) {
        base._Process(delta);
        bubblePreviewSprite.GlobalRotation = 0;

        float speedScale = 0f;

        // If left/right arrow keys are pressed, rotate the gun
        if (TurnTurret < 0) {
            if (RotateGun(-1, delta)) {
                speedScale -= 0.6f;
            }
        } else if (TurnTurret > 0) {
            if (RotateGun(1, delta)) {
                speedScale += 0.6f;
            }
        }
        if (Strafe < 0) {
            pathFollow.ProgressRatio = (pathFollow.ProgressRatio - (float)(delta * trackSpeed) + 1f) % 1f;
            speedScale += 1f;
        } else if (Strafe > 0) {
            pathFollow.ProgressRatio = (pathFollow.ProgressRatio + (float)(delta * trackSpeed)) % 1f;
            speedScale -= 1f;
        }
        animatedBody.SpeedScale = speedScale;
    }

    bool RotateGun(int direction, double delta) {
        var oldRotation = turretSprite.Rotation;
        turretSprite.Rotation += direction * (float)delta * rotationSpeed;
        turretSprite.Rotation = Mathf.Clamp(turretSprite.Rotation, -Mathf.DegToRad(angleLimit), Mathf.DegToRad(angleLimit));
        return oldRotation != Rotation;
    }

    public void Shoot() {
        BubbleGame.Game.Audio.Shoot();
        Bubble bubble = bubbleScene.Instantiate<Bubble>();
        BubbleGame.Game.RegisterBubble(bubble);

        // Set the color
        bubble.Sprite.Texture = BubbleGame.Game.BubbleQueue.DequeueColor();

        // Trajectory and position        
        bubble.GlobalPosition = turretSprite.GlobalPosition;
        bubble.LinearVelocity = new Vector2(0, -1).Rotated(turretSprite.GlobalRotation) * bubbleSpeed;

        EmitSignal(SignalName.OnShoot);
    }

    public void SetTrackAngle(Vector2 InputDirection) {
        pathFollow.ProgressRatio = -InputDirection.AngleTo(Vector2.Up) / Mathf.Tau;
    }

    public void SetTurretAngle(Vector2 InputDirection) {
        turretSprite.GlobalRotation = InputDirection.AngleTo(Vector2.Down);
        turretSprite.Rotation = Mathf.Clamp(turretSprite.Rotation, -Mathf.DegToRad(angleLimit), Mathf.DegToRad(angleLimit));
    }

    public void Reset() {
        pathFollow.ProgressRatio = 0f;
        turretSprite.Rotation = 0f;
    }

}

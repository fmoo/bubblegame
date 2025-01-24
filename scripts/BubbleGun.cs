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

    public int Strafe { get; private set; } = 0;
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

    int GetStrafe() {
        if (ActiveMoveTarget.IsVisibleInTree()) {
            var TargetRatio = ActiveMoveTarget.ProgressRatio;
            var CurrentRatio = pathFollow.ProgressRatio;
            var diff = (TargetRatio - CurrentRatio + 1f) % 1f;
            if (Mathf.Abs(diff) < 0.005f) {
                ActiveMoveTarget.Visible = false;
                return 0;
            }
            if (diff < 0.5f) {
                return 1;
            } else {
                return -1;
            }
        }
        return Strafe;
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
        var strafe = GetStrafe();
        if (strafe < 0) {
            pathFollow.ProgressRatio = (pathFollow.ProgressRatio - (float)(delta * trackSpeed) + 1f) % 1f;
            speedScale += 1f;
        } else if (strafe > 0) {
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

    [Export] PathFollow2D ActiveMoveTarget;
    public void SetTrackDestination(Vector2 InputDirection) {
        var TargetRatio = (InputDirection.AngleTo(Vector2.Up) - Mathf.Pi) / Mathf.Tau + 1f;
        ActiveMoveTarget.ProgressRatio = TargetRatio;
        ActiveMoveTarget.Visible = true;
    }

    public void TurretLookAt(Vector2 GlobalMousePosition) {
        var RelativeTarget = turretSprite.GlobalPosition - GlobalMousePosition;
        turretSprite.GlobalRotation = RelativeTarget.Angle() - Mathf.Pi / 2;
        turretSprite.Rotation = Mathf.Clamp(turretSprite.Rotation, -Mathf.DegToRad(angleLimit), Mathf.DegToRad(angleLimit));
    }

    public void SetStrafe(int strafeValue) {
        Strafe = strafeValue;
        if (strafeValue != 0) {
            ActiveMoveTarget.Visible = false;
        }
    }

    public void SetTrackAngle(Vector2 InputDirection) {
        pathFollow.ProgressRatio = -InputDirection.AngleTo(Vector2.Up) / Mathf.Tau;
    }

    public void SetTurretAngle(Vector2 InputDirection) {
        turretSprite.GlobalRotation = InputDirection.AngleTo(Vector2.Down);
        turretSprite.Rotation = Mathf.Clamp(turretSprite.Rotation, -Mathf.DegToRad(angleLimit), Mathf.DegToRad(angleLimit));
    }

    public void Reset() {
        ActiveMoveTarget.Visible = false;
        pathFollow.ProgressRatio = 0f;
        turretSprite.Rotation = 0f;
    }

}

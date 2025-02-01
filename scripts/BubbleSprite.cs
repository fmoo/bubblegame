using Godot;
using System;

public partial class BubbleSprite : Sprite2D {
	[Export] AnimationPlayer AnimationPlayer;

	float blinkTimer = 1f;

	public override void _Ready() {
		base._Ready();
		ResetTimer();
	}

	void ResetTimer() {
		blinkTimer = (float)GD.RandRange(3, 8);
	}

	Vector2 globalPositionStart;
	float _targetRatio = 0f;
	Node2D target;
	public void SetTarget(Node2D target) {
		globalPositionStart = GlobalPosition;
		this.target = target;
		_targetRatio = 0f;
	}
	public float TargetRatio {
		get => _targetRatio;
		set {
			_targetRatio = value;
			GlobalPosition = globalPositionStart.Lerp(target.GlobalPosition, _targetRatio);
		}
	}


	public override void _Process(double delta) {
		base._Process(delta);
		blinkTimer -= (float)delta;
		if (blinkTimer <= 0) {
			ResetTimer();
			PlayAnimation("blink");
		}
	}

	public void PlayAnimation(string animation) {
		if (animation == "pop") blinkTimer = 999;
		AnimationPlayer.Play(animation);
	}

	public void SetConfig(BubbleConfig config) {
		Texture = config.PlayerBubbleTexture;
	}
}

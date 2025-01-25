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

using Godot;
using System;

public partial class ScaleTweenFollower : Sprite2D {
	[Export] Node2D Follow;
	[Export] float ScaleMultiplier = 0.25f;

	public override void _Process(double delta) {
		// Move the Scale of this sprite towards the Scale of the Follow node multiplied by ScaleMultiplier
		Scale = Scale.Lerp(Follow.Scale * ScaleMultiplier, 0.1f);
	}
}

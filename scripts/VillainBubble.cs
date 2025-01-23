using Godot;
using System;

public partial class VillainBubble : RigidBody2D {
	[Export] CollisionShape2D CollisionShape;
	[Export] float DefaultScale = 2.0f;
	[Export] float MinimumScale = 2.0f;
	[Export] float GameOverScale = 12f;
	[Export] float SizeChangeIncrement = 0.8f;

	public CircleShape2D CircleShape => (CircleShape2D)CollisionShape.Shape;
	
	public void Grow() {
		CollisionShape.Scale = new Vector2(CollisionShape.Scale.X + SizeChangeIncrement, CollisionShape.Scale.Y + SizeChangeIncrement);
		if (CollisionShape.Scale.X >= GameOverScale) {
			BubbleGame.Game.GameOver();
		}
	}

	public void Shrink() {
		if (CollisionShape.Scale.X <= MinimumScale) {
			return;
		}
		CollisionShape.Scale = new Vector2(CollisionShape.Scale.X - SizeChangeIncrement, CollisionShape.Scale.Y - SizeChangeIncrement);
	}


	public void Reset() {
		CollisionShape.Scale = new Vector2(DefaultScale, DefaultScale);
	}
}

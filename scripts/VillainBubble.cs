using Godot;
using System;
using System.Collections.Generic;

public partial class VillainBubble : RigidBody2D {
	[Export] CollisionShape2D CollisionShape;
	[Export] float DefaultScale = 2.0f;
	[Export] float MinimumScale = 2.0f;
	[Export] float GameOverScale = 12f;
	[Export] float SizeChangeIncrement = 0.8f;

	public CircleShape2D CircleShape => (CircleShape2D)CollisionShape.Shape;
	
	public void Grow() {
		// Temporarily re-enable collisions on the joints
		foreach (var joint in GetJoints()) {
			joint.QueueFree();
		}

		CollisionShape.Scale = new Vector2(CollisionShape.Scale.X + SizeChangeIncrement, CollisionShape.Scale.Y + SizeChangeIncrement);
		if (CollisionShape.Scale.X >= GameOverScale) {
			BubbleGame.Game.GameOver();
		} else {
			BubbleGame.Game.Audio.Grow();
		}
	}

	const float PopFallForce = 50f;
	public void Shrink() {
		if (CollisionShape.Scale.X <= MinimumScale) {
			return;
		}

		foreach (var bubbleNode in BubbleGame.Game.Bubbles.GetChildren()) {
			if (bubbleNode is Bubble bubble) {
				bubble.LinearVelocity = (GlobalPosition - bubble.GlobalPosition).Normalized() * PopFallForce / 2;
			}
		}

		foreach (var joint in GetJoints()) {
			// Make all bubbles move towards the vollain node
			try {
				var bubbleNode = joint.GetNode(joint.NodeB);
				if (bubbleNode is Bubble bubble && bubble != null) {
					bubble.LinearVelocity = (GlobalPosition - bubble.GlobalPosition).Normalized() * PopFallForce;
				}
			} catch (Exception e) {
				GD.PrintErr(e);
			}
			joint.QueueFree();
		}

		CollisionShape.Scale = new Vector2(CollisionShape.Scale.X - SizeChangeIncrement, CollisionShape.Scale.Y - SizeChangeIncrement);
		BubbleGame.Game.Audio.Shrink();
	}

	public IEnumerable<PinJoint2D> GetJoints() {
		foreach (Node child in GetChildren()) {
			if (child is PinJoint2D joint) {
				yield return joint;
			}
		}
	}

	public void Reset() {
		CollisionShape.Scale = new Vector2(DefaultScale, DefaultScale);
		// Delete all children that are Joints
		foreach (Node child in GetChildren()) {
			if (child is PinJoint2D joint) {
				joint.QueueFree();
			}
		}
	}
}

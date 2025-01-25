using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class VillainBubble : RigidBody2D {
	[Export] CollisionShape2D CollisionShape;
	[Export] Sprite2D Sprite;
	const float MINIMUM_SCALE = 1.0f;
	const float SIZE_CHANGE_INCREMENT = 0.5f;
	const float GAME_OVER_SCALE = 6.5f;

	public float ScoreMultiplier => CollisionShape.Scale.X / MINIMUM_SCALE;

	public CircleShape2D CircleShape => (CircleShape2D)CollisionShape.Shape;

	public void Grow() {
		// Temporarily re-enable collisions on the joints
		foreach (var joint in GetJoints()) {
			joint.QueueFree();
		}

		CollisionShape.Scale = new Vector2(CollisionShape.Scale.X + SIZE_CHANGE_INCREMENT, CollisionShape.Scale.Y + SIZE_CHANGE_INCREMENT);
		if (CollisionShape.Scale.X >= GAME_OVER_SCALE) {
			BubbleGame.Game.GameOver();
		} else {
			BubbleGame.Game.Audio.Grow();
		}
	}

	const float PopFallForce = 25f;
	public void Shrink() {
		if (CollisionShape.Scale.X <= MINIMUM_SCALE) {
			return;
		}

		foreach (var bubbleNode in BubbleGame.Game.Bubbles.GetChildren()) {
			if (bubbleNode is Bubble bubble) {
				var gravityMultiplier = bubble.IsAnchored ? 1f : 8f;
				bubble.LinearVelocity = (GlobalPosition - bubble.GlobalPosition).Normalized() * PopFallForce * gravityMultiplier;
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
				// GD.PrintErr(e);
			}
			joint.QueueFree();
		}

		CollisionShape.Scale = new Vector2(CollisionShape.Scale.X - SIZE_CHANGE_INCREMENT, CollisionShape.Scale.Y - SIZE_CHANGE_INCREMENT);
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
		CollisionShape.Scale = new Vector2(MINIMUM_SCALE, MINIMUM_SCALE);
		// Delete all children that are Joints
		foreach (Node child in GetChildren()) {
			if (child is PinJoint2D joint) {
				joint.QueueFree();
			}
		}
	}

	public void SetConfig(BubbleConfig config) {
		Sprite.Modulate = config.BubbleColor;
	}
}

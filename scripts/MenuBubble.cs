using Godot;
using System;
using System.Collections.Generic;

public partial class MenuBubble : RigidBody2D {
	[Export] CollisionShape2D CollisionShape;
	[Export] Sprite2D Sprite;
	[Export] public String GameFunction;
	const float MINIMUM_SCALE = 1.0f;
	const float SIZE_CHANGE_INCREMENT = 0.5f;
	const float GAME_OVER_SCALE = 6.5f;

	public float ScoreMultiplier => CollisionShape.Scale.X / MINIMUM_SCALE;

	public CircleShape2D CircleShape => (CircleShape2D)CollisionShape.Shape;
	public RectangleShape2D RectangleShape => (RectangleShape2D)CollisionShape.Shape;

	[Signal] public delegate void ContainRatioChangedEventHandler(double containRatio);

	public IEnumerable<PinJoint2D> GetJoints() {
		foreach (Node child in GetChildren()) {
			if (child is PinJoint2D joint) {
				yield return joint;
			}
		}
	}

	public void Reset() {
		var tween = SetConfig(null);
		tween.CustomStep(999f);
		CollisionShape.Scale = new Vector2(MINIMUM_SCALE, MINIMUM_SCALE);
		NotifyContainRatioChange();
		// Delete all children that are Joints
		foreach (Node child in GetChildren()) {
			if (child is PinJoint2D joint) {
				joint.QueueFree();
			}
		}
	}

	public Resource Config { get; private set; }
	public Tween SetConfig(Resource config) {
		var tween = GetTree().CreateTween();
		var bubbleColor = config != null ? config.Get("BubbleColor") : new Color(1, 1, 1);

		tween.TweenProperty(Sprite, "modulate", bubbleColor, 0.15f);
		Config = config;
		return tween;
	}

	void NotifyContainRatioChange() {
		EmitSignal(SignalName.ContainRatioChanged, GetContainRatio());
	}

	double GetContainRatio() {
		return (CollisionShape.Scale.X - MINIMUM_SCALE) / (GAME_OVER_SCALE - MINIMUM_SCALE);
	}
}

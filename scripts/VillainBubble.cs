using Godot;
using System;

public partial class VillainBubble : RigidBody2D {
	[Export] CollisionShape2D CollisionShape;

	public CircleShape2D CircleShape => (CircleShape2D)CollisionShape.Shape;


	public void Reset() {
		// TODO: Reset the scale/size, once growing that's implemented.
	}
}

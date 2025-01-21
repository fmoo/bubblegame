using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

public partial class Bubble : RigidBody2D {
	HashSet<Bubble> neighbors = new();
	Dictionary<Color, HashSet<Bubble>> colorNeighbors = new();

	public void SetNeighbor(Bubble bubble) {
		if (neighbors.Contains(bubble)) return;
		neighbors.Add(bubble);
		if (!colorNeighbors.ContainsKey(bubble.Modulate)) {
			colorNeighbors[bubble.Modulate] = new HashSet<Bubble>();
		}
		colorNeighbors[bubble.Modulate].Add(bubble);
	}

	void _on_body_entered(Node body) {
		if (body is Bubble bubble)  {
			bubble.LinearVelocity = Vector2.Zero;
			this.LinearVelocity = Vector2.Zero;

			if (neighbors.Contains(bubble)) return;
			this.SetNeighbor(bubble);
			bubble.SetNeighbor(this);

			BubbleGame.Game.LinkBubbles(this, bubble);

		} else if (body is VillainBubble villainBubble) {
			BubbleGame.Game.LinkToVillainBubble(villainBubble, this);

		}
	}

	void _on_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape) {
		if (body is VillainBubble villainBubble) {
		}
	}

}

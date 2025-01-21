using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

public partial class Bubble : RigidBody2D {
	[Export] CollisionShape2D CollisionShape;
	[Export] public Sprite2D Sprite { get; private set; }
	HashSet<Bubble> neighbors = new();
	Dictionary<Texture, HashSet<Bubble>> colorNeighbors = new();

	public HashSet<Bubble> WalkSameColorNeighbors() {
		var visited = new HashSet<Bubble>();
		var work = new Queue<Bubble>();
		work.Enqueue(this);
		while (work.Count > 0) {
			var bubble = work.Dequeue();
			if (visited.Contains(bubble)) continue;
			visited.Add(bubble);
			foreach (var neighbor in bubble.neighbors) {
				if (neighbor.Sprite.Texture == this.Sprite.Texture) {
					work.Enqueue(neighbor);
				}
			}
		}
		return visited;
	}

	public float Radius => ((CircleShape2D)CollisionShape.Shape).Radius;

	public void SetNeighbor(Bubble bubble) {
		if (neighbors.Contains(bubble)) return;
		neighbors.Add(bubble);
		if (!colorNeighbors.ContainsKey(bubble.Sprite.Texture)) {
			colorNeighbors[bubble.Sprite.Texture] = new HashSet<Bubble>();
		}
		colorNeighbors[bubble.Sprite.Texture].Add(bubble);
	}

	void _on_body_entered(Node body) {
		if (body is Bubble bubble)  {
			bubble.LinearVelocity = Vector2.Zero;
			this.LinearVelocity = Vector2.Zero;

			if (neighbors.Contains(bubble)) return;
			this.SetNeighbor(bubble);
			bubble.SetNeighbor(this);

			BubbleGame.Game.LinkBubbles(this, bubble);

			BubbleGame.Game.MaybePopBubbles(this);

		} else if (body is VillainBubble villainBubble) {
			BubbleGame.Game.LinkToVillainBubblePinJoint(villainBubble, this);

		}
	}

	void _on_body_shape_entered(int bodyId, Node body, int bodyShape, int areaShape) {
		if (body is VillainBubble villainBubble) {
		}
	}

    public override void _Process(double delta) {
        base._Process(delta);
		Sprite.GlobalRotation = 0f;
    }

	public void StartDestroy() {
		// Walk neighbors and remove yourself from their list
		foreach (var neighbor in neighbors) {
			neighbor.neighbors.Remove(this);
			neighbor.colorNeighbors[this.Sprite.Texture].Remove(this);
		}
		this.QueueFree();
	}

}

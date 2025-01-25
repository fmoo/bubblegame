using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

public partial class Bubble : RigidBody2D {
	[Export] CollisionShape2D CollisionShape;
	[Export] public BubbleSprite Sprite { get; private set; }
	public HashSet<Bubble> neighbors = new();
	Dictionary<Texture, HashSet<Bubble>> colorNeighbors = new();
	public bool HasVillainAnchor = false;

	public IEnumerable<Bubble> Neighbors => neighbors;
	public bool IsAnchored {
		get {
			if (HasVillainAnchor) return true;
			// breadth walk neighbors to check if any are anchored
			var visited = new HashSet<Bubble>();
			var work = new Queue<Bubble>();
			work.Enqueue(this);
			while (work.Count > 0) {
				var bubble = work.Dequeue();
				if (visited.Contains(bubble)) continue;
				visited.Add(bubble);
				if (bubble.HasVillainAnchor) return true;
				foreach (var neighbor in bubble.neighbors) {
					work.Enqueue(neighbor);
				}
			}
			return false;
		}
	}

	const float PopFallForce = 100f;
	public void ChainMoveTowards(Vector2 target) {
		var visited = new HashSet<Bubble>();
		var work = new Queue<Bubble>();
		work.Enqueue(this);
		while (work.Count > 0) {
			var bubble = work.Dequeue();
			if (visited.Contains(bubble)) continue;
			visited.Add(bubble);
			bubble.LinearVelocity = (target - bubble.GlobalPosition).Normalized() * PopFallForce;
			foreach (var neighbor in bubble.neighbors) {
				work.Enqueue(neighbor);
			}
		}
	}

	public HashSet<Bubble> WalkSameColorNeighbors() {
		var visited = new HashSet<Bubble>();
		var work = new Queue<Bubble>();
		work.Enqueue(this);
		while (work.Count > 0) {
			var bubble = work.Dequeue();
			if (visited.Contains(bubble)) continue;
			visited.Add(bubble);
			foreach (var neighbor in bubble.neighbors) {
				try {
					if (neighbor.Sprite.Texture == this.Sprite.Texture) {
						work.Enqueue(neighbor);
					}
				} catch (Exception e) {
					// GD.PrintErr(e);
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
		if (body is Bubble bubble) {
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
	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		Sprite.GlobalRotation = 0f;
	}

	public void StartDestroy() {
		// Disable collisions and set velocity to 0
		CollisionLayer = 0;
		CollisionMask = 0;
		CollisionShape = null;
		LinearVelocity = Vector2.Zero;

		BubbleGame.Game.Audio.Pop();
		// Walk neighbors and remove yourself from their list
		foreach (var neighbor in neighbors) {
			neighbor.neighbors.Remove(this);
			neighbor.colorNeighbors[this.Sprite.Texture].Remove(this);
		}
		QueueFree();
		BubbleGame.Game.SpawnBubblePop(GlobalPosition, Sprite.Texture);
		// Sprite.PlayAnimation("pop");

		// var tween = GetTree().CreateTween();
		// tween.TweenCallback(Callable.From(() => {
		// 	this.QueueFree();
		// })).SetDelay(1f);
	}

}

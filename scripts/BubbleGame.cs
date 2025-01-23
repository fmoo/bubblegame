using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BubbleGame : Node2D {
    [Export] public BubbleQueue BubbleQueue { get; private set; }
    [Export] public Texture2D[] bubbleColors;
    [Export] public Node2D Springs;
    [Export] public Node2D Bubbles;
    [Export] public VillainBubble VillainBubble { get; private set; }
    [Export] BubbleGun Player;
    [Export] public bool DebugMode { get; private set; } = false;

    [Export] PackedScene PinJointTemplate;

    BubbleGame() {
        Game = this;
    }

    public override void _Ready() {
        base._Ready();
    }

    public void RegisterBubble(Bubble bubble) {
        Bubbles.AddChild(bubble);
    }

    public void DestroyBubble(Bubble bubble) {
        bubble.StartDestroy();
    }

    public void RegisterLink(Bubble bubble, Node2D join) {
    }

    public PinJoint2D LinkBubbles(Bubble bubble1, Bubble bubble2) {
        var joint = PinJointTemplate.Instantiate<PinJoint2D>();
        bubble1.AddChild(joint);
        // Springs.AddChild(joint);
        joint.NodeA = bubble1.GetPath();
        joint.NodeB = bubble2.GetPath();
        joint.GlobalPosition = bubble1.GlobalPosition;
        // joint.GlobalPosition = (bubble1.GlobalPosition + bubble2.GlobalPosition) / 2;
        joint.GlobalRotation = bubble1.GlobalPosition.AngleToPoint(bubble2.GlobalPosition);
        return joint;
    }
    public RemoteTransform2D LinkToVillainBubbleRemoteTransform(VillainBubble villainBubble, Bubble bubble2) {
        GD.Print($"Villain Bubble at {villainBubble.GlobalPosition} linked to Bubble at {bubble2.GlobalPosition}");

        // Get the point of intersection by moving from bubble2 towards bubble1 by bubble2.Radius
        var direction = (villainBubble.GlobalPosition - bubble2.GlobalPosition).Normalized();
        var intersection = bubble2.GlobalPosition + direction * bubble2.Radius;

        // Create the sync point for the bubble at the intersectionPoint,
        // making it a child of the villainBubble.
        var joint = new RemoteTransform2D();
        villainBubble.AddChild(joint);
        joint.GlobalPosition = bubble2.GlobalPosition;
        joint.GlobalRotation = bubble2.GlobalRotation;
        joint.UseGlobalCoordinates = true;
        joint.UpdateRotation = false;
        joint.UpdateScale = false;
        bubble2.SetDeferred("freeze", true);

        joint.RemotePath = bubble2.GetPath();

        RegisterLink(bubble2, joint);
        return joint;
    }

    public PinJoint2D LinkToVillainBubblePinJoint(VillainBubble villainBubble, Bubble bubble) {
        GD.Print($"Villain Bubble at {villainBubble.GlobalPosition} linked to Bubble at {bubble.GlobalPosition}");
        bubble.HasVillainAnchor = true;

        var joint = PinJointTemplate.Instantiate<PinJoint2D>();
        villainBubble.AddChild(joint);
        joint.NodeA = villainBubble.GetPath();
        joint.NodeB = bubble.GetPath();
        joint.GlobalPosition = villainBubble.GlobalPosition;
        joint.GlobalRotation = villainBubble.GlobalPosition.AngleToPoint(bubble.GlobalPosition);

        RegisterLink(bubble, joint);
        return joint;
    }

    public Texture2D PickColor() {
        var index = GD.RandRange(0, bubbleColors.Length - 1);
        GD.Print(index);
        return bubbleColors[index];
    }

    public void MaybePopBubbles(Bubble bubble) {
        var maybePop = bubble.WalkSameColorNeighbors();
        if (maybePop.Count < 3) return;
        HashSet<Bubble> maybeFall = new();
        foreach (var p in maybePop) {
            foreach (var f in p.Neighbors) {
                if (!maybePop.Contains(f)) {
                    maybeFall.Add(f);
                }
            }
            DestroyBubble(p);
        }

        foreach (var f in maybeFall) {
            if (f.IsAnchored) continue;
            f.ChainMoveTowards(VillainBubble.GlobalPosition);
        }
    }

    public void GameOver() {
        GD.Print("Game Over");
        Reset();
    }

    public void Reset() {
        numShots = 0;
        foreach (var bubble in Bubbles.GetChildren()) {
            bubble.QueueFree();
        }
        foreach (var spring in Springs.GetChildren()) {
            spring.QueueFree();
        }
        VillainBubble.Reset();
        Player.Reset();
        BubbleQueue.Reset();
    }

    int numShots = 0;
    public void _on_shoot_event() {
        numShots++;
        if (numShots == 5) {
            numShots = 0;
            VillainBubble.Grow();
        }
    }

    public static BubbleGame Game { get; private set; }
}

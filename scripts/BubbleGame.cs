using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BubbleGame : Node2D {
    [Export] public BubbleQueue BubbleQueue { get; private set; }
    [Export] public Texture2D[] bubbleColors;
    [Export] public Node2D Springs;
    [Export] public Node2D Bubbles;
    [Export] VillainBubble  VillainBubble;
    [Export] BubbleGun Player;
    public Dictionary<Bubble, HashSet<Node2D>> bubbleLinks = new();
    public Dictionary<Node2D, HashSet<Bubble>> springLinks = new();
    

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
        if (bubbleLinks.ContainsKey(bubble)) {
            foreach (var link in bubbleLinks[bubble]) {
                var otherNode = springLinks[link].Except(new[] { bubble }).FirstOrDefault();
                springLinks.Remove(link);
                if (otherNode != null) {
                    bubbleLinks[otherNode].Remove(link);
                }
                link.QueueFree();
            }
            bubbleLinks.Remove(bubble);
        }
        bubble.StartDestroy();
    }

    public void RegisterLink(Bubble bubble, Node2D join) {
        if (!bubbleLinks.ContainsKey(bubble)) {
            bubbleLinks[bubble] = new HashSet<Node2D>();
        }
        bubbleLinks[bubble].Add(join);
        if (!springLinks.ContainsKey(join)) {
            springLinks[join] = new HashSet<Bubble>();
        }
        springLinks[join].Add(bubble);
    }

    public PinJoint2D LinkBubbles(Bubble bubble1, Bubble bubble2) {
        var joint = PinJointTemplate.Instantiate<PinJoint2D>();
        bubble1.AddChild(joint);
        joint.NodeA = bubble1.GetPath();
        joint.NodeB = bubble2.GetPath();
        joint.GlobalPosition = bubble1.GlobalPosition;
        // joint.GlobalPosition = (bubble1.GlobalPosition + bubble2.GlobalPosition) / 2;
        joint.GlobalRotation = bubble1.GlobalPosition.AngleToPoint(bubble2.GlobalPosition);
        RegisterLink(bubble1, joint);
        RegisterLink(bubble2, joint);
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
        var bubbles = bubble.WalkSameColorNeighbors();
        if (bubbles.Count >= 3) {
            foreach (var b in bubbles) {
                DestroyBubble(b);
            }
        }
    }

    public void Reset() {
        foreach (var bubble in Bubbles.GetChildren()) {
            bubble.QueueFree();
        }
        foreach (var spring in Springs.GetChildren()) {
            spring.QueueFree();
        }
        bubbleLinks.Clear();
        springLinks.Clear();
        VillainBubble.Reset();
        Player.Reset();
    }

    public static BubbleGame Game { get; private set; }
}

using Godot;
using System;
using System.Collections.Generic;

public partial class BubbleGame : Node2D {
    [Export] public BubbleQueue BubbleQueue { get; private set; }
    [Export] public Texture2D[] bubbleColors;
    [Export] public Node2D Springs;
    [Export] public Node2D Bubbles;

    [Export] PackedScene PinJointTemplate;

    BubbleGame() {
        Game = this;
    }

    public override void _Ready() {
        base._Ready();
        // Set screen scale to 2x
        var window = GetWindow();
        window.Size *= 2;

    }

    public void RegisterBubble(Bubble bubble) {
        Bubbles.AddChild(bubble);
    }

    public void DestroyBubble(Bubble bubble) {
        bubble.QueueFree();
    }

    public PinJoint2D LinkBubbles(Bubble bubble1, Bubble bubble2) {
        var joint = PinJointTemplate.Instantiate<PinJoint2D>();
        bubble1.AddChild(joint);
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
        return joint;
    }

    public Texture2D PickColor() {
        var index = GD.RandRange(0, bubbleColors.Length - 1);
        GD.Print(index);
        return bubbleColors[index];
    }

    public static BubbleGame Game { get; private set; }
}

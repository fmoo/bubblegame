using Godot;
using System;
using System.Collections.Generic;

public partial class BubbleGame : Node2D {
    [Export] public BubbleQueue BubbleQueue { get; private set; }
    [Export] public Color[] bubbleColors;
    public Color CurrentColor;
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

        CurrentColor = bubbleColors[0];
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
        joint.GlobalPosition = (bubble1.GlobalPosition + bubble2.GlobalPosition) / 2;
        joint.GlobalRotation = bubble1.GlobalPosition.AngleToPoint(bubble2.GlobalPosition);
        return joint;
    }
    public RemoteTransform2D LinkToVillainBubble(VillainBubble bubble1, Bubble bubble2) {
        GD.Print($"Villain Bubble at {bubble1.GlobalPosition} linked to Bubble at {bubble2.GlobalPosition}");
        var joint = new RemoteTransform2D();
        bubble1.AddChild(joint);
        joint.GlobalPosition = bubble2.GlobalPosition;
        GD.Print($"Joint set to {joint.GlobalPosition}");
        joint.GlobalRotation = bubble2.GlobalRotation;
        joint.UseGlobalCoordinates = true;
        joint.UpdateRotation = false;
        joint.UpdateScale = false;
        bubble2.SetDeferred("freeze", true);

        joint.RemotePath = bubble2.GetPath();
        return joint;
    }

    public Color PickColor() {
        var index = GD.RandRange(0, bubbleColors.Length - 1);
        GD.Print(index);
        return bubbleColors[index];
    }

    public static BubbleGame Game { get; private set; }
}

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

    public void LinkBubbles(Bubble bubble1, Bubble bubble2) {
        var spring = PinJointTemplate.Instantiate<PinJoint2D>();
        bubble1.AddChild(spring);
        spring.NodeA = bubble1.GetPath();
        spring.NodeB = bubble2.GetPath();
        spring.GlobalPosition = bubble1.GlobalPosition;
        spring.GlobalPosition = (bubble1.GlobalPosition + bubble2.GlobalPosition) / 2;
        spring.GlobalRotation = bubble1.GlobalPosition.AngleToPoint(bubble2.GlobalPosition);
    }

    public Color PickColor() {
        var index = GD.RandRange(0, bubbleColors.Length - 1);
        GD.Print(index);
        return bubbleColors[index];
    }

    public static BubbleGame Game { get; private set; }
}

using System.Collections.Generic;
using Godot;

public partial class BubbleQueue : Node2D {
    [Export] public BubbleSprite[] bubbleRenders;
    List<BubbleConfig> colorQueue = new();
    public override void _Ready() {
        base._Ready();
        Reset();
    }

    public BubbleConfig DequeueColor() {
        var result = colorQueue[0];
        colorQueue.RemoveAt(0);
        colorQueue.Add(BubbleGame.Game.PickColor());
        RefreshRender();
        return result; 
    }

    void RefreshRender() {
        for (int i = 0; i < bubbleRenders.Length; i++) {
            bubbleRenders[i].SetConfig(colorQueue[i]);
        }
    }

    public void Reset() {
        colorQueue.Clear();
        for (int i = 0; i < bubbleRenders.Length; i++) {
            var color = BubbleGame.Game.PickColor();
            colorQueue.Add(color);
        }
        RefreshRender();
    }
}
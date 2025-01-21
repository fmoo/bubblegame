using System.Collections.Generic;
using Godot;

public partial class BubbleQueue : Node2D {
    [Export] public Sprite2D[] bubbleRenders;
    List<Texture2D> colorQueue = new();
    public override void _Ready() {
        base._Ready();

        // Fill colorQueue with random colors from Game.bubbleColors
        for (int i = 0; i < bubbleRenders.Length; i++) {
            var color = BubbleGame.Game.PickColor();
            colorQueue.Add(color);
        }
        RefreshRender();
    }

    public Texture2D DequeueColor() {
        var result = colorQueue[0];
        colorQueue.RemoveAt(0);
        colorQueue.Add(BubbleGame.Game.PickColor());
        RefreshRender();
        return result; 
    }

    void RefreshRender() {
        for (int i = 0; i < bubbleRenders.Length; i++) {
            bubbleRenders[i].Texture = colorQueue[i];
        }
    }


}
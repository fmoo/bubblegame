using Godot;
using System;

public partial class DebugMode : ControlSchemeBase {
    public override void _Ready() {
        base._Ready();
        if (!EngineDebugger.IsActive()) {
            QueueFree();
        }
    }

    public override void _Process(double delta) {
        base._Process(delta);
        if (Input.IsActionJustPressed("debug_reset")) {
            GD.Print("DEBUG: Resetting game");
            BubbleGame.Game.Reset();
        }
        if (Input.IsActionJustPressed("debug_growbubble")) {
            GD.Print("DEBUG: Growing bubble");
            BubbleGame.Game.VillainBubble.Grow();
        }
        if (Input.IsActionJustPressed("debug_shrinkbubble")) {
            GD.Print("DEBUG: Shrinking bubble");
            BubbleGame.Game.VillainBubble.Shrink();
        }
    }
}

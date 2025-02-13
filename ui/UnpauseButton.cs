using System;
using Godot;

public partial class UnpauseButton : Button {
    public override void _Ready() {
        base._Ready();
        Pressed += () => {
            GD.Print("Unpausing!!");
            BubbleGame.Game.Unpause();
        };
    }
}

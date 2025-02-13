using System;
using Godot;

public partial class ConfigButton : Button {
    [Export] Control ConfigPanel;
    public override void _Ready() {
        base._Ready();
        this.Pressed += () => BubbleGame.Game.PauseWithPanel(ConfigPanel);
    }
}

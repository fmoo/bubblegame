using System;
using Godot;

public partial class PauseGame : ControlSchemeBase {
    [Export] Control PausePanel;

    public override void _Process(double delta) {
        base._Process(delta);
        if (Input.IsActionJustPressed("toggle_pause")) {
            if (!GetTree().Paused) {
                if (PausePanel != null) {
                    BubbleGame.Game.PauseWithPanel(PausePanel);
                }
            } else {
                BubbleGame.Game.Unpause();
            }
        }
    }
}

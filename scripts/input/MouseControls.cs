using Godot;
using System;

public partial class MouseControls : ControlSchemeBase {
    [Export] PathFollow2D moveTargetPath;

    public override void _Input(InputEvent @event) {
        base._Input(@event);
        // if (@event is InputEventMouseButton mouseButton) {
        //     if (mouseButton.ButtonIndex == (int)ButtonList.Left && mouseButton.Pressed) {
        //         if (BubbleGame.Game.GameOver) {
        //             BubbleGame.Game.Restart();
        //         } else {
        //             BubbleGame.Game.VillainBubble.Grow();
        //         }
        //     } else if (mouseButton.ButtonIndex == (int)ButtonList.Right && mouseButton.Pressed) {
        //         BubbleGame.Game.VillainBubble.Shrink();
        //     }
        // }
    }

}

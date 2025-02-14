using Godot;


public partial class TwinstickInstantControls : ControlSchemeBase {
    public override void _Process(double delta) {
        base._Process(delta);

        Vector2 leftStick = new Vector2(
            Input.GetActionStrength("twinstick1_right") - Input.GetActionStrength("twinstick1_left"),
            Input.GetActionStrength("twinstick1_up") - Input.GetActionStrength("twinstick1_down")
        );
        Vector2 rightStick = new Vector2(
            Input.GetActionStrength("twinstick2_right") - Input.GetActionStrength("twinstick2_left"),
            Input.GetActionStrength("twinstick2_up") - Input.GetActionStrength("twinstick2_down")
        );
        BubbleGame.Game.Player.Call("SetTrackAngle", leftStick);
        BubbleGame.Game.Player.Call("SetTurretAngle", rightStick);

        if (Input.IsActionJustPressed("twinstick_shoot")) {
            BubbleGame.Game.Player.Call("Shoot");
        }
    }
}
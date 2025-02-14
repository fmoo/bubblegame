class_name TwinStickInstantControls
extends ControlSchemeBase

@onready var bubbleGame = get_node("/root/BubbleGame")

func _process(_delta: float) -> void:
    var leftStick = Vector2(
        Input.get_action_strength("twinstick1_right") - Input.get_action_strength("twinstick1_left"),
        Input.get_action_strength("twinstick1_up") - Input.get_action_strength("twinstick1_down")
    )
    var rightStick = Vector2(
        Input.get_action_strength("twinstick2_right") - Input.get_action_strength("twinstick2_left"),
        Input.get_action_strength("twinstick2_up") - Input.get_action_strength("twinstick2_down")
    )
    bubbleGame.Player.SetTrackAngle(leftStick)
    bubbleGame.Player.SetTurretAngle(rightStick)

    if Input.is_action_just_pressed("twinstick_shoot"):
        bubbleGame.Player.Shoot()
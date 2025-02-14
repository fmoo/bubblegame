class_name SimpleShoot
extends ControlSchemeBase

@onready var bubbleGame = get_node("/root/BubbleGame")

func _process(_delta: float) -> void:
    if Input.is_action_just_pressed("ui_select"):
        bubbleGame.Player.Shoot()

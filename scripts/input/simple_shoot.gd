class_name SimpleShoot
extends "res://scripts/input/ControlSchemeBase.gd"

@onready var bubbleGame: BubbleGame = get_node("/root/BubbleGame")

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("ui_select"):
		bubbleGame.Player.Shoot()

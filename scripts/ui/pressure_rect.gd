class_name PressureRect
extends TextureProgressBar

@onready var bubbleGame = get_node("/root/BubbleGame")

func _process(_delta: float) -> void:
    value = bubbleGame.Pressure
class_name PressureRect
extends TextureProgressBar

@onready var bubbleGame: BubbleGame = get_node("/root/BubbleGame")

func _process(_delta: float) -> void:
    value = bubbleGame.Pressure
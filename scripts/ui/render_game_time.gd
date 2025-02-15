class_name RenderGameTime
extends Label

@onready var bubbleGame: BubbleGame = get_node("/root/BubbleGame")

func _ready() -> void:
    bubbleGame.TimeElapsed.connect(_on_time_elapsed)

func _on_time_elapsed(seconds: int) -> void:
    var colon: String = ":"
    text = str(seconds / 60) + colon + str(seconds % 60).pad_zeros(2)

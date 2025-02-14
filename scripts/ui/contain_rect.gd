class_name ContainRect
extends TextureProgressBar

@onready var bubbleGame = get_node("/root/BubbleGame")
var wantValue: float = 0

func _on_contain_ratio_changed(containRatio: float) -> void:
    wantValue = containRatio

func _ready() -> void:
    bubbleGame.VillainBubble.ContainRatioChanged.connect(_on_contain_ratio_changed)

func _process(_delta: float) -> void:
    if wantValue == 0:
        value = 0
        return

    value = lerp(value, wantValue, 0.1)
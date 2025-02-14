class_name ChainRender
extends TextureProgressBar

@onready var bubbleGame = get_node("/root/BubbleGame")
@export var ChainLabel: Label

func _ready() -> void:
    bubbleGame.ChainChanged.connect(_on_chain_changed)
    min_value = 0
    max_value = bubbleGame.GameplayConfig.ChainDuration

func _process(_delta: float) -> void:
    value = bubbleGame.ChainTimeRemaining

func _on_chain_changed(chain: int) -> void:
    print("Chain changed to " + str(chain))
    ChainLabel.text = str(chain) + "x"

class_name BubbleSprite
extends Sprite2D

@export var animation_player: AnimationPlayer

var blinkTimer: float = 1.0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ResetTimer()

func ResetTimer() -> void:
	blinkTimer = randf_range(3, 8)

func _process(delta: float):
	blinkTimer -= delta
	if (blinkTimer <= 0):
		ResetTimer()
		PlayAnimation("blink")

func PlayAnimation(animation: String) -> void:
	if (animation == "pop"):
		blinkTimer = 999
	animation_player.play(animation)

func set_config(config: BubbleConfig) -> void:
	texture = config.PlayerBubbleTexture

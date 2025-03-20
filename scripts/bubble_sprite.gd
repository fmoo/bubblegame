
class_name BubbleSprite
extends Sprite2D

@export var animation_player: AnimationPlayer

var blinkTimer: float = 1.0

var move_towards_gun: bool
var start_offset
var load_offset
var offset_progress

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ResetTimer()
	if($Trigger != null):
		start_offset = Vector2(0,0)
		load_offset = $Trigger.global_position - global_position
		offset_progress = 0.0
		
func ResetTimer() -> void:
	blinkTimer = randf_range(3, 8)

func _process(delta: float):
	blinkTimer -= delta
	if (blinkTimer <= 0):
		ResetTimer()
		PlayAnimation("blink")
	if(move_towards_gun):
		offset_progress += delta
		offset = lerp(start_offset,load_offset,clamp(offset_progress,0,1))
		if(offset_progress >= 1.0):
			var pos = get_index()
			get_parent().swap_in_doubleball(pos)
			get_parent().stop_doubleball_move(pos)
			
func PlayAnimation(animation: String) -> void:
	if (animation == "pop"):
		blinkTimer = 999
	animation_player.play(animation)

func set_config(config: BubbleConfig) -> void:
	if(config == null):
		texture = null
		return
	texture = config.PlayerBubbleTexture

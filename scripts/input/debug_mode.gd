class_name DebugMode
extends ControlSchemeBase

@onready var bubbleGame = get_node("/root/BubbleGame")

func _ready() -> void:
	if not EngineDebugger.is_active():
		queue_free()

func _process(_delta: float) -> void:
	if Input.is_action_just_released("debug_physics"):
		bubbleGame.Audio.BGM.PlayRandom()
	
	if Input.is_action_just_pressed("debug_reset"):
		# bubbleGame.Reset()
		pass
	
	if Input.is_action_just_pressed("debug_growbubble"):
		bubbleGame.VillainBubble.Grow()
	
	if Input.is_action_just_pressed("debug_shrinkbubble"):
		bubbleGame.VillainBubble.Shrink()
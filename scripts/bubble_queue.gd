class_name BubbleQueue
extends Node2D

@onready var bubbleGame: BubbleGame = get_node("/root/BubbleGame")
@export var bubbleRenders: Array[BubbleSprite]
@export var queueEjector: Node2D
@export var reloadPath: PathFollow2D
@export var gunPath: PathFollow2D
@export var ejectorPathRatio: float = 0.0995
@export var is_double_queue: bool
@export var empty_bubble_config: BubbleConfig

var colorQueue: Array[BubbleConfig] = []

const COOLDOWN: float = 0.3

func _ready() -> void:
	bubbleGame.ChainTimerOut.connect(on_chain_timer_out)
	if(!is_double_queue):
		reset()
	else:
		RefreshRender()
		
func ShowBubbles() -> void:
	for bubble in bubbleRenders:
		bubble.show()

func HideBubbles() -> void:
	for bubble in bubbleRenders:
		bubble.hide()

func StartTween() -> void:
	var tween = get_tree().create_tween().set_parallel(true)
	var clones: Array[BubbleSprite] = []
	for i in range(1, bubbleRenders.size()):
		var bubbleClone = bubbleRenders[i].duplicate() as BubbleSprite
		clones.append(bubbleClone)
		add_child(bubbleClone)
		bubbleClone.global_position = bubbleRenders[i].global_position
		if i == 1:
			var tween2 = get_tree().create_tween()
			tween2.tween_property(bubbleClone, "global_position", queueEjector.global_position, COOLDOWN / 4.0)
			tween2.tween_method(func (value: float) -> void:
				var targetRatio = gunPath.progress_ratio
				if targetRatio > ejectorPathRatio + 0.5:
					targetRatio -= 1.0
				reloadPath.progress_ratio = lerp(ejectorPathRatio, targetRatio, value)
				bubbleClone.global_position = reloadPath.global_position
			, 0.0, 1.0, COOLDOWN * 3.0 / 4.0)
		else:
			tween.tween_property(bubbleClone, "global_position", bubbleRenders[i - 1].global_position, COOLDOWN)
	
	HideBubbles()
	tween.chain().tween_callback(func () -> void:
		RefreshRender()
		ShowBubbles()
		for clone in clones:
			remove_child(clone)
			clone.queue_free()
	)

func DequeueColor() -> BubbleConfig:
	print("DequeueColor: BubbleQueue has ", colorQueue.size(), " colors")
	for c in colorQueue:
		print(c.BubbleColor)

	var result = colorQueue[0]
	colorQueue.remove_at(0)
	colorQueue.append(bubbleGame.PickColor())
	StartTween()
	return result

func add_random_double():
	colorQueue.append(bubbleGame.pick_random_double())
	RefreshRender()

func swap_config(new_config, position):
	colorQueue[position] = new_config
	RefreshRender()
	
func RefreshRender() -> void:
	for i in range(bubbleRenders.size()):
		if(colorQueue.size() > i):
			bubbleRenders[i].set_config(colorQueue[i])
		else:
			bubbleRenders[i].set_config(empty_bubble_config)

func reset() -> void:
	colorQueue.clear()
	if bubbleGame.TitleMode:
		for i in range(bubbleRenders.size()):
			var color = bubbleGame.gameplayConfig.Bubbles[0]
			colorQueue.append(color)
	else:
		for i in range(bubbleRenders.size()):
			var color = bubbleGame.PickColor()
			colorQueue.append(color)
	RefreshRender()

	print("reset: BubbleQueue has ", colorQueue.size(), " colors")
	for c in colorQueue:
		print(c.BubbleColor)

func on_chain_timer_out(chain: int):
	if(chain < 2):
		return
	if(is_double_queue && colorQueue.size() < 3):
		add_random_double()


func _on_doubleball_load_button_up() -> void:
	if(colorQueue.size() > 0):
		bubbleGame.swap_for_double_ball(colorQueue[0])
		if(colorQueue.size() == 1):
			colorQueue.remove_at(0)
		else:
			for i in range(0,colorQueue.size()):
				if(i+1 < colorQueue.size()):
					colorQueue[i] = colorQueue[i+1]
			colorQueue.remove_at(colorQueue.size()-1)
		RefreshRender()
			


func _on_doubleball_load_button_mouse_entered() -> void:
	bubbleGame.double_load_hovered = true

func _on_doubleball_load_button_mouse_exited() -> void:
	bubbleGame.double_load_hovered = false

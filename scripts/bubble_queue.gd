class_name BubbleQueue
extends Node2D

@onready var bubbleGame: BubbleGame = get_node("/root/BubbleGame")
@export var bubbleRenders: Array[BubbleSprite]
@export var queueEjector: Node2D
@export var reloadPath: PathFollow2D
@export var gunPath: PathFollow2D
@export var ejectorPathRatio: float = 0.0995

var colorQueue: Array[BubbleConfig] = []

const COOLDOWN: float = 0.3

func _ready() -> void:
	reset()

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

func RefreshRender() -> void:
	for i in range(bubbleRenders.size()):
		bubbleRenders[i].set_config(colorQueue[i])

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

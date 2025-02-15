class_name VillainBubble
extends RigidBody2D
@onready var bubbleGame: BubbleGame = get_node("/root/BubbleGame")
@export var collision_shape: CollisionShape2D
@export var sprite: Sprite2D

const MINIMUM_SCALE: float = 1.0
const SIZE_CHANGE_INCREMENT: float = 0.5
const GAME_OVER_SCALE: float = 6.5
const POP_FALL_FORCE: float = 25.0

signal contain_ratio_changed(contain_ratio: float)

func _ready() -> void:
	notify_contain_ratio_change()

func get_score_multiplier() -> float:
	return collision_shape.scale.x / MINIMUM_SCALE

func get_circle_shape() -> CircleShape2D:
	return collision_shape.shape as CircleShape2D

func grow() -> void:
	# Temporarily re-enable collisions on the joints
	for joint in get_joints():
		joint.queue_free()

	collision_shape.scale += Vector2(SIZE_CHANGE_INCREMENT, SIZE_CHANGE_INCREMENT)
	notify_contain_ratio_change()

	if collision_shape.scale.x >= GAME_OVER_SCALE:
		bubbleGame.GameOver()
	else:
		bubbleGame.audio.Grow()

func shrink() -> void:
	if collision_shape.scale.x <= MINIMUM_SCALE:
		return

	for bubble_node in bubbleGame.Bubbles.get_children():
		if bubble_node is Bubble:
			var gravity_multiplier = 1.0 if bubble_node.is_anchored() else 8.0
			bubble_node.linear_velocity = (global_position - bubble_node.global_position).normalized() * POP_FALL_FORCE * gravity_multiplier

	for joint in get_joints():
		# Make all bubbles move towards the villain node
		if joint.has_node(joint.node_b):
			var bubble_node = joint.get_node(joint.node_b)
			if bubble_node is Bubble:
				bubble_node.linear_velocity = (global_position - bubble_node.global_position).normalized() * POP_FALL_FORCE
		joint.queue_free()

	collision_shape.scale -= Vector2(SIZE_CHANGE_INCREMENT, SIZE_CHANGE_INCREMENT)
	notify_contain_ratio_change()
	bubbleGame.audio.Shrink()

func get_joints() -> Array:
	var joints: Array = []
	for child in get_children():
		if child is PinJoint2D:
			joints.append(child)
	return joints

func reset() -> void:
	var tween = set_config(null)
	tween.custom_step(999.0)
	collision_shape.scale = Vector2(MINIMUM_SCALE, MINIMUM_SCALE)
	notify_contain_ratio_change()

	# Delete all children that are joints
	for child in get_children():
		if child is PinJoint2D:
			child.queue_free()

var config: BubbleConfig

func set_config(new_config: BubbleConfig) -> Tween:
	var tween = get_tree().create_tween()
	var bubble_color = new_config.BubbleColor if new_config else Color(1, 1, 1)

	tween.tween_property(sprite, "modulate", bubble_color, 0.15)
	config = new_config
	return tween

func notify_contain_ratio_change() -> void:
	contain_ratio_changed.emit(get_contain_ratio())

func get_contain_ratio() -> float:
	return (collision_shape.scale.x - MINIMUM_SCALE) / (GAME_OVER_SCALE - MINIMUM_SCALE)

class_name MenuBubble
extends RigidBody2D

@export var collision_shape: CollisionShape2D
@export var sprite: Sprite2D
@export var game_function: String

const MINIMUM_SCALE: float = 1.0
const SIZE_CHANGE_INCREMENT: float = 0.5
const GAME_OVER_SCALE: float = 6.5

signal contain_ratio_changed(contain_ratio: float)

func get_score_multiplier() -> float:
	return collision_shape.scale.x / MINIMUM_SCALE

func get_circle_shape() -> CircleShape2D:
	return collision_shape.shape as CircleShape2D

func get_rectangle_shape() -> RectangleShape2D:
	return collision_shape.shape as RectangleShape2D

func get_joints() -> Array:
	var joints: Array = []
	for child in get_children():
		if child is PinJoint2D:
			joints.append(child)
	return joints

func reset() -> void:
	var tween = set_config(null)
	tween.custom_step(999.0)
	#collision_shape.scale = Vector2(MINIMUM_SCALE, MINIMUM_SCALE)
	#notify_contain_ratio_change()

	# Delete all children that are joints
	for child in get_children():
		if child is PinJoint2D:
			child.queue_free()

func set_collision_disabled(state):
	$CollisionShape2D.disabled = state

func set_collision(state):
	set_collision_layer_value(1,state)
	set_collision_mask_value(1,state)
	
var config: BubbleConfig

func set_config(new_config: BubbleConfig) -> Tween:
	var tween = get_tree().create_tween()
	var bubble_color = new_config.BubbleColor if new_config else Color(1, 1, 1)

	tween.tween_property(sprite, "modulate", bubble_color, 0.15)
	config = new_config
	return tween

func pop_in():
	var tween = get_tree().create_tween()
	tween.tween_property($Border, "scale", Vector2(1,1), 1.0).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_ELASTIC)
	var tween2 = get_tree().create_tween()
	tween2.tween_property($Highlight, "scale", Vector2(1,1), 1.0).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_ELASTIC)
	
func pop_out():
	var tween = get_tree().create_tween()
	tween.tween_property($Border, "scale", Vector2(0,0), 1.0).set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_ELASTIC)
	var tween2 = get_tree().create_tween()
	tween2.tween_property($Highlight, "scale", Vector2(0,0), 1.0).set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_ELASTIC)

func notify_contain_ratio_change() -> void:
	contain_ratio_changed.emit(get_contain_ratio())

func get_contain_ratio() -> float:
	return (collision_shape.scale.x - MINIMUM_SCALE) / (GAME_OVER_SCALE - MINIMUM_SCALE)

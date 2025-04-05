extends Node2D

@export var circle_scene : PackedScene
@export var circ_colors: Array[Color]
var screen_size

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	screen_size = get_viewport_rect().size


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_timer_timeout() -> void:
	var circ = circle_scene.instantiate()
	
	
	var color = circ_colors[randi_range(0,circ_colors.size()-1)]
	circ.get_node("AnimationPlayer").get_animation("grow").track_set_key_value(1,0,Color(color.r,color.g,color.b))
	circ.get_node("AnimationPlayer").get_animation("grow").track_set_key_value(1,1,Color(color.r,color.g,color.b))
	var color_transparent = Color(color.r,color.g,color.b, 0.0)
	circ.get_node("AnimationPlayer").get_animation("grow").track_set_key_value(2,2,color_transparent)
	print("color 1:", circ.get_node("AnimationPlayer").get_animation("grow").track_get_key_value(1,0))
	print("color 2:", circ.get_node("AnimationPlayer").get_animation("grow").track_get_key_value(1,1))
	#circ.set_modulate(color)
	circ.global_position = Vector2(randi_range(100,screen_size.x-100),randi_range(0,screen_size.y))
	var end_scale_val = randf_range(.5,1.5)
	var end_scale = Vector2(end_scale_val,end_scale_val)
	circ.get_node("AnimationPlayer").get_animation("grow").track_set_key_value(0,1,end_scale)
	circ.scale = Vector2(0.0,0.0)
	
	add_child(circ)
	circ.get_node("AnimationPlayer").play("grow")
	
	$Timer.wait_time = randf_range(.5,1.0)
	#$Timer.start()

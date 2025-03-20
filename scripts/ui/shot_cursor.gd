extends Node2D

@export var cursor_line_scene: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
	
func instance_line(y_pos:float):
	var line = cursor_line_scene.instantiate()
	line.position = Vector2(0,-1.0*y_pos)
	add_child(line)
	await get_tree().create_timer(.7).timeout
	line.queue_free()

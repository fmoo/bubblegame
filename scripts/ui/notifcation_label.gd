extends Label

var tween_dir = Vector2(-1,0)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

func move_in(endpos):
	var tween = get_tree().create_tween()
	tween.tween_property(self, "position", endpos, 1.0).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_BACK)
	tween.tween_callback(end_of_tween)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func end_of_tween():
	var tween = get_tree().create_tween()
	tween.tween_property(self, "modulate", Color(1,1,1,0), 1.0).set_ease(Tween.EASE_IN_OUT)
	await get_tree().create_timer(1.0).timeout
	queue_free()

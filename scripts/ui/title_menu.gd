extends Node2D

@onready var bubble_game: BubbleGame = get_node("/root/BubbleGame")

@export var button_texts: Node
@export var menu_bubble: Node2D
@export var bubbles_groups: Node


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func show_menu_button(pos):
	if(bubble_game.game_state != "Title"):
		return
	menu_bubble.pop_in()
	menu_bubble.set_collision(true)
	#await get_tree().create_timer(1.0).timeout
	button_texts.get_child(pos).visible = true
	var bubble_group = bubbles_groups.get_child(pos)
	bubble_group.visible = true
	for bubb in bubble_group.get_children():
		bubb.set_collision(true)
	
	
func hide_menu_button(pos):
	if(bubble_game.game_state != "Title"):
		return
	button_texts.get_child(pos).visible = false
	bubbles_groups.get_child(pos).visible = false
	menu_bubble.pop_out()
	


func _on_mouseaimhover2d_area_shape_entered(area_rid: RID, area: Area2D, area_shape_index: int, local_shape_index: int) -> void:
	if(area.is_in_group("title_menu_button")):
		var pos = area.get_index()
		get_parent().animation = str(pos)


func _on_mouseaimhover2d_area_shape_exited(area_rid: RID, area: Area2D, area_shape_index: int, local_shape_index: int) -> void:
	if(area.is_in_group("title_menu_button")):
		get_parent().animation = "default"

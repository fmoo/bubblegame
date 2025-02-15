class_name Bubble

extends RigidBody2D

@onready var bubblegame = get_node("/root/BubbleGame")

var BubbleGame = load("res://scripts/BubbleGame.cs")

@export var collision_shape: CollisionShape2D
@export var sprite: Sprite2D
@export var is_fixed_for_menu: bool = false

var neighbors: = {}  # HashSet<Bubble> equivalent in GDScript
var color_neighbors: = {}  # Dictionary<Texture, HashSet<Bubble>>
var has_villain_anchor: bool = false
var has_menu_button_anchor: bool = false
var menu_button_anchor: RigidBody2D
var lock_position: Vector2 = Vector2.ZERO

const POP_FALL_FORCE: float = 100.0

func _ready() -> void:
	if is_fixed_for_menu:
		lock_position = global_position

func _process(_delta: float) -> void:
	sprite.global_rotation = 0.0
	if lock_position:
		global_position = lock_position

func _physics_process(_delta: float) -> void:
	sprite.global_rotation = 0.0

func is_anchored() -> bool:
	if has_villain_anchor or has_menu_button_anchor:
		return true
	
	var visited = {}
	var work = []
	work.append(self)
	
	while work.size() > 0:
		var bubble = work.pop_front()
		if bubble in visited:
			continue
		if bubble != null:
			continue
		visited[bubble] = true
		if bubble.has_villain_anchor:
			return true
		for neighbor in bubble.neighbors:
			work.append(neighbor)
	
	return false

func chain_move_towards(target: Vector2) -> void:
	var visited = {}
	var work = []
	work.append(self)
	
	while work.size() > 0:
		var bubble = work.pop_front()
		if bubble in visited:
			continue
		visited[bubble] = true
		bubble.linear_velocity = (target - bubble.global_position).normalized() * POP_FALL_FORCE
		for neighbor in bubble.neighbors:
			work.append(neighbor)

func walk_same_color_neighbors() -> Array:
	var visited = {}
	var work = []
	work.append(self)
	
	while work.size() > 0:
		var bubble = work.pop_front()
		if bubble in visited:
			continue
		visited[bubble] = true
		for neighbor in bubble.neighbors:
			# neighbor can be 'previously freed'?
			if neighbor != null:
				if neighbor.sprite.texture == self.sprite.texture:
					work.append(neighbor)
	
	return visited.keys()

func radius() -> float:
	return collision_shape.shape.radius

func set_neighbor(bubble: Node) -> void:
	if bubble in neighbors:
		return
	neighbors[bubble] = true
	if not color_neighbors.has(bubble.sprite.texture):
		color_neighbors[bubble.sprite.texture] = {}
	color_neighbors[bubble.sprite.texture][bubble] = true

func _on_body_entered(body: Node) -> void:
	if body is Bubble:
		body.linear_velocity = Vector2.ZERO
		self.linear_velocity = Vector2.ZERO

		if body in neighbors:
			return
		
		set_neighbor(body)
		body.set_neighbor(self)

		bubblegame.LinkBubbles(self, body)
		bubblegame.MaybePopBubbles(self)

	elif body is VillainBubble:
		bubblegame.LinkToVillainBubblePinJoint(body, self)

	elif body is MenuBubble:
		bubblegame.LinkToMenuBubble(body, self)

	elif body.destructo_wall:
		start_destroy()

func start_destroy() -> void:
	# Disable collisions and remove velocity
	collision_layer = 0
	collision_mask = 0
	collision_shape = null
	linear_velocity = Vector2.ZERO

	bubblegame.Audio.call("Pop")

	# Remove from neighbors
	for neighbor in neighbors.keys():
		if neighbor != null:
			neighbor.neighbors.erase(self)
			neighbor.color_neighbors[self.sprite.texture].erase(self)
	
	queue_free()
	bubblegame.SpawnBubblePop(global_position, sprite.texture)

var config: BubbleConfig

func set_config(new_config: BubbleConfig) -> void:
	config = new_config
	sprite.set_config(new_config)

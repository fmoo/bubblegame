class_name BubbleGame
extends Node2D

@export var audio: Audio
@export var bubbleQueue: BubbleQueue
@export var gameplayConfig: GameplayConfig
@export var Springs: Node2D
@export var Bubbles: Node2D
@export var villainBubble: VillainBubble
@export var villainBubbleHighlight: Node2D
@export var villainBubbleEyes: Node2D
@export var Player: BubbleGun
@export var debugMode: bool = false
@export var TitleMode: bool = false
@export var PinJointTemplate: PackedScene
@export var GrooveJointTemplate: PackedScene
@export var VillainPinJointTemplate: PackedScene
@export var renderHighScore: RenderHighScore
@export var bubbleSprite: PackedScene;

@export var GameOverPanel: Control
@export var NotificationLabel: PackedScene

var Score = 0;
signal ScoreChanged(score: int)
signal TimeElapsed(seconds: int)
signal ChainChanged(chain: int)
signal LargeBurst(size: int)

var lastSecond: int = 0
var timeElapsed: float = 0
var Pressure: float = 0
var currentPressureDuration: float = 0
var multi_shrink_wait: bool = false

func GetDifficultyMultiplier() -> float:
	return pow(2, (gameplayConfig.BasePressureDuration / currentPressureDuration) - 1)

func GainPoints(points: int) -> int:
	Score += points
	ScoreChanged.emit(Score)
	return Score

func _ready() -> void:
	if GameOverPanel != null:
		GameOverPanel.visible = false
	LargeBurst.connect(on_large_burst)
	MaybePickNewVillainBubbleColor()
	reset()

func RegisterBubble(bubble: Bubble) -> void:
	Bubbles.add_child(bubble)


func DestroyBubble(bubble: Bubble, multiplier: float) -> void:
	var canShrink = true
	if gameplayConfig.VillainBubbleColorChanges and bubble.config != villainBubble.config:
		canShrink = false

	if bubble != null and not bubble.is_queued_for_deletion() and canShrink:
		print("Pop: Pressure decrease by ", 1.0 / gameplayConfig.ShrinkBubblePops)
		Pressure -= 1.0 * multiplier / gameplayConfig.ShrinkBubblePops
	print("Destroying bubble ", bubble)
	bubble.start_destroy()


func MaybePickNewVillainBubbleColor() -> void:
	# Pick a new color for the villain bubble
	if not gameplayConfig.VillainBubbleColorChanges:
		return
	var currentColor = villainBubble.config
	var newColor = PickColor()
	while newColor == currentColor:
		newColor = PickColor()
	villainBubble.set_config(newColor)


func LinkBubbles(bubble1: Bubble, bubble2: Bubble) -> PinJoint2D:
	var joint = PinJointTemplate.instantiate() as PinJoint2D
	bubble1.add_child(joint)
	joint.node_a = bubble1.get_path()
	joint.node_b = bubble2.get_path()
	joint.global_position = bubble1.global_position
	joint.global_rotation = bubble1.global_position.angle_to_point(bubble2.global_position)
	return joint

func LinkToVillainBubblePinJoint(villain_bubble: VillainBubble, bubble: Bubble) -> PinJoint2D:
	print("Villain Bubble at ", villain_bubble.global_position, " linked to Bubble at ", bubble.global_position)
	bubble.has_villain_anchor = true

	var joint = VillainPinJointTemplate.instantiate() as PinJoint2D
	villain_bubble.add_child(joint)
	joint.node_a = villain_bubble.get_path()
	joint.node_b = bubble.get_path()
	joint.global_position = villain_bubble.global_position
	joint.global_rotation = villain_bubble.global_position.angle_to_point(bubble.global_position)
	return joint

func LinkToMenuBubble(menuBubble: MenuBubble, bubble: Bubble) -> void:
	# If the position of the bubble is beyond the width of the bubble, use a pin joint,
	# otherwise use a groove joint.
	if (bubble.global_position.x > menuBubble.global_position.x + menuBubble.get_rectangle_shape().size.x / 2 or
		bubble.global_position.x < menuBubble.global_position.x - menuBubble.get_rectangle_shape().size.x / 2):
		bubble.lock_position = bubble.global_position
	else:
		LinkToMenuBubbleGrooveJoint(menuBubble, bubble)
	bubble.has_villain_anchor = true
	bubble.has_menu_button_anchor = true
	bubble.menu_button_anchor = menuBubble
	

func LinkToMenuBubbleGrooveJoint(menuBubble: MenuBubble, bubble: Bubble) -> GrooveJoint2D:
	print("Menu Bubble at ", menuBubble.global_position, " linked to Bubble at ", bubble.global_position)
	bubble.has_villain_anchor = true
	bubble.has_menu_button_anchor = true
	bubble.menu_button_anchor = menuBubble

	var joint = GrooveJointTemplate.instantiate() as GrooveJoint2D
	menuBubble.add_child(joint)
	joint.set_deferred("node_a", menuBubble.get_path())
	joint.set_deferred("node_b", bubble.get_path())
	joint.global_position = Vector2(
		menuBubble.global_position.x - menuBubble.get_rectangle_shape().size.x / 2,
		bubble.global_position.y
	)
	joint.length = menuBubble.get_rectangle_shape().size.x
	joint.initial_offset = joint.length - (menuBubble.global_position.x - bubble.global_position.x + (menuBubble.get_rectangle_shape().size.x / 2))
	joint.global_rotation_degrees = -90

	return joint

func PickColor() -> BubbleConfig:
	var index = randi() % gameplayConfig.Bubbles.size()
	return gameplayConfig.Bubbles[index]

var ChainTimeRemaining: float = 0
var currentChain: int = 1


func MaybePopBubbles(bubble: Bubble) -> void:
	if bubble.is_queued_for_deletion():
		return

	var maybePop = bubble.walk_same_color_neighbors()
	if maybePop.size() < gameplayConfig.MinMatchSize:
		return

	if not TitleMode:
		var pointsGained = 50
		# 10x the points for every extra bubble popped simultaneously
		pointsGained *= pow(10, maybePop.size() - gameplayConfig.MinMatchSize)
		# Scale points based on the current VillainBubbleMultiplier
		print("Gaining Points: Base=", pointsGained, "  SizeMul=", villainBubble.get_score_multiplier(), "  ChainMul=", currentChain, "  SpeedMul=", GetDifficultyMultiplier())
		pointsGained = int(pointsGained * villainBubble.get_score_multiplier() * currentChain * GetDifficultyMultiplier())
		GainPoints(pointsGained)
		currentChain += 1
		ChainChanged.emit(currentChain)
		ChainTimeRemaining = gameplayConfig.ChainDuration

	var maybeFall = []
	var WasAnchored = false
	var AnchoredMenuBubble: MenuBubble = null
	for p in maybePop:
		for f in p.neighbors:
			if not maybePop.has(f):
				maybeFall.append(f)
		if p.is_anchored():
			WasAnchored = true
			AnchoredMenuBubble = p.menu_button_anchor
		# for each ball popped over min match size, multiply shrink amount
		var ShrinkMultiplier = maybePop.size() - (gameplayConfig.MinMatchSize - 1)
		if(ShrinkMultiplier > 1):
			emit_signal("LargeBurst", maybePop.size())
		DestroyBubble(p, ShrinkMultiplier)

	if TitleMode:
		if WasAnchored:
			call(AnchoredMenuBubble.game_function)
		return
	for f in maybeFall:
		if f != null:
			if f.is_anchored():
				continue
			f.chain_move_towards(villainBubble.global_position)

func DebugTestFall() -> void:
	for bubble in Bubbles.get_children():
		if bubble.is_anchored():
			continue
		bubble.chain_move_towards(villainBubble.global_position)

func _process(delta: float) -> void:
	if TitleMode:
		return
	if gameplayConfig.TimerTicks:
		if gameplayConfig.ChainsStallPressure and currentChain > 1:
			# Do nothing
			pass
		else:
			Pressure += 1.0 / gameplayConfig.GrowBubbleShots / currentPressureDuration * delta
			# print("Tick: Pressure increase by ", 1.0 / gameplayConfig.GrowBubbleShots / currentPressureDuration * delta, " -> ", Pressure)

	timeElapsed += delta
	if int(timeElapsed) != lastSecond:
		lastSecond = int(timeElapsed)
		TimeElapsed.emit(lastSecond)

	if Pressure > 1.0:
		print("Pressure ", Pressure, " over threshold! Growing!")
		villainBubble.grow()
		Pressure -= 1.0
	elif Pressure < 0.0 && !multi_shrink_wait:
		print("Pressure ", Pressure, " under threshold! shrinking!")
		if villainBubble.shrink() :
			Pressure += 1.0
			if(Pressure < 0.0):
				#still need to do another shrink at least
				start_new_multi_shrink_wait()
		else:
			Pressure = 0.0
		currentPressureDuration *= gameplayConfig.PressureDecayMultiplier
		MaybePickNewVillainBubbleColor()
		
	# bulge bubble
	if(Pressure >= 0.0):
		var pressure_incs = floor(Pressure / .2)
		var h_bulge_amt = 0.0 + pressure_incs * .06
		var e_bulge_amt = 0.0 + pressure_incs * .2
		villainBubbleHighlight.get_material().set_shader_parameter("bulge", h_bulge_amt)
		villainBubbleEyes.get_material().set_shader_parameter("bulge", e_bulge_amt)

	if currentChain > 1:
		ChainTimeRemaining -= delta
		if ChainTimeRemaining <= 0:
			currentChain = 1
			ChainChanged.emit(currentChain)
			ChainTimeRemaining = 0

func start_new_multi_shrink_wait():
	multi_shrink_wait = true
	await get_tree().create_timer(.5).timeout
	multi_shrink_wait = false
	
func GameOver() -> void:
	audio.GameOver()
	print("Game Over")
	if renderHighScore != null:
		renderHighScore.SaveHighScore()
	PauseWithPanel(GameOverPanel)

var VisiblePausePanel: Control = null;

func PauseWithPanel(panel: Control) -> void:
	if get_tree().paused:
		return
	get_tree().paused = true
	VisiblePausePanel = panel
	panel.visible = true

func Unpause() -> void:
	if not get_tree().paused:
		return
	get_tree().paused = false
	if VisiblePausePanel != null:
		VisiblePausePanel.visible = false
		VisiblePausePanel = null

func _on_play_again_pressed() -> void:
	reset()
	Unpause()

func reset() -> void:
	timeElapsed = 0
	currentPressureDuration = gameplayConfig.BasePressureDuration
	Pressure = 0
	if not TitleMode:
		for bubble in Bubbles.get_children():
			bubble.queue_free()
		for spring in Springs.get_children():
			spring.queue_free()
		villainBubble.reset()
		MaybePickNewVillainBubbleColor()

	Player.reset()
	bubbleQueue.reset()
	Score = 0
	ScoreChanged.emit(Score)

func PlayGame() -> void:
	# Wait for 1 second
	await get_tree().create_timer(1.0).timeout
	get_tree().change_scene_to_file("res://scenes/bubble_game.tscn")

func _on_shoot_event() -> void:
	if TitleMode:
		return
	print("Shoot: Pressure increase by ", 1.0 / gameplayConfig.GrowBubbleShots)
	Pressure += 1.0 / gameplayConfig.GrowBubbleShots

func SpawnBubblePop(globalPosition: Vector2, texture: Texture2D) -> void:
	var bubblePop = bubbleSprite.instantiate() as BubbleSprite
	add_child(bubblePop)
	bubblePop.global_position = globalPosition
	bubblePop.texture = texture
	bubblePop.PlayAnimation("pop")

func Quit() -> void:
	get_tree().quit()

func _on_main_menu_pressed() -> void:
	get_tree().paused = false
	get_tree().change_scene_to_file("res://scenes/title.tscn")

func on_large_burst(size):
	var notif = NotificationLabel.instantiate()
	notif.position = Vector2(370,70)
	add_child(notif)

class_name BubbleGun
extends Node2D

@export var bubbleScene: PackedScene
@export var pathFollow: PathFollow2D
@export var angleLimit: float = 80
@export var bubbleSpeed: float = 200
@export var rotationSpeed: float = 2
@export var trackSpeed: float = 25
@export var animatedBody: AnimatedSprite2D
@export var turretSprite: Sprite2D
@export var bubblePreviewSprite: Sprite2D
@export var ActiveMoveTarget: PathFollow2D

signal OnShoot

var COOLDOWN: float = 0.300
var cooldownTime: float = 0

var Strafe: int = 0
var TurnTurret: int = 0

func _ready() -> void:
	animatedBody.speed_scale = 0
	animatedBody.play()

func _physics_process(_delta: float) -> void:
	bubblePreviewSprite.global_rotation = 0

func GetStrafe() -> int:
	if ActiveMoveTarget.is_visible_in_tree():
		var TargetRatio = ActiveMoveTarget.progress_ratio
		var CurrentRatio = pathFollow.progress_ratio
		var diff = (TargetRatio - CurrentRatio + 1)
		while diff > 1:
			diff -= 1
		if abs(diff) < 0.005:
			ActiveMoveTarget.visible = false
			return 0
		if diff < 0.5:
			return 1
		else:
			return -1
	return Strafe

func _process(delta: float) -> void:
	if cooldownTime > 0:
		cooldownTime = max(0, cooldownTime - delta)
	bubblePreviewSprite.global_rotation = 0
	var speedScale = 0;
	if TurnTurret < 0:
		if RotateGun(-1, delta):
			speedScale -= 0.6
	elif TurnTurret > 0:
		if RotateGun(1, delta):
			speedScale += 0.6

	var strafe = GetStrafe()
	if strafe < 0:
		pathFollow.progress_ratio = (pathFollow.progress_ratio - (delta * trackSpeed) + 1)
		while pathFollow.progress_ratio > 1:
			pathFollow.progress_ratio -= 1
		speedScale += 1
	elif strafe > 0:
		pathFollow.progress_ratio = (pathFollow.progress_ratio + (delta * trackSpeed))
		while pathFollow.progress_ratio > 1:
			pathFollow.progress_ratio -= 1

	animatedBody.speed_scale = speedScale


func RotateGun(direction: int, delta: float) -> bool:
	var oldRotation = turretSprite.rotation
	turretSprite.rotation += direction * delta * rotationSpeed
	turretSprite.rotation = clamp(turretSprite.rotation, -deg_to_rad(angleLimit), deg_to_rad(angleLimit))
	return oldRotation != turretSprite.rotation

@onready var bubbleGame = get_node("/root/BubbleGame")

func Shoot() -> void:
	if cooldownTime > 0:
		return
	bubbleGame.Audio.Shoot()
	var bubble = bubbleScene.instantiate()
	bubbleGame.RegisterBubble(bubble)

	# Set the color
	bubble.SetConfig(bubbleGame.BubbleQueue.DequeueColor())

	# Trajectory and position
	bubble.global_position = turretSprite.global_position
	bubble.linear_velocity = Vector2(0, -1).rotated(turretSprite.global_rotation) * bubbleSpeed
	cooldownTime = COOLDOWN
	OnShoot.emit();


func SetTrackDestination(InputDirection: Vector2) -> void:
	var TargetRatio = (InputDirection.angle_to(Vector2.UP) - PI) / TAU + 1
	ActiveMoveTarget.progress_ratio = TargetRatio
	ActiveMoveTarget.visible = true

func TurretLookAt(GlobalMousePosition: Vector2) -> void:
	var RelativeTarget = turretSprite.global_position - GlobalMousePosition
	turretSprite.global_rotation = RelativeTarget.angle() - PI / 2
	turretSprite.rotation = clamp(turretSprite.rotation, -deg_to_rad(angleLimit), deg_to_rad(angleLimit))

func SetStrafe(strafeValue: int) -> void:
	Strafe = strafeValue
	if strafeValue != 0:
		ActiveMoveTarget.visible = false

func SetTrackAngle(InputDirection: Vector2) -> void:
	pathFollow.progress_ratio = -InputDirection.angle_to(Vector2.UP) / TAU

func SetTurretAngle(InputDirection: Vector2) -> void:
	turretSprite.global_rotation = InputDirection.angle_to(Vector2.DOWN)
	turretSprite.rotation = clamp(turretSprite.rotation, -deg_to_rad(angleLimit), deg_to_rad(angleLimit))

func Reset() -> void:
	ActiveMoveTarget.visible = false
	pathFollow.progress_ratio = 0
	turretSprite.rotation = 0

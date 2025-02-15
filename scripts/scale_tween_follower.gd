class_name ScaleTweenFollower
extends Sprite2D

@export var Follow: Node2D
@export var ScaleMultiplier: float = 0.25

func _process(_delta: float) -> void:
	# Move the Scale of this sprite towards the Scale of the Follow node multiplied by ScaleMultiplier
	scale = lerp(scale, Follow.scale * ScaleMultiplier, 0.1)

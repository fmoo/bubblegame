class_name InitialControlScheme
extends ControlSchemeBase

@onready var bubbleGame = get_node("/root/BubbleGame")
@export var _MouseControls: MouseControls

func MaybeDisableMouseAim() -> void:
    if _MouseControls != null:
        _MouseControls.DisableMouseAim = true


func _process(_delta: float) -> void:
    if Input.is_action_pressed("ui_left"):
        bubbleGame.Player.TurnTurret = -1
        MaybeDisableMouseAim()
    elif Input.is_action_pressed("ui_right"):
        bubbleGame.Player.TurnTurret = 1
        MaybeDisableMouseAim()
    else:
        bubbleGame.Player.TurnTurret = 0

    if Input.is_action_pressed("track_left"):
        bubbleGame.Player.SetStrafe(-1)
    elif Input.is_action_pressed("track_right"):
        bubbleGame.Player.SetStrafe(1)
    else:
        bubbleGame.Player.SetStrafe(0)
class_name PauseGame
extends ControlSchemeBase

@export var PausePanel: Control
@onready var bubbleGame: BubbleGame = get_node("/root/BubbleGame")

func _process(_delta: float) -> void:
    if Input.is_action_just_pressed("toggle_pause"):
        if not get_tree().is_paused():
            if PausePanel != null:
                bubbleGame.PauseWithPanel(PausePanel)
        else:
            bubbleGame.Unpause()
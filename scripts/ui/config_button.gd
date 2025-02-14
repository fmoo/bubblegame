class_name ConfigButton
extends Button

@export var ConfigPanel: Control;
@onready var bubbleGame = get_node("/root/BubbleGame")

func _ready():
    pressed.connect(_on_pressed)

func _on_pressed():
    bubbleGame.PauseWithPanel(ConfigPanel)

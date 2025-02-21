
class_name UnpauseButton
extends Button

func _ready():
	pressed.connect(unpause)

func unpause():
	# Get /BubbleGame/ and call Unpause() on it.
	get_node("/root/BubbleGame").Unpause()

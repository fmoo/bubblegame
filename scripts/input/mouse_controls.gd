class_name MouseControls
extends ControlSchemeBase

@onready var bubbleGame = get_node("/root/BubbleGame")
@export var moveTargetPath: PathFollow2D
var moveMouseDown: bool = false
var DisableMouseAim: bool = false

func _input(event: InputEvent) -> void:
    if not is_visible_in_tree():
        return

    # Right click means shoot
    if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_RIGHT and event.is_pressed():
        bubbleGame.Player.Shoot()

    # Left click means SetStrafePosition
    if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
        if event.is_pressed():
            moveMouseDown = true
            var InputDirection = get_local_mouse_position()
            bubbleGame.Player.SetTrackDestination(InputDirection)
        else:
            moveMouseDown = false

    # If mouse moves, re-enable mouse aim
    if event is InputEventMouseMotion:
        DisableMouseAim = false

func _process(_delta: float) -> void:
    var InputDirection = get_local_mouse_position()
    if not DisableMouseAim:
        moveTargetPath.progress_ratio = (InputDirection.angle_to(Vector2.UP) - PI) / TAU
        bubbleGame.Player.TurretLookAt(get_global_mouse_position())

    # If the player is holding the button down, update the targeting to reflect
    if moveMouseDown:
        bubbleGame.Player.SetTrackDestination(InputDirection)
extends Node
class_name Audio

@export var BGM: AudioStreamPlayer = null

# export a bool
@export var _Shoot:AudioStreamPlayer = null
@export var _Pop:AudioStreamPlayer = null
@export var _Grow:AudioStreamPlayer = null
@export var _Shrink:AudioStreamPlayer = null
@export var _GameOver:AudioStreamPlayer = null

var didPlayThisFrame = {}

func MaybePlay(player: AudioStreamPlayer):
    if player == null:
        return
    if didPlayThisFrame.has(player):
        return
    didPlayThisFrame[player] = true
    player.play()

func Shoot():
    MaybePlay(_Shoot)

func Pop():
    MaybePlay(_Pop)

func Grow():
    MaybePlay(_Grow)

func Shrink():
    MaybePlay(_Shrink)

func GameOver():
    MaybePlay(_GameOver)

func _process(_delta):
    didPlayThisFrame = {}


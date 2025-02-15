class_name RenderHighScore
extends Label

@export var _RenderScore: RenderScore
var CurrentHighScore: int = 0;

const RES_HIGH_SCORE = "user://highscore3.dat"

func _ready() -> void:
    if !FileAccess.file_exists(RES_HIGH_SCORE):
        return
    var file = FileAccess.open(RES_HIGH_SCORE, FileAccess.ModeFlags.READ)
    CurrentHighScore = file.get_64()
    file.close()

func SaveHighScore() -> void:
    var file = FileAccess.open(RES_HIGH_SCORE, FileAccess.ModeFlags.WRITE)
    file.store_64(CurrentHighScore)
    file.close()

func _process(_delta: float) -> void:
    if _RenderScore != null and _RenderScore.displayScore > CurrentHighScore:
        CurrentHighScore = int(_RenderScore.displayScore)
    text = str(CurrentHighScore).pad_zeros(8)

func _exit_tree() -> void:
    SaveHighScore()
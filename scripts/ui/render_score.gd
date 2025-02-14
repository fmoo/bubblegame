class_name RenderScore
extends Label

@onready var bubbleGame = get_node("/root/BubbleGame")
var baseScore: float = 0
var targetScore: float = 0
var displayScore: float = 0
var scoreTweenProgress: float = 0
const INCREMENT_DURATION: float = 1

func _ready() -> void:
    bubbleGame.ScoreChanged.connect(_on_score_changed)

func _on_score_changed(score: int):
    if score == 0:
        baseScore = 0
        displayScore = 0
        targetScore = 0
        scoreTweenProgress = 1
        Refresh()
        return
    else:
        baseScore = displayScore
        scoreTweenProgress = 0
        targetScore = score
        Refresh()
    # print($"baseScore: {baseScore} displayScore: {displayScore} targetScore: {targetScore}")

func Refresh() -> void:
    displayScore = lerp(baseScore, targetScore, clamp(scoreTweenProgress, 0, 1))
    text = str(int(displayScore)).pad_zeros(8)


func _process(delta: float) -> void:
    if displayScore != targetScore:
        scoreTweenProgress += delta / INCREMENT_DURATION
        if scoreTweenProgress >= 1:
            scoreTweenProgress = 1
        Refresh()
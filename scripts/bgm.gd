class_name BGM
extends AudioStreamPlayer

@export var BGMConfigs: Array[BGMConfig] = []

signal OnBGMChange(config: BGMConfig)

var timeSinceStart: float = 0
var isChangingTracks: bool = false
var currentConfig: BGMConfig = null

const FADE_OUT_TIME: float = 4
const MIN_DB: float = -80


func _ready() -> void:
	currentConfig = PlayRandom()

func _process(delta: float) -> void:
	timeSinceStart += delta
	if timeSinceStart >= currentConfig.FadeAfter && !isChangingTracks:
		PlayRandom()


func ChangeBGM(config: BGMConfig) -> void:
	stream = config._AudioStream
	volume_db = 0
	play()
	timeSinceStart = 0
	isChangingTracks = false
	OnBGMChange.emit(config)

func PlayRandom() -> BGMConfig:
	var fadeOutTime = FADE_OUT_TIME
	if stream == null: fadeOutTime = 0

	isChangingTracks = true
	var config = BGMConfigs[randi() % BGMConfigs.size()]

	if BGMConfigs.size() > 1:
		while stream == config._AudioStream:
			config = BGMConfigs[randi() % BGMConfigs.size()]

	print("Playing " + config.Title + " by " + config.Composer)
	var tween = get_tree().create_tween()
	tween.tween_property(self, "volume_db", MIN_DB, fadeOutTime)
	tween.tween_callback(Callable(self, "ChangeBGM").bind(config))

	return config

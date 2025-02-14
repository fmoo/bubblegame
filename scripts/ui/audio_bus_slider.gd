class_name AudioBusSlider
extends HSlider

@export var BusName: String
@export var PlaySFX: bool = false
@onready var busIndex: int = AudioServer.get_bus_index(BusName)
@onready var bubbleGame = get_node("/root/BubbleGame")

var timeSinceSound: float = 10000
const SFX_DEBOUNCE: float = 0.2

func _ready():
	min_value = 0
	max_value = 1
	step = 0.01
	# Value = AudioServer.GetBusVolumeDb(busIndex));
	# ValueChanged += _on_value_changed;
	value = DbToRatio(AudioServer.get_bus_volume_db(busIndex))
	value_changed.connect(_on_value_changed)


func _on_value_changed(new_value: float) -> void:
	AudioServer.set_bus_volume_db(
		busIndex,
		RatioToDb(new_value)
	);

	if PlaySFX:
		if timeSinceSound > SFX_DEBOUNCE:
			timeSinceSound = 0;
			bubbleGame.Audio.Shoot();
			print("Playing sound")
		else:
			print("Skpping sound")


func _process(delta: float) -> void:
	if (PlaySFX):
		timeSinceSound += delta;

func DbToRatio(db: float) -> float:
	return pow(10, db / 20)

func RatioToDb(new_ratio: float) -> float:
	return 20 * (log(new_ratio) / log (10))

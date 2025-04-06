extends TextureRect

var time_passed
@export var ydir: float

var hue
var col

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	time_passed = 0.0
	col = Color()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	time_passed += 4*delta
	time_passed = fmod(time_passed,360)
	hue = floor(time_passed)
	
	col.h = hue/360.0
	col.s = 11/100.0
	col.v = 88.0/100.0
	set_modulate(col)
	
	#
	#time_passed = fposmod(time_passed,2.0*PI)
	#ydir = sin(time_passed) / 2.0
	#print(ydir)
	#get_material().set_shader_parameter("dir", Vector2(-.5,ydir))

class_name GameplayConfig
extends Resource

@export var Name: String = "Normal"
@export var Description: String = "A normal way to play."
@export var Bubbles: Array[BubbleConfig] = []
@export var VillainBubbleColorChanges: bool = false
@export var ChainDuration: float = 4
@export var TimerTicks: bool = true;
@export var MinMatchSize: int = 3;
@export var ShrinkBubblePops: int = 4;
@export var GrowBubbleShots: int = 5;
@export var BasePressureDuration: float = 6
@export var PressureDecayMultiplier: float = 0.99
@export var ChainsStallPressure: bool = true;

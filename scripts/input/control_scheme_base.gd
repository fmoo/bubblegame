class_name ControlSchemeBase
extends Node2D

@export var ConflictingSchemes: Array[ControlSchemeBase] = [];
@export var VisibleProcessMode: ProcessMode = ProcessMode.PROCESS_MODE_INHERIT;
@export var InvisibleProcessMode: ProcessMode = ProcessMode.PROCESS_MODE_DISABLED;

func _init() -> void:
    visibility_changed.connect(_on_visibility_changed)

func _ready() -> void:
    _on_visibility_changed()

func _on_visibility_changed() -> void:
    if is_visible_in_tree():
        process_mode = VisibleProcessMode
    else:
        process_mode = InvisibleProcessMode

    if is_visible_in_tree():
        for Scheme in ConflictingSchemes:
            if Scheme.IsVisibleInTree():
                Scheme.Hide();
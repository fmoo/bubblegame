using Godot;
using System;

public partial class ControlSchemeBase : Node2D {
    [Export] ControlSchemeBase[] ConflictingSchemes = new ControlSchemeBase[0];
    [Export] ProcessModeEnum VisibleProcessMode = ProcessModeEnum.Inherit;
    [Export] ProcessModeEnum InvisibleProcessMode = ProcessModeEnum.Disabled;

    public ControlSchemeBase() {
        VisibilityChanged += _on_visibility_changed;
    }

    void _on_visibility_changed() {
        ProcessMode = IsVisibleInTree() ? VisibleProcessMode : InvisibleProcessMode;
        if (IsVisibleInTree()) {
            foreach (var Scheme in ConflictingSchemes) {
                if (Scheme.IsVisibleInTree()) {
                    Scheme.Hide();
                }
            }
        }
    }

    public override void _Ready() {
        base._Ready();
        _on_visibility_changed();
    }
}

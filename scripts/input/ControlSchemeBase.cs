using Godot;
using System;

public partial class ControlSchemeBase : Node2D {
    public ControlSchemeBase() {
        VisibilityChanged += _on_visibility_changed;
    }

    void _on_visibility_changed() {
        ProcessMode = IsVisibleInTree() ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }

    public override void _Ready() {
        base._Ready();
        _on_visibility_changed();
    }
}

using Godot;
using System;

public partial class ContainRect : TextureProgressBar {
	float wantValue = 0;

	void _on_contain_ratio_changed(float containRatio) {
		wantValue = containRatio;
	}

	public override void _Ready() {
		BubbleGame.Game.VillainBubble.Connect("contain_ratio_changed", Callable.From<float>(_on_contain_ratio_changed));
	}

	public override void _Process(double delta) {
		// Ease in Value towards want value, unless wantValue = 0, then just set it instantly
		if (wantValue == 0) {
			Value = 0;
			return;
		}

		Value = Mathf.Lerp(Value, wantValue, 0.1);
	}
}

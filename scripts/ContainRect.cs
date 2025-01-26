using Godot;
using System;

public partial class ContainRect : TextureProgressBar {
	double wantValue = 0;

	void _on_contain_ratio_changed(double containRatio) {
		wantValue = containRatio;
	}

	public override void _Ready() {
		BubbleGame.Game.VillainBubble.ContainRatioChanged += _on_contain_ratio_changed;
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

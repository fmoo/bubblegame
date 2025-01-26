using Godot;
using System;

public partial class RenderGameTime : Label {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		BubbleGame.Game.TimeElapsed += _on_time_elapsed;
	}

	void _on_time_elapsed(int seconds) {
		// Format time in mm:ss
		var colon = ":";
		Text = $"{seconds / 60}{colon}{seconds % 60:00}";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}

using Godot;
using System;

public partial class RenderHighScore : Label {
	[Export] RenderScore RenderScore;

	int currentHighScore = 0;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (RenderScore.displayScore > currentHighScore) {
			currentHighScore = (int)RenderScore.displayScore;
		}
		Text = $"{currentHighScore:00000000}";
	}
}

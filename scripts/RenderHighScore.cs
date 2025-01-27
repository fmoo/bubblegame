using Godot;
using System;

public partial class RenderHighScore : Label {
	[Export] RenderScore RenderScore;

	public static long CurrentHighScore = 0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (RenderScore != null && RenderScore.displayScore > CurrentHighScore) {
			CurrentHighScore = (long)RenderScore.displayScore;
		}
		Text = $"{CurrentHighScore:00000000}";
	}
}

using Godot;
using System;

public partial class RenderHighScore : Label {
	[Export] RenderScore RenderScore;

	public static long CurrentHighScore = 0;

	public override void _Ready() {
		base._Ready();
		if (!FileAccess.FileExists("user://highscore.dat")) {
			return;
		}
		var file = FileAccess.Open("user://highscore.dat", FileAccess.ModeFlags.Read);
		CurrentHighScore = long.Parse(file.GetLine());
		file.Close();
	}

	public static void SaveHighScore() {
		var file = FileAccess.Open("user://highscore.dat", FileAccess.ModeFlags.Write);
		file.StoreLine(CurrentHighScore.ToString());
		file.Close();
	}

	public override void _Process(double delta) {
		if (RenderScore != null && RenderScore.displayScore > CurrentHighScore) {
			CurrentHighScore = (long)RenderScore.displayScore;
		}
		Text = $"{CurrentHighScore:00000000}";
	}
}

using Godot;
using System;

public partial class RenderScore : RichTextLabel {
	float currentScore = 0;
	int targetScore = 0;

	public override void _Ready() {
		BubbleGame.Game.ScoreChanged += OnScoreChanged;
	}

	void OnScoreChanged(int score) {
		if (score == 0) {
			currentScore = 0;
			targetScore = 0;
			Refresh();
		}
		targetScore = score;
	}

	void Refresh() {
		Text = $"{(int)currentScore:00000000}";
	}


	public override void _Process(double delta) {
		base._Process(delta);
		//move the current score towards the target score
		if (currentScore != targetScore) {
			currentScore = (float)Mathf.MoveToward(currentScore, targetScore, 1000 * delta);
			Refresh();
		}
	}

}

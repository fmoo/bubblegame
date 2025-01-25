using Godot;
using System;

public partial class RenderScore : Label {
	float baseScore = 0;
	float targetScore = 0;
	float displayScore = 0;

	public override void _Ready() {
		BubbleGame.Game.ScoreChanged += OnScoreChanged;
	}

	void OnScoreChanged(int score) {
		if (score == 0) {
			baseScore = 0;
			displayScore = 0;
			targetScore = 0;
			scoreTweenProgress = 1f;
			Refresh();
			return;
		} else {
			baseScore = displayScore;
			scoreTweenProgress = 0f;
			targetScore = score;
			Refresh();
		}
		// GD.Print($"baseScore: {baseScore} displayScore: {displayScore} targetScore: {targetScore}");
	}

	void Refresh() {
		displayScore = Mathf.Lerp(baseScore, targetScore, Math.Clamp(scoreTweenProgress, 0f, 1f));
		Text = $"{(int)displayScore:00000000}";
	}

	float scoreTweenProgress = 0f;
	const float INCREMENT_DURATION = 1f;

	public override void _Process(double delta) {
		base._Process(delta);
		// Lerp the current score towards the target score
		if (displayScore != targetScore) {
			scoreTweenProgress += (float)delta / INCREMENT_DURATION;
			if (scoreTweenProgress >= 1) {
				scoreTweenProgress = 1;
			}
			Refresh();
		}
	}

}

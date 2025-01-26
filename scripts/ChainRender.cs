using Godot;
using System;

public partial class ChainRender : TextureProgressBar {
	[Export] Label ChainLabel;
	public override void _Ready() {
		BubbleGame.Game.ChainChanged += OnChainChanged;
		MinValue = 0;
		MaxValue = BubbleGame.Game.GameplayConfig.ChainDuration;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Value = BubbleGame.Game.ChainTimeRemaining;
	}

	void OnChainChanged(int chain) {
		GD.Print($"Chain changed to {chain}");
		ChainLabel.Text = $"{chain}x";
	}
}

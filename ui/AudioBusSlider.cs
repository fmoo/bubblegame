using System;
using Godot;

public partial class AudioBusSlider : HSlider {

	// [Export] AudioBusLayout _AudioBusLayout;
	[Export] string BusName;
	[Export] bool PlaySFX = false;
	int busIndex;

	// Called when the node enters the scene tree for the first time.

	public override void _Ready() {
		busIndex = AudioServer.GetBusIndex(BusName);
		GD.Print($"Got bus index {busIndex} for {BusName}");
		MinValue = 0;
		MaxValue = 1;
		Step = 0.01f;
		// Normalize the exponential volume to linear
		Value = DbToRatio(AudioServer.GetBusVolumeDb(busIndex));
		ValueChanged += _on_value_changed;
	}

	double DbToRatio(double db) {
		return Math.Pow(10, db / 20);
	}
	float RatioToDB(double ratio) {
		return (float)(20 * Math.Log10(ratio));
	}

	double timeSinceSound = float.PositiveInfinity;
	const double SFX_DEBOUNCE = 0.2f;

	public void _on_value_changed(double value) {
		AudioServer.SetBusVolumeDb(
			busIndex,
			RatioToDB(value)
		);

		if (PlaySFX && timeSinceSound > SFX_DEBOUNCE) {
			BubbleGame.Game.Audio.Shoot();
			timeSinceSound = 0;
		}
	}

	public override void _Process(double delta) {
		if (PlaySFX) {
			timeSinceSound += delta;
		}
	}


}

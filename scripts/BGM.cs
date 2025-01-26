using Godot;
using System;

public partial class BGM : AudioStreamPlayer {
	[Export] public BGMConfig[] BGMConfigs { get; private set; } = new BGMConfig[0];

	[Signal] public delegate void OnBGMChangeEventHandler(BGMConfig config);

	double timeSinceStart = 0;
	bool isChangingTracks = false;
	BGMConfig currentConfig = null;

	public override void _Ready() {
		base._Ready();
		currentConfig = PlayRandom();
	}

	public override void _Process(double delta) {
		base._Process(delta);
		timeSinceStart += delta;
		if (timeSinceStart >= currentConfig.FadeAfter && !isChangingTracks) {
			PlayRandom();
		}
	}

	const float FADE_OUT_TIME = 3f;
	const float MIN_DB = -80f;

	public BGMConfig PlayRandom() {
		GD.Print("HERE!");
		isChangingTracks = true;
		var config = BGMConfigs[GD.RandRange(0, BGMConfigs.Length - 1)];
		while (Stream == config.AudioStream) {
			// Pick a new song
			config = BGMConfigs[GD.RandRange(0, BGMConfigs.Length - 1)];
		}
		GD.Print($"Playing {config.Title} by {config.Composer}");
		var tween = GetTree().CreateTween();
		// Fade out the current track
		tween.TweenProperty(this, "volume_db", MIN_DB, FADE_OUT_TIME);
		tween.TweenCallback(Callable.From(() => {
			Stream = config.AudioStream;
			VolumeDb = 0;
			Play();
			timeSinceStart = 0f;
			isChangingTracks = false;
		}));

		return config;
	}
}

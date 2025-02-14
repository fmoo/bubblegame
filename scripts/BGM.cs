using Godot;
using System;

public partial class BGM : AudioStreamPlayer {
	[Export] public Resource[] BGMConfigs { get; private set; } = new Resource[0];

	[Signal] public delegate void OnBGMChangeEventHandler(Resource config);

	double timeSinceStart = 0;
	bool isChangingTracks = false;
	Resource currentConfig = null;

	public override void _Ready() {
		base._Ready();
		currentConfig = PlayRandom();
	}

	public override void _Process(double delta) {
		base._Process(delta);
		timeSinceStart += delta;
		if (timeSinceStart >= currentConfig.Get("FadeAfter").AsDouble() && !isChangingTracks) {
			PlayRandom();
		}
	}

	const float FADE_OUT_TIME = 4f;
	const float MIN_DB = -80f;

	public Resource PlayRandom() {
		var fadeOutTime = Stream != null ? FADE_OUT_TIME : 0;
		isChangingTracks = true;
		var config = BGMConfigs[GD.RandRange(0, BGMConfigs.Length - 1)];
		while (Stream == config.Get("_AudioStream").As<AudioStream>()) {
			// Pick a new song
			config = BGMConfigs[GD.RandRange(0, BGMConfigs.Length - 1)];
		}
		GD.Print($"Playing {config.Get("Title").AsString()} by {config.Get("Composer").AsString()}");
		var tween = GetTree().CreateTween();
		// Fade out the current track
		tween.TweenProperty(this, "volume_db", MIN_DB, fadeOutTime);
		tween.TweenCallback(Callable.From(() => {
			Stream = config.Get("_AudioStream").As<AudioStream>();
			VolumeDb = 0;
			Play();
			timeSinceStart = 0f;
			isChangingTracks = false;
		}));

		return config;
	}
}

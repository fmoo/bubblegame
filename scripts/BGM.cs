using Godot;
using System;

public partial class BGM : AudioStreamPlayer {
	[Export] public BGMConfig[] BGMConfigs { get; private set; } = new BGMConfig[0];

	public BGMConfig PlayRandom() {
		var config = BGMConfigs[GD.RandRange(0, BGMConfigs.Length - 1)];
		Stream = config.AudioStream;
		Play();
		return config;
	}
}

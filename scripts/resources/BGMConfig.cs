using Godot;
using System;

[GlobalClass]
public partial class BGMConfig : Resource {
    [Export] public AudioStream AudioStream { get; private set; }
    [Export] public string Title { get; private set; }
    [Export] public string Composer { get; private set; }
    [Export] public string Link { get; private set; }
    [Export] public float FadeAfter { get; private set; } = 120f;
}

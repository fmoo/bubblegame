using Godot;

[GlobalClass]
public partial class GameplayConfig : Resource {
    [Export] public string Name { get; private set; } = "Normal";
    [Export] public string Description { get; private set; } = "A normal way to play.";

    [Export] public BubbleConfig[] Bubbles { get; private set; } = new BubbleConfig[0];
    [Export] public bool VillainBubbleColorChanges { get; private set; } = false;

    [Export] public bool TimerTicks { get; private set; } = true;
    [Export] public int MinMatchSize { get; private set; } = 3;
    [Export] public int ShrinkBubblePops { get; private set; } = 4;
    [Export] public int GrowBubbleShots { get; private set; } = 5;
}
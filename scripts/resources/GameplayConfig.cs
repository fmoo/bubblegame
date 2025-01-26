using Godot;

[GlobalClass]
public partial class GameplayConfig : Resource {
    [Export] public string Name { get; private set; } = "Normal";
    [Export] public string Description { get; private set; } = "A normal way to play.";

    [Export] public BubbleConfig[] Bubbles { get; private set; } = new BubbleConfig[0];
    [Export] public bool VillainBubbleColorChanges { get; private set; } = false;

    [Export] public double ChainDuration { get; private set; } = 4f;
    [Export] public bool TimerTicks { get; private set; } = true;
    [Export] public int MinMatchSize { get; private set; } = 3;
    [Export] public int ShrinkBubblePops { get; private set; } = 4;
    [Export] public int GrowBubbleShots { get; private set; } = 5;
    // Pressure grows at a rate of 1/GrowBubbleShots every BasePresureDuration seconds
    [Export] public double BasePressureDuration { get; private set; } = 6f;
    [Export] public double PressureDecayMultiplier { get; private set; } = 0.99f;
    [Export] public bool ChainsStallPressure { get; private set; } = true;
}
using Godot;

[GlobalClass]
public partial class GameplayConfig : Resource {
    [Export] string Name = "Normal";
    [Export] string Description = "A normal way to play.";

    [Export] BubbleConfig[] Bubbles;

    [Export] bool TimerTicks { get; set; } = true;
    [Export] int MinMatchSize { get; set; } = 3;
    [Export] int ShrinkBubblePops { get; set; } = 4;
    [Export] int GrowBubbleShots { get; set; } = 5;


}
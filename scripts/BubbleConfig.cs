using Godot;

[GlobalClass]
public partial class BubbleConfig : Resource {
    [Export] public Color BubbleColor { get; private set; } = new Color(1, 1, 1);
    [Export] public Texture2D PlayerBubbleTexture { get; private set; }
}

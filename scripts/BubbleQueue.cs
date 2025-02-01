using System.Collections.Generic;
using Godot;

public partial class BubbleQueue : Node2D {
	[Export] public BubbleSprite[] bubbleRenders;
	List<BubbleConfig> colorQueue = new();
	public override void _Ready() {
		base._Ready();
		Reset();
	}

	public void ShowBubbles() {
		foreach (var bubble in bubbleRenders) {
			bubble.Show();
		}
	}

	public void HideBubbles() {
		foreach (var bubble in bubbleRenders) {
			bubble.Hide();
		}
	}

	public void StartTween() {
		var tween = GetTree().CreateTween().SetParallel(true);
		BubbleSprite[] clones = new BubbleSprite[bubbleRenders.Length - 1];
		for (int i = 1; i < bubbleRenders.Length; i++) {
			// Copy the bubble
			var bubbleClone = bubbleRenders[i].Duplicate() as BubbleSprite;
			clones[i - 1] = bubbleClone;
			AddChild(bubbleClone);
			bubbleClone.GlobalPosition = bubbleRenders[i].GlobalPosition;
			if (i == 1) {
				bubbleClone.SetTarget(bubbleRenders[i - 1]);
				tween.TweenProperty(bubbleClone, "TargetRatio", 1f, BubbleGun.COOLDOWN);
			} else {
				tween.TweenProperty(bubbleClone, "global_position", bubbleRenders[i - 1].GlobalPosition, BubbleGun.COOLDOWN);
			}
		}
		HideBubbles();
		tween.Chain().TweenCallback(Callable.From(() => {
			RefreshRender();
			ShowBubbles();
			foreach (var clone in clones) {
				RemoveChild(clone);
				clone.QueueFree();
			}
		}));
	}

	public BubbleConfig DequeueColor() {
		var result = colorQueue[0];
		colorQueue.RemoveAt(0);
		colorQueue.Add(BubbleGame.Game.PickColor());
		// RefreshRender();
		StartTween();
		return result; 
	}

	void RefreshRender() {
		for (int i = 0; i < bubbleRenders.Length; i++) {
			bubbleRenders[i].SetConfig(colorQueue[i]);
		}
	}

	public void Reset() {
		colorQueue.Clear();
		
		if(BubbleGame.Game.TitleMode) {
			for (int i = 0; i < bubbleRenders.Length; i++) {
				var color = BubbleGame.Game.GameplayConfig.Bubbles[0];
				colorQueue.Add(color);
			}
		}
		else {
			for (int i = 0; i < bubbleRenders.Length; i++) {
				var color = BubbleGame.Game.PickColor();
				colorQueue.Add(color);
			}
		}
		
		RefreshRender();
	}
}

using System.Collections.Generic;
using Godot;

public partial class BubbleQueue : Node2D {
	[Export] public Sprite2D[] bubbleRenders;
	[Export] Node2D queueEjector;
	[Export] PathFollow2D reloadPath;
	[Export] PathFollow2D gunPath;
	[Export] double ejectorPathRatio = 0.0995;
	List<Resource> colorQueue = new();
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

	const double COOLDOWN = 0.3;

	public void StartTween() {
		var tween = GetTree().CreateTween().SetParallel(true);
		Sprite2D[] clones = new Sprite2D[bubbleRenders.Length - 1];
		for (int i = 1; i < bubbleRenders.Length; i++) {
			// Copy the bubble
			var bubbleClone = bubbleRenders[i].Duplicate() as Sprite2D;
			clones[i - 1] = bubbleClone;
			AddChild(bubbleClone);
			bubbleClone.GlobalPosition = bubbleRenders[i].GlobalPosition;
			if (i == 1) {
				var tween2 = GetTree().CreateTween();
				// For the first 1/4 of time, we tween bubble directly to the "ejector" position
				tween2.TweenProperty(bubbleClone, "global_position", queueEjector.GlobalPosition, COOLDOWN / 4.0);
				// For the remaining 3/4 of the time, we tween the bubble along the reloadPath
				tween2.TweenMethod(Callable.From<float>((value) => {
					// Hack to make the bubble go the short way around the path
					var targetRatio = gunPath.ProgressRatio;
					if (targetRatio > ejectorPathRatio + 0.5) {
						targetRatio -= 1.0f;
					}
					reloadPath.ProgressRatio = (float)Mathf.Lerp(ejectorPathRatio, targetRatio, value);
					bubbleClone.GlobalPosition = reloadPath.GlobalPosition;

				}), 0.0, 1.0, COOLDOWN * 3.0 / 4.0);
			} else {
				tween.TweenProperty(bubbleClone, "global_position", bubbleRenders[i - 1].GlobalPosition, COOLDOWN);
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

	public Resource DequeueColor() {
		var result = colorQueue[0];
		colorQueue.RemoveAt(0);
		colorQueue.Add(BubbleGame.Game.PickColor());
		// RefreshRender();
		StartTween();
		return result;
	}

	void RefreshRender() {
		for (int i = 0; i < bubbleRenders.Length; i++) {
			bubbleRenders[i].Call("set_config", colorQueue[i]);
		}
	}

	public void Reset() {
		colorQueue.Clear();

		if (BubbleGame.Game.TitleMode) {
			for (int i = 0; i < bubbleRenders.Length; i++) {
				var color = BubbleGame.Game.GameplayConfig.Bubbles[0];
				colorQueue.Add(color);
			}
		} else {
			for (int i = 0; i < bubbleRenders.Length; i++) {
				var color = BubbleGame.Game.PickColor();
				colorQueue.Add(color);
			}
		}

		RefreshRender();
	}
}

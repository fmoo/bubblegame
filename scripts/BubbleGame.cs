using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BubbleGame : Node2D {
	[Export] public Audio Audio { get; private set; }
	[Export] public BubbleQueue BubbleQueue { get; private set; }
	[Export] public GameplayConfig GameplayConfig { get; private set; }
	[Export] public Node2D Springs;
	[Export] public Node2D Bubbles;
	[Export] public VillainBubble VillainBubble { get; private set; }
	[Export] public BubbleGun Player { get; private set; }
	[Export] public bool DebugMode { get; private set; } = false;
	[Export] public bool TitleMode { get; private set; } = false;
	[Export] PackedScene PinJointTemplate;
	[Export] PackedScene VillainPinJointTemplate;

	public int Score { get; private set; } = 0;
	[Signal] public delegate void ScoreChangedEventHandler(int score);
	[Signal] public delegate void TimeElapsedEventHandler(int seconds);
	[Signal] public delegate void ChainChangedEventHandler(int chain);
	public int GainPoints(int points) {
		Score += points;
		EmitSignal(SignalName.ScoreChanged, Score);
		return Score;
	}

	BubbleGame() {
		Game = this;
	}

	public override void _Ready() {
		base._Ready();
		if (GameOverPanel != null) {
			GameOverPanel.Visible = false;
		}
		MaybePickNewVillainBubbleColor();
		Reset();
	}

	public void RegisterBubble(Bubble bubble) {
		Bubbles.AddChild(bubble);
	}

	public void DestroyBubble(Bubble bubble) {
		bool canShrink = true;
		if (GameplayConfig.VillainBubbleColorChanges && bubble.Config != VillainBubble.Config) {
			canShrink = false;
		}

		if (bubble != null && !bubble.IsQueuedForDeletion() && canShrink) {
			GD.Print($"Pop: Pressure decrease by {1.0 / GameplayConfig.ShrinkBubblePops}");
			Pressure -= 1.0 / GameplayConfig.ShrinkBubblePops;
		}
		GD.Print($"Destroying bubble {bubble}");
		bubble.StartDestroy();
	}

	void MaybePickNewVillainBubbleColor() {
		// Pick a new color for the villain bubble
		if (!GameplayConfig.VillainBubbleColorChanges) return;
		var currentColor = VillainBubble.Config;
		var newColor = PickColor();
		while (newColor == currentColor) {
			newColor = PickColor();
		}
		VillainBubble.SetConfig(newColor);
	}

	public void RegisterLink(Bubble bubble, Node2D join) {
	}

	public PinJoint2D LinkBubbles(Bubble bubble1, Bubble bubble2) {
		var joint = PinJointTemplate.Instantiate<PinJoint2D>();
		bubble1.AddChild(joint);
		// Springs.AddChild(joint);
		joint.NodeA = bubble1.GetPath();
		joint.NodeB = bubble2.GetPath();
		joint.GlobalPosition = bubble1.GlobalPosition;
		// joint.GlobalPosition = (bubble1.GlobalPosition + bubble2.GlobalPosition) / 2;
		joint.GlobalRotation = bubble1.GlobalPosition.AngleToPoint(bubble2.GlobalPosition);
		return joint;
	}
	public RemoteTransform2D LinkToVillainBubbleRemoteTransform(VillainBubble villainBubble, Bubble bubble2) {
		GD.Print($"Villain Bubble at {villainBubble.GlobalPosition} linked to Bubble at {bubble2.GlobalPosition}");

		// Get the point of intersection by moving from bubble2 towards bubble1 by bubble2.Radius
		var direction = (villainBubble.GlobalPosition - bubble2.GlobalPosition).Normalized();
		var intersection = bubble2.GlobalPosition + direction * bubble2.Radius;

		// Create the sync point for the bubble at the intersectionPoint,
		// making it a child of the villainBubble.
		var joint = new RemoteTransform2D();
		villainBubble.AddChild(joint);
		joint.GlobalPosition = bubble2.GlobalPosition;
		joint.GlobalRotation = bubble2.GlobalRotation;
		joint.UseGlobalCoordinates = true;
		joint.UpdateRotation = false;
		joint.UpdateScale = false;
		bubble2.SetDeferred("freeze", true);

		joint.RemotePath = bubble2.GetPath();

		RegisterLink(bubble2, joint);
		return joint;
	}

	public PinJoint2D LinkToVillainBubblePinJoint(VillainBubble villainBubble, Bubble bubble) {
		GD.Print($"Villain Bubble at {villainBubble.GlobalPosition} linked to Bubble at {bubble.GlobalPosition}");
		bubble.HasVillainAnchor = true;

		var joint = PinJointTemplate.Instantiate<PinJoint2D>();
		villainBubble.AddChild(joint);
		joint.NodeA = villainBubble.GetPath();
		joint.NodeB = bubble.GetPath();
		joint.GlobalPosition = villainBubble.GlobalPosition;
		joint.GlobalRotation = villainBubble.GlobalPosition.AngleToPoint(bubble.GlobalPosition);

		RegisterLink(bubble, joint);
		return joint;
	}

	public PinJoint2D LinkToMenuBubblePinJoint(MenuBubble menuBubble, Bubble bubble) {
		GD.Print($"Villain Bubble at {menuBubble.GlobalPosition} linked to Bubble at {bubble.GlobalPosition}");
		bubble.HasVillainAnchor = true;
		bubble.HasMenuButtonAnchor = true;
		bubble.MenuButtonAnchor = menuBubble;
		var joint = PinJointTemplate.Instantiate<PinJoint2D>();
		menuBubble.AddChild(joint);
		joint.NodeA = menuBubble.GetPath();
		joint.NodeB = bubble.GetPath();
		joint.GlobalPosition = menuBubble.GlobalPosition;
		joint.GlobalRotation = menuBubble.GlobalPosition.AngleToPoint(bubble.GlobalPosition);

		RegisterLink(bubble, joint);
		return joint;
	}

	public BubbleConfig PickColor() {
		var index = GD.RandRange(0, GameplayConfig.Bubbles.Length - 1);
		GD.Print(index);
		return GameplayConfig.Bubbles[index];
	}

	public double ChainTimeRemaining { get; private set; } = 0f;
	int currentChain = 1;

	public void MaybePopBubbles(Bubble bubble) {
		if (bubble.IsQueuedForDeletion()) return;

		var maybePop = bubble.WalkSameColorNeighbors();
		if (maybePop.Count < GameplayConfig.MinMatchSize) return;

		if (!TitleMode) {
			var pointsGained = 50;
			// 10x the points for every extra bubble popped simultaneously
			pointsGained *= (int)Math.Pow(10, maybePop.Count - GameplayConfig.MinMatchSize);
			// Scale points based on the current VillainBubbleMultiplier
			GD.Print($"Gaining Points: Base={pointsGained}  SizeMul={VillainBubble.ScoreMultiplier}  ChainMul={currentChain}  SpeedMul={DifficultyMultiplier}");
			pointsGained = (int)(pointsGained * VillainBubble.ScoreMultiplier * currentChain * DifficultyMultiplier);
			GainPoints(pointsGained);
			currentChain += 1;
			EmitSignal(SignalName.ChainChanged, currentChain);
			ChainTimeRemaining = GameplayConfig.ChainDuration;
		}
		HashSet<Bubble> maybeFall = new();

		var WasAnchored = false;
		var AnchoredMenuBubble = new MenuBubble();
		foreach (var p in maybePop) {
			foreach (var f in p.Neighbors) {
				if (!maybePop.Contains(f)) {
					maybeFall.Add(f);
				}
			}
			if (p.IsAnchored) {
				WasAnchored = true;
				AnchoredMenuBubble = p.MenuButtonAnchor;
			}
			DestroyBubble(p);
		}

		if (TitleMode) {
			if (WasAnchored) {
				Call(AnchoredMenuBubble.GameFunction);
			}
			return;
		}
		foreach (var f in maybeFall) {
			if (f.IsAnchored) continue;
			f.ChainMoveTowards(VillainBubble.GlobalPosition);
		}
	}

	public void DebugTestFall() {
		foreach (var bubble in Bubbles.GetChildren().Cast<Bubble>()) {
			if (bubble.IsAnchored) continue;
			bubble.ChainMoveTowards(VillainBubble.GlobalPosition);
		}
	}

	int lastSecond = 0;
	double timeElapsed = 0;
	public double Pressure { get; private set; } = 0;
	double currentPressureDuration = 0;
	double DifficultyMultiplier => Mathf.Pow(10, (GameplayConfig.BasePressureDuration / currentPressureDuration) - 1);
	// float DifficultyMultiplier => 1f;
	public override void _Process(double delta) {
		base._Process(delta);
		if (Input.IsActionJustReleased("debug_physics")) {
			Audio.BGM.PlayRandom();
			// VillainBubble.SetConfig(PickColor());
		}
		if (TitleMode) {
			return;
		}
		if (GameplayConfig.TimerTicks) {
			if (GameplayConfig.ChainsStallPressure && currentChain > 1) {

			} else {
				Pressure += 1f / GameplayConfig.GrowBubbleShots / currentPressureDuration * delta;
				// GD.Print($"Tick: Pressure increase by {1f / GameplayConfig.GrowBubbleShots / currentPressureDuration * delta} -> {Pressure}");
			}
		}
		timeElapsed += (float)delta;
		if ((int)timeElapsed != lastSecond) {
			lastSecond = (int)timeElapsed;
			EmitSignal(SignalName.TimeElapsed, lastSecond);
		}

		if (Pressure > 1.0) {
			GD.Print($"Pressure {Pressure} over threshold! Growing!");
			VillainBubble.Grow();
			Pressure -= 1.0;
		} else if (Pressure < 0.0) {
			GD.Print($"Pressure {Pressure} under threshold! shrinking!");
			VillainBubble.Shrink();
			Pressure += 1.0;
			currentPressureDuration *= GameplayConfig.PressureDecayMultiplier;
			MaybePickNewVillainBubbleColor();
		}

		if (currentChain > 1) {
			ChainTimeRemaining -= delta;
			if (ChainTimeRemaining <= 0) {
				currentChain = 1;
				EmitSignal(SignalName.ChainChanged, currentChain);
				ChainTimeRemaining = 0;
			}
		}
	}

	[Export] Control GameOverPanel;
	public void GameOver() {
		Audio.GameOver();
		GD.Print("Game Over");
		if (GameOverPanel != null) {
			GameOverPanel.Visible = true;
		}
		GetTree().Paused = true;
	}

	public void _on_play_again_pressed() {
		Reset();
		GameOverPanel.Visible = false;
		GetTree().Paused = false;
	}

	public void Reset() {
		timeElapsed = 0;
		currentPressureDuration = GameplayConfig.BasePressureDuration;
		numShots = 0;
		Pressure = 0;
		if (!TitleMode) {
			foreach (var bubble in Bubbles.GetChildren()) {
				bubble.QueueFree();
			}
			foreach (var spring in Springs.GetChildren()) {
				spring.QueueFree();
			}
			VillainBubble.Reset();
			MaybePickNewVillainBubbleColor();
		}

		Player.Reset();
		BubbleQueue.Reset();
		Score = 0;
		EmitSignal(SignalName.ScoreChanged, Score);
	}

	public async void PlayGame() {
		GD.Print("PLAYGAME");
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile("res://scenes/bubble_game.tscn");
	}

	int numShots = 0;
	public void _on_shoot_event() {
		if (TitleMode) {
			return;
		}
		GD.Print($"Shoot: Pressure increase by {1.0 / GameplayConfig.GrowBubbleShots}");
		Pressure += 1.0 / GameplayConfig.GrowBubbleShots;
	}

	[Export] PackedScene BubbleSprite;
	public void SpawnBubblePop(Vector2 globalPosition, Texture2D texture) {
		var bubblePop = BubbleSprite.Instantiate<BubbleSprite>();
		AddChild(bubblePop);
		bubblePop.GlobalPosition = globalPosition;
		bubblePop.Texture = texture;
		bubblePop.PlayAnimation("pop");
	}


	public void _on_main_menu_pressed() {
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/title.tscn");
		// Unpause
	}

	public static BubbleGame Game { get; private set; }
}

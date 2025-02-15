using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class BubbleGame : Node2D {
	[Export] public Node Audio { get; private set; }
	[Export] public Node2D BubbleQueue { get; private set; }
	[Export] public Resource _GameplayConfig { get; private set; }
	[Export] public Node2D Springs;
	[Export] public Node2D Bubbles;
	[Export] public RigidBody2D VillainBubble { get; private set; }
	[Export] public Node2D Player { get; private set; }
	[Export] public bool DebugMode { get; private set; } = false;
	[Export] public bool TitleMode { get; private set; } = false;
	[Export] PackedScene PinJointTemplate;
	[Export] PackedScene GrooveJointTemplate;
	[Export] PackedScene VillainPinJointTemplate;
	[Export] Label renderHighScore;

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

	public void RegisterBubble(RigidBody2D bubble) {
		Bubbles.AddChild(bubble);
	}

	public void DestroyBubble(RigidBody2D bubble, float multiplier) {
		bool canShrink = true;
		if (_GameplayConfig.Get("VillainBubbleColorChanges").AsBool() && bubble.Get("config").Obj != VillainBubble.Get("config").Obj) {
			canShrink = false;
		}

		if (bubble != null && !bubble.IsQueuedForDeletion() && canShrink) {
			GD.Print($"Pop: Pressure decrease by {1.0 / _GameplayConfig.Get("ShrinkBubblePops").AsDouble()}");
			Pressure -= 1.0 * multiplier / _GameplayConfig.Get("ShrinkBubblePops").AsDouble();
		}
		GD.Print($"Destroying bubble {bubble}");
		bubble.Call("start_destroy");
	}

	void MaybePickNewVillainBubbleColor() {
		// Pick a new color for the villain bubble

		if (!_GameplayConfig.Get("VillainBubbleColorChanges").AsBool()) return;
		var currentColor = VillainBubble.Get("config").As<Resource>();
		var newColor = PickColor();
		while (newColor == currentColor) {
			newColor = PickColor();
		}
		VillainBubble.Call("set_config", newColor);
	}

	public void RegisterLink(RigidBody2D bubble, Node2D join) {
	}

	public PinJoint2D LinkBubbles(RigidBody2D bubble1, RigidBody2D bubble2) {
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
	public RemoteTransform2D LinkToVillainBubbleRemoteTransform(RigidBody2D villainBubble, RigidBody2D bubble2) {
		GD.Print($"Villain Bubble at {villainBubble.GlobalPosition} linked to Bubble at {bubble2.GlobalPosition}");

		// Get the point of intersection by moving from bubble2 towards bubble1 by bubble2.Radius

		var direction = (villainBubble.GlobalPosition - bubble2.GlobalPosition).Normalized();
		var intersection = bubble2.GlobalPosition + direction * bubble2.Call("radius").As<float>();

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

	public PinJoint2D LinkToVillainBubblePinJoint(RigidBody2D villainBubble, RigidBody2D bubble) {
		GD.Print($"Villain Bubble at {villainBubble.GlobalPosition} linked to Bubble at {bubble.GlobalPosition}");
		bubble.Set("has_villain_anchor", true);

		var joint = PinJointTemplate.Instantiate<PinJoint2D>();
		villainBubble.AddChild(joint);
		joint.NodeA = villainBubble.GetPath();
		joint.NodeB = bubble.GetPath();
		joint.GlobalPosition = villainBubble.GlobalPosition;
		joint.GlobalRotation = villainBubble.GlobalPosition.AngleToPoint(bubble.GlobalPosition);

		RegisterLink(bubble, joint);
		return joint;
	}

	public void LinkToMenuBubble(RigidBody2D menuBubble, RigidBody2D bubble) {
		// If the position of the bubble is beyond the width of the bubble, use a pin joint,
		// otherwise use a groove joint.
		if (bubble.GlobalPosition.X > menuBubble.GlobalPosition.X + menuBubble.Call("get_rectangle_shape").As<RectangleShape2D>().Size.X / 2 ||
			bubble.GlobalPosition.X < menuBubble.GlobalPosition.X - menuBubble.Call("get_rectangle_shape").As<RectangleShape2D>().Size.X / 2
		) {
			bubble.Set("lock_position", bubble.GlobalPosition);
		} else {
			LinkToMenuBubbleGrooveJoint(menuBubble, bubble);
		}
		bubble.Set("has_villain_anchor", true);
		bubble.Set("has_menu_button_anchor", true);
		bubble.Set("menu_button_anchor", menuBubble);
	}

	public PinJoint2D LinkToMenuBubblePinJoint(RigidBody2D menuBubble, RigidBody2D bubble) {
		GD.Print($"Menu Bubble at {menuBubble.GlobalPosition} linked to Bubble at {bubble.GlobalPosition}");
		bubble.Set("has_villain_anchor", true);
		bubble.Set("has_menu_button_anchor", true);
		var joint = PinJointTemplate.Instantiate<PinJoint2D>();
		menuBubble.AddChild(joint);
		joint.NodeA = menuBubble.GetPath();
		joint.NodeB = bubble.GetPath();
		joint.GlobalPosition = menuBubble.GlobalPosition;
		joint.GlobalRotation = menuBubble.GlobalPosition.AngleToPoint(bubble.GlobalPosition);

		RegisterLink(bubble, joint);
		return joint;
	}
	public GrooveJoint2D LinkToMenuBubbleGrooveJoint(RigidBody2D menuBubble, RigidBody2D bubble) {
		GD.Print($"Menu Bubble at {menuBubble.GlobalPosition} linked to Bubble at {bubble.GlobalPosition}");
		bubble.Set("has_villain_anchor", true);
		bubble.Set("has_menu_button_anchor", true);
		bubble.Set("menu_button_anchor", menuBubble);
		var joint = GrooveJointTemplate.Instantiate<GrooveJoint2D>();
		menuBubble.AddChild(joint);
		joint.SetDeferred("node_a", menuBubble.GetPath());
		joint.SetDeferred("node_b", bubble.GetPath());
		joint.GlobalPosition = new Vector2(
			menuBubble.GlobalPosition.X - menuBubble.Call("get_rectangle_shape").As<RectangleShape2D>().Size.X / 2,
			bubble.GlobalPosition.Y
		);
		joint.Length = menuBubble.Call("get_rectangle_shape").As<RectangleShape2D>().Size.X;
		joint.InitialOffset = joint.Length - (menuBubble.GlobalPosition.X - bubble.GlobalPosition.X + (menuBubble.Call("get_rectangle_shape").As<RectangleShape2D>().Size.X / 2));
		joint.GlobalRotationDegrees = -90;

		RegisterLink(bubble, joint);
		return joint;
	}

	public Resource PickColor() {
		var index = GD.RandRange(0, _GameplayConfig.Get("Bubbles").AsGodotObjectArray<Resource>().Length - 1);
		GD.Print(index);
		return _GameplayConfig.Get("Bubbles").AsGodotObjectArray<Resource>()[index];
	}

	public double ChainTimeRemaining { get; private set; } = 0f;
	int currentChain = 1;

	public void MaybePopBubbles(RigidBody2D bubble) {
		if (bubble.IsQueuedForDeletion()) return;

		var maybePop = bubble.Call("walk_same_color_neighbors").As<Godot.Collections.Array>();
		if (maybePop.Count() < _GameplayConfig.Get("MinMatchSize").AsInt64()) return;

		if (!TitleMode) {
			var pointsGained = 50;
			// 10x the points for every extra bubble popped simultaneously
			pointsGained *= (int)Math.Pow(10, maybePop.Count() - _GameplayConfig.Get("MinMatchSize").AsInt64());
			// Scale points based on the current VillainBubbleMultiplier
			GD.Print($"Gaining Points: Base={pointsGained}  SizeMul={VillainBubble.Call("get_score_multiplier").As<float>()}  ChainMul={currentChain}  SpeedMul={DifficultyMultiplier}");
			pointsGained = (int)(pointsGained * VillainBubble.Call("get_score_multiplier").As<float>() * currentChain * DifficultyMultiplier);
			GainPoints(pointsGained);
			currentChain += 1;
			EmitSignal(SignalName.ChainChanged, currentChain);
			ChainTimeRemaining = _GameplayConfig.Get("ChainDuration").AsDouble();
		}
		Godot.Collections.Array maybeFall = new();

		var WasAnchored = false;
		var AnchoredMenuBubble = new RigidBody2D();
		foreach (RigidBody2D p in maybePop) {
			foreach (var f in p.Get("neighbors").As<Godot.Collections.Array>()) {
				if (!maybePop.Contains(f)) {
					maybeFall.Append(f);
				}
			}
			if (p.Call("is_anchored").AsBool()) {
				WasAnchored = true;
				AnchoredMenuBubble = p.Get("menu_button_anchor").As<RigidBody2D>();
			}
			//for each ball popped over min match size, multiply shrink amount
			var ShrinkMultiplier = maybePop.Count - (_GameplayConfig.Get("MinMatchSize").AsInt64() - 1f);
			DestroyBubble(p, ShrinkMultiplier);
		}

		if (TitleMode) {
			if (WasAnchored) {
				Call(AnchoredMenuBubble.Get("game_function").AsString());
			}
			return;
		}
		foreach (RigidBody2D f in maybeFall) {
			if (f.Call("is_anchored").AsBool()) continue;
			f.Call("chain_move_towards", VillainBubble.GlobalPosition);
		}
	}

	public void DebugTestFall() {
		foreach (var bubble in Bubbles.GetChildren().Cast<RigidBody2D>()) {
			if (bubble.Call("is_anchored").AsBool()) continue;
			bubble.Call("chain_move_towards", VillainBubble.GlobalPosition);
		}
	}

	int lastSecond = 0;
	double timeElapsed = 0;
	public double Pressure { get; private set; } = 0;
	double currentPressureDuration = 0;
	double DifficultyMultiplier => Mathf.Pow(2, (_GameplayConfig.Get("BasePressureDuration").AsDouble() / currentPressureDuration) - 1);
	// float DifficultyMultiplier => 1f;

	public override void _Process(double delta) {
		base._Process(delta);
		if (TitleMode) {
			return;
		}
		if (_GameplayConfig.Get("TimerTicks").AsBool()) {
			if (_GameplayConfig.Get("ChainsStallPressure").AsBool() && currentChain > 1) {

			} else {
				Pressure += 1f / _GameplayConfig.Get("GrowBubbleShots").AsDouble() / currentPressureDuration * delta;
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
			VillainBubble.Call("grow");
			Pressure -= 1.0;
		} else if (Pressure < 0.0) {
			GD.Print($"Pressure {Pressure} under threshold! shrinking!");
			VillainBubble.Call("shrink");
			Pressure += 1.0;
			currentPressureDuration *= _GameplayConfig.Get("PressureDecayMultiplier").AsDouble();
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
		Audio.Call("GameOver");
		GD.Print("Game Over");
		if (renderHighScore != null) renderHighScore.Call("SaveHighScore");
		PauseWithPanel(GameOverPanel);
	}

	Control VisiblePausePanel = null;
	public void PauseWithPanel(Control panel) {
		if (GetTree().Paused) return;
		GetTree().Paused = true;
		VisiblePausePanel = panel;
		panel.Visible = true;
	}
	public void Unpause() {
		if (!GetTree().Paused) return;
		GetTree().Paused = false;
		if (VisiblePausePanel != null) {
			VisiblePausePanel.Visible = false;
			VisiblePausePanel = null;
		}
	}

	public void _on_play_again_pressed() {
		Reset();
		Unpause();
	}

	public void Reset() {
		timeElapsed = 0;
		currentPressureDuration = _GameplayConfig.Get("BasePressureDuration").AsDouble();
		numShots = 0;
		Pressure = 0;
		if (!TitleMode) {
			foreach (var bubble in Bubbles.GetChildren()) {
				bubble.QueueFree();
			}
			foreach (var spring in Springs.GetChildren()) {
				spring.QueueFree();
			}
			VillainBubble.Call("reset");
			MaybePickNewVillainBubbleColor();
		}

		Player.Call("Reset");
		BubbleQueue.Call("Reset");
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
		GD.Print($"Shoot: Pressure increase by {1.0 / _GameplayConfig.Get("GrowBubbleShots").AsDouble()}");
		Pressure += 1.0 / _GameplayConfig.Get("GrowBubbleShots").AsDouble();
	}

	[Export] PackedScene BubbleSprite;
	public void SpawnBubblePop(Vector2 globalPosition, Texture2D texture) {
		var bubblePop = BubbleSprite.Instantiate<Sprite2D>();
		AddChild(bubblePop);
		bubblePop.GlobalPosition = globalPosition;
		bubblePop.Texture = texture;
		bubblePop.Call("PlayAnimation", "pop");
	}

	public void Quit() {
		GetTree().Quit();
	}

	public void _on_main_menu_pressed() {
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/title.tscn");
		// Unpause

	}

	public static BubbleGame Game { get; private set; }
}

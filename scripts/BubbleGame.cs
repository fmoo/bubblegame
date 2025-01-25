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

    [Export] PackedScene PinJointTemplate;
    [Export] PackedScene VillainPinJointTemplate;

    public int Score { get; private set; } = 0;
    [Signal] public delegate void ScoreChangedEventHandler(int score);
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
    }

    public void RegisterBubble(Bubble bubble) {
        Bubbles.AddChild(bubble);
    }

    int poppedBubbles = 0;
    public void DestroyBubble(Bubble bubble) {
        GD.Print($"Destroying bubble {bubble}");
        if (bubble != null && !bubble.IsQueuedForDeletion()) {
            poppedBubbles++;
            while (poppedBubbles >= GameplayConfig.ShrinkBubblePops) {
                poppedBubbles -= GameplayConfig.ShrinkBubblePops;
                VillainBubble.Shrink();
                incrementBadDuration *= 0.99f;
            }
        }
        bubble.StartDestroy();
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

    public BubbleConfig PickColor() {
        var index = GD.RandRange(0, GameplayConfig.Bubbles.Length - 1);
        GD.Print(index);
        return GameplayConfig.Bubbles[index];
    }

    float chainTimer = 0f;
    int currentChain = 1;

    public void MaybePopBubbles(Bubble bubble) {
        if (bubble.IsQueuedForDeletion()) return;

        var maybePop = bubble.WalkSameColorNeighbors();
        if (maybePop.Count < GameplayConfig.MinMatchSize) return;

        var pointsGained = 50;
        // 10x the points for every extra bubble popped simultaneously
        pointsGained *= (int)Math.Pow(10, maybePop.Count - GameplayConfig.MinMatchSize);
        // Scale points based on the current VillainBubbleMultiplier
        GD.Print($"Gaining Points: Base={pointsGained}  SizeMul={VillainBubble.ScoreMultiplier}  ChainMul={currentChain}  SpeedMul={DifficultyMultiplier}");
        pointsGained = (int)(pointsGained * VillainBubble.ScoreMultiplier * currentChain * DifficultyMultiplier);
        GainPoints(pointsGained);
        currentChain = Mathf.Clamp(currentChain + 1, 1, 8);
        chainTimer = 1f;

        HashSet<Bubble> maybeFall = new();
        foreach (var p in maybePop) {
            foreach (var f in p.Neighbors) {
                if (!maybePop.Contains(f)) {
                    maybeFall.Add(f);
                }
            }
            DestroyBubble(p);
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

    float timeElapsed = 0;
    const float DEFAULT_INCREMENT_BAD_DURATION = 6f;
    float incrementBadDuration = DEFAULT_INCREMENT_BAD_DURATION;
    float DifficultyMultiplier => Mathf.Pow(2, (DEFAULT_INCREMENT_BAD_DURATION / incrementBadDuration) - 1);
    // float DifficultyMultiplier => 1f;
    public override void _Process(double delta) {
        base._Process(delta);
        if (Input.IsActionJustReleased("debug_physics")) {
            VillainBubble.SetConfig(PickColor());
        }

        if (GameplayConfig.TimerTicks) {
            timeElapsed += (float)delta;
            if (timeElapsed > incrementBadDuration) {
                timeElapsed -= incrementBadDuration;
                _on_shoot_event();
            }
        }
        chainTimer = (float)Mathf.MoveToward(chainTimer, 0f, delta);
        if (chainTimer <= 0) {
            currentChain = 1;
        }
    }

    public void GameOver() {
        Audio.GameOver();
        GD.Print("Game Over");
        Reset();
    }

    public void Reset() {
        timeElapsed = 0;
        incrementBadDuration = DEFAULT_INCREMENT_BAD_DURATION;
        numShots = 0;
        foreach (var bubble in Bubbles.GetChildren()) {
            bubble.QueueFree();
        }
        foreach (var spring in Springs.GetChildren()) {
            spring.QueueFree();
        }
        VillainBubble.Reset();
        Player.Reset();
        BubbleQueue.Reset();
        Score = 0;
        EmitSignal(SignalName.ScoreChanged, Score);
    }

    int numShots = 0;
    public void _on_shoot_event() {
        numShots++;
        if (numShots >= GameplayConfig.GrowBubbleShots) {
            numShots = 0;
            VillainBubble.Grow();
        }
    }

	[Export] PackedScene BubbleSprite;
	public void SpawnBubblePop(Vector2 globalPosition, Texture2D texture) {
		var bubblePop = BubbleSprite.Instantiate<BubbleSprite>();
		AddChild(bubblePop);
		bubblePop.GlobalPosition = globalPosition;
		bubblePop.Texture = texture;
		bubblePop.PlayAnimation("pop");
	}

    public static BubbleGame Game { get; private set; }
}

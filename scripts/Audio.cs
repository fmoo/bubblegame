using Godot;
using System;
using System.Collections.Generic;

public partial class Audio : Node {
    public void Shoot() {
        MaybePlay(_Shoot);
    }
    public void Pop() {
        MaybePlay(_Pop);
    }
    public void Grow() {
        MaybePlay(_Grow);
    }
    public void Shrink() {
        MaybePlay(_Shrink);
    }
    public void GameOver() {
        MaybePlay(_GameOver);
    }

    HashSet<AudioStreamPlayer> didPlayThisFrame = new();
    void MaybePlay(AudioStreamPlayer player) {
        if (!didPlayThisFrame.Add(player)) return;
        player.Play();
    }

    [Export] AudioStreamPlayer _Shoot;
    [Export] AudioStreamPlayer _Pop;
    [Export] AudioStreamPlayer _Grow;
    [Export] AudioStreamPlayer _Shrink;
    [Export] AudioStreamPlayer _GameOver;

    public override void _Process(double delta) {
        base._Process(delta);
        didPlayThisFrame.Clear();
    }

}

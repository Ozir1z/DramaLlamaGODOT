using Godot;
using System;

public partial class llamaCharacter : CharacterBody3D
{
    public AnimationPlayer animPlayer;
    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        animPlayer.Play("Idle");
    }

}

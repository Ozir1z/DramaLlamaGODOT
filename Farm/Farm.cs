using Godot;
using System;

public partial class Farm : Node
{
    private VoidCharacter _player;

    public override void _Ready()
    {
        base._Ready();

        _player = GetNode<VoidCharacter>("Void");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GetTree().CallGroup("AI", "UpdateAITargetLocation", _player.GlobalPosition);
    }
}

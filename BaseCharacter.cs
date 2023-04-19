using Godot;
using System;

public partial class BaseCharacter : CharacterBody3D
{
	protected float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
	{
        Vector3 newVelocity = Velocity;

        if (!IsOnFloor())
            newVelocity.Y -= Gravity * (float)delta;

		Velocity = newVelocity;
		MoveAndSlide();
	}
}

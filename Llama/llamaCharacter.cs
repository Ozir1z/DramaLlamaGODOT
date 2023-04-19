using Godot;
using System;

public partial class LlamaCharacter : BaseCharacter
{
    protected const float Speed = 2f;
    private AnimationPlayer animPlayer;
    private NavigationAgent3D navAgent;
    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

        animPlayer.Play("Idle");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Velocity == Vector3.Zero)
            animPlayer.Play("Idle");

        if (IsOnFloor())
        {
            Vector3 currentLocation = GlobalPosition;
            Vector3 nextLocation = navAgent.TargetPosition;
            Vector3 newVelocity = (nextLocation - currentLocation).Normalized() * Speed;

            Velocity = Velocity.MoveToward(newVelocity, .25f);
            //RotateY((nextLocation - currentLocation).Normalized().Y);
            LookAt(nextLocation);
            //Rotation = new Vector3(0,nextLocation.Y,0);
            if (Velocity != Vector3.Zero)
                animPlayer.Play("Walk");

            MoveAndSlide();
        }

    //    navAgent.GetNextPathPosition();


    }

    public void UpdateAITargetLocation(Vector3 targetLocation)
    {
        navAgent.TargetPosition = targetLocation;
    }
}
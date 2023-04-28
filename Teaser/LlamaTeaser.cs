using Godot;
using System;

public partial class LlamaTeaser : BaseCharacterDramaLlama
{
    private AnimationPlayer _animPlayer;
    private NavigationAgent3D _navAgent;
    private Vector3 _moveToPoint;
    private bool _GoTime = false;

    public override void _Ready()
    {
        _moveToPoint = GetNode<Node3D>("MoveToPoint").GlobalPosition;
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animPlayer.Play("Walk",.2,3);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("kickBus"))
        {
            _GoTime = true;
            _navAgent.TargetPosition = _moveToPoint;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_GoTime)
        {
            Vector3 currentLocation = GlobalPosition;
            Vector3 nextLocation = _navAgent.GetNextPathPosition();
            Vector3 newVelocity = (nextLocation - currentLocation).Normalized() * 10f;

            Vector3 lookLocation = new(nextLocation.X, currentLocation.Y, nextLocation.Z);
            LookAt(lookLocation);

            Velocity = Velocity.MoveToward(newVelocity, .25f);
        }
        //else if (_currentStatus == FarmerStatus.Idle)
        //    Velocity = new Vector3(Vector3.Zero.X, Velocity.Y, Vector3.Zero.Z);


        MoveAndSlide();
        KinematicCollision3D kc = GetLastSlideCollision();
        if(kc != null)
        {
            if (kc.GetCollider() is BusTeaser bus)
            {
                bus.Boop();
            }

        }
       // PlayAnimation();
    }

}

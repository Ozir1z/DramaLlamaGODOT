using Godot;
using System;

public partial class FarmerCharacter : BaseCharacterDramaLlama
{
    protected const float Speed = 2f;

    private AnimationPlayer _animPlayer;
    private NavigationAgent3D _navAgent;
    private Vector3 _moveToPoint;

    private bool _movingToPoint = false;
    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        _moveToPoint = GetNode<Node3D>("MoveToPoint").GlobalPosition;
        _animPlayer.Play("Idle");
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("moveFarmer"))
        {
            _movingToPoint = true;
            _navAgent.TargetPosition = _moveToPoint;
        }
        if(@event.IsActionPressed("talkFarmer"))
        {

        }
    }

    public override void _Process(double delta)
    {
        if (_navAgent.DistanceToTarget() < 0.5f)
            _movingToPoint = false;

    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if(_movingToPoint)
        {
            Vector3 currentLocation = GlobalPosition;
            Vector3 nextLocation = _navAgent.GetNextPathPosition();
            Vector3 newVelocity = (nextLocation - currentLocation).Normalized() * Speed;

            Vector3 lookLocation = new(nextLocation.X, currentLocation.Y, nextLocation.Z);
            LookAt(lookLocation);

            Velocity = Velocity.MoveToward(newVelocity, .25f);

            _animPlayer.Play("Walk");
        }
        else
        {
            Velocity = new Vector3(Vector3.Zero.X, Velocity.Y, Vector3.Zero.Z);
            _animPlayer.Play("Idle");
        }
           

        MoveAndSlide();
    }
}

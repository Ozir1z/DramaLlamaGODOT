using Godot;
using System;
using System.Linq;
using static LlamaCharacter;

public partial class FarmerCharacter : BaseCharacterDramaLlama
{
    public enum FarmerStatus { Idle, Walk, Talk };

    protected FarmerStatus _currentStatus = FarmerStatus.Idle;
    protected const float Speed = 2f;

    private AnimationPlayer _animPlayer;
    private NavigationAgent3D _navAgent;
    private Vector3 _moveToPoint;
    private string[] _idleAnimations = {"Idle_Scratch", "Idle_Inspect"};
    private string _nextIdleAnimation = "Idle";
    private const double _idleTimerTime = 2;
    private double _idleTimer = _idleTimerTime;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        _moveToPoint = GetNode<Node3D>("MoveToPoint").GlobalPosition;
        _animPlayer.Play("Idle");
    }

    public override void _Input(InputEvent @event)
    {
        // input for the teaser
        if(@event.IsActionPressed("moveFarmer"))
        {
            _currentStatus = FarmerStatus.Walk;
            _navAgent.TargetPosition = _moveToPoint;
        }
        if(@event.IsActionPressed("talkFarmer"))
        {
            if(_currentStatus == FarmerStatus.Idle)
                _currentStatus = FarmerStatus.Talk;
            else if(_currentStatus == FarmerStatus.Talk)
            {
                SwitchToIdleStatus();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (_navAgent.DistanceToTarget() < 0.5f && _currentStatus == FarmerStatus.Walk)
            SwitchToIdleStatus();

        if (_currentStatus == FarmerStatus.Idle)
        {
            _idleTimer -= delta;

            if (_nextIdleAnimation == "Idle" && _idleTimer <= 0)
            {
                SwitchToIdleStatus();
                Random rand = new();
                int chance = rand.Next(1, 5);// 25% chance

                if (chance == 1)
                {
                    Random rand2 = new();
                    int idleAnimationNr = rand2.Next(0, _idleAnimations.Length);
                    _nextIdleAnimation = _idleAnimations[idleAnimationNr];

                }
            }
           
            if (_idleAnimations.Contains(_animPlayer.CurrentAnimation)
                && _animPlayer.CurrentAnimationPosition + 0.1 >= _animPlayer.CurrentAnimationLength)
            {
                _nextIdleAnimation = "Idle";
                SwitchToIdleStatus();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if(_currentStatus == FarmerStatus.Walk)
        {
            Vector3 currentLocation = GlobalPosition;
            Vector3 nextLocation = _navAgent.GetNextPathPosition();
            Vector3 newVelocity = (nextLocation - currentLocation).Normalized() * Speed;

            Vector3 lookLocation = new(nextLocation.X, currentLocation.Y, nextLocation.Z);
            LookAt(lookLocation);

            Velocity = Velocity.MoveToward(newVelocity, .25f);
        }
        else if(_currentStatus == FarmerStatus.Idle)
            Velocity = new Vector3(Vector3.Zero.X, Velocity.Y, Vector3.Zero.Z);



        MoveAndSlide();
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        switch (_currentStatus)
        {
            case FarmerStatus.Walk:
                _animPlayer.Play("Walk", .2);
                break;
            case FarmerStatus.Idle:
                _animPlayer.Play(_nextIdleAnimation, .2);
                break;
            case FarmerStatus.Talk:
                _animPlayer.Play("Talk", .2);
                break;
        }
    }


    private void SwitchToIdleStatus()
    {
        _currentStatus = FarmerStatus.Idle;
        _idleTimer = _idleTimerTime;
    }
}

using Godot;
using System;
using System.Linq;

public partial class LlamaCharacter : BaseCharacterDramaLlama
{
    public enum LlamaStatus { Idle, IdleClean, IdlePee, Walk };
    public LlamaStatus CurrentStatus = LlamaStatus.Idle;

    protected const float Speed = 2f;

    private AnimationPlayer _animPlayer;
    private NavigationAgent3D _navAgent;
    private readonly LlamaStatus[] _idleStatusses = { LlamaStatus.IdlePee, LlamaStatus.IdleClean, LlamaStatus.Idle };

    private int _roamRadius = 5;
    private float _locationTolerance = 0.5f;
    private Vector3 _startLocation;
    private double _idleTimer = 0;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

        _animPlayer.Play("Idle");

        _startLocation = GlobalPosition;
    }

    public override void _Process(double delta)
    {
        if (_navAgent.DistanceToTarget() < _locationTolerance && CurrentStatus == LlamaStatus.Walk)
        {
            Random rand = new();
            CurrentStatus = _idleStatusses[rand.Next(0, _idleStatusses.Length)];
            _idleTimer = 0;
        }
 
        // currently in an idle status
        // instead of a random idle status, give 20% chance to trigger a special idle witihin the idle (but only once)
        if (_idleStatusses.Contains(CurrentStatus))
        {
            if (CurrentStatus != LlamaStatus.Idle && _animPlayer.CurrentAnimationPosition >= _animPlayer.CurrentAnimationLength)
            {
                Random rand = new();
                CurrentStatus = _idleStatusses[rand.Next(0, _idleStatusses.Length)];
            }

           if(CurrentStatus == LlamaStatus.Idle)
                _idleTimer += delta;
        }


        // make amount idle time random
        if (_idleTimer > 3 && CurrentStatus == LlamaStatus.Idle)
        {
            Random random = new();
            int randomX = random.Next(-_roamRadius, _roamRadius);
            int randomZ = random.Next(-_roamRadius, _roamRadius);
            _navAgent.TargetPosition = _startLocation - new Vector3(randomX, 0, randomZ);
            CurrentStatus = LlamaStatus.Walk;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (IsOnFloor() && CurrentStatus == LlamaStatus.Walk)
        {
            Vector3 currentLocation = GlobalPosition;
            Vector3 nextLocation = _navAgent.GetNextPathPosition();
            Vector3 newVelocity = (nextLocation - currentLocation).Normalized() * Speed;
            
            Vector3 lookLocation = new(nextLocation.X, currentLocation.Y, nextLocation.Z);
            LookAt(lookLocation);
            Velocity = Velocity.MoveToward(newVelocity, .25f);

        }
        if(_idleStatusses.Contains(CurrentStatus))
            Velocity = new Vector3(Vector3.Zero.X, Velocity.Y, Vector3.Zero.Z);

        MoveAndSlide();
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        switch (CurrentStatus)
        {
            case LlamaStatus.Walk:
                _animPlayer.Play("Walk");
                break;
            case LlamaStatus.Idle:
                _animPlayer.Play("Idle");
                break;
            case LlamaStatus.IdlePee:
                _animPlayer.Play("Idle_Pee");
                break;
            case LlamaStatus.IdleClean:
                _animPlayer.Play("Idle_Clean");
                break;
        }
    }
}

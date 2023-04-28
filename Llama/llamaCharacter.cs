using Godot;
using System;
using System.Linq;
using static FarmerCharacter;

public partial class LlamaCharacter : BaseCharacterDramaLlama
{
    public enum LlamaStatus { Idle, Walk };
    public LlamaStatus CurrentStatus = LlamaStatus.Idle;

    protected const float Speed = 2f;

    private AnimationPlayer _animPlayer;
    private NavigationAgent3D _navAgent;
    private string[] _idleAnimations = { "Idle_Pee", "Idle_Clean", "Idle_Eating" };
    private string _nextIdleAnimation = "Idle";
    private const double _SpecialIdleTimerTime = 2;
    private double _specialIdleTimer = _SpecialIdleTimerTime;
    private double _idleTimer = 0;

    private int _roamRadius = 5;
    private float _locationTolerance = 0.5f;
    private Vector3 _startLocation;

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
            SwitchToIdleStatus();
            Random rand = new();
            _idleTimer = rand.Next(3, 6);
        }

        if (CurrentStatus == LlamaStatus.Idle)
        {
            _specialIdleTimer -= delta;
            if(_animPlayer.CurrentAnimation.Equals("Idle"))
                _idleTimer -= delta;

            if (_nextIdleAnimation == "Idle" && _specialIdleTimer <= 0)
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
        if (_idleTimer <= 0 && CurrentStatus == LlamaStatus.Idle)
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
        else if (CurrentStatus == LlamaStatus.Idle)
            Velocity = new Vector3(Vector3.Zero.X, Velocity.Y, Vector3.Zero.Z);

        MoveAndSlide();
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        switch (CurrentStatus)
        {
            case LlamaStatus.Walk:
                _animPlayer.Play("Walk", .2);
                break;
            case LlamaStatus.Idle:
                _animPlayer.Play(_nextIdleAnimation, .2);
                break;
        }
    }


    private void SwitchToIdleStatus()
    {
        CurrentStatus = LlamaStatus.Idle;
        _specialIdleTimer = _SpecialIdleTimerTime;
    }
}

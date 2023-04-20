using Godot;
using System;
using System.Linq;

public partial class LlamaCharacter : BaseCharacter
{
    public enum LlamaStatus { Idle, IdleClean, IdlePee, Walk };
    public LlamaStatus CurrentStatus = LlamaStatus.Idle;

    protected const float Speed = 2f;

    private AnimationPlayer animPlayer;
    private NavigationAgent3D navAgent;
    private LlamaStatus[] _idleStatusses = { LlamaStatus.IdlePee, LlamaStatus.IdleClean, LlamaStatus.Idle };

    private int _roamRadius = 5;
    private float _locationTolerance = 0.5f;
    private Vector3 _nextLocation; // if this doesnt get used outside of process, make local variables
    private Vector3 _startLocation;
    private bool _isMoving = false;
    private double _idleTimer = 0;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

        animPlayer.Play("Idle");

        _nextLocation = GlobalPosition;
        _startLocation = GlobalPosition;
    }

    public override void _Process(double delta)
    {
        if ((GlobalPosition - _nextLocation).Length() < _locationTolerance && CurrentStatus == LlamaStatus.Walk)
        {
            Random rand = new();
            CurrentStatus = _idleStatusses[rand.Next(0, _idleStatusses.Length)];
            _idleTimer = 0;
        }
 
        // currently in an idle status
        if (_idleStatusses.Contains(CurrentStatus))
        {
            if (CurrentStatus != LlamaStatus.Idle && animPlayer.CurrentAnimationPosition >= animPlayer.CurrentAnimationLength)
            {
                Random rand = new();
                CurrentStatus = _idleStatusses[rand.Next(0, _idleStatusses.Length)];
            }

           if(CurrentStatus == LlamaStatus.Idle)
                _idleTimer += delta;
        }


        // make amoutn idle time random
        if (_idleTimer > 3 && CurrentStatus == LlamaStatus.Idle)
        {
            Random random = new();
            int randomX = random.Next(-_roamRadius, _roamRadius);
            int randomZ = random.Next(-_roamRadius, _roamRadius);
            _nextLocation = _startLocation - new Vector3(randomX, 0, randomZ);
            navAgent.TargetPosition = _nextLocation;
            CurrentStatus = LlamaStatus.Walk;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (IsOnFloor() && CurrentStatus == LlamaStatus.Walk)
        {
            Vector3 currentLocation = GlobalPosition;
            Vector3 nextLocation = navAgent.TargetPosition;
            Vector3 newVelocity = (nextLocation - currentLocation).Normalized() * Speed;

            LookAt(new Vector3(nextLocation.X, currentLocation.Y, nextLocation.Z));
            Velocity = Velocity.MoveToward(newVelocity, .25f);

        }
        if(_idleStatusses.Contains(CurrentStatus))
            Velocity = Vector3.Zero;

        MoveAndSlide();
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        switch (CurrentStatus)
        {
            case LlamaStatus.Walk:
                animPlayer.Play("Walk");
                break;
            case LlamaStatus.Idle:
                animPlayer.Play("Idle");
                break;
            case LlamaStatus.IdlePee:
                animPlayer.Play("Idle_Pee");
                break;
            case LlamaStatus.IdleClean:
                animPlayer.Play("Idle_Clean");
                break;
        }
    }
}

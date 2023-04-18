using Godot;
using System;
using System.Diagnostics;

public partial class VoidCharacter : CharacterBody3D
{
    const float speed = 5f;
    const float jumpForce = 3f;
    const float acceleration = 5f;
    const float sensitivity = 0.01f;

    public Node3D Head;
    public Camera3D Cam;
    
    private Vector3 velocity = Vector3.Zero;

    public override void _Ready()
    {
        Head = GetNode<Node3D>("Head");
        Cam = GetNode<Camera3D>("Head/Camera3D");

       // Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion) 
        {
            InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
            Head.RotateY(-mouseMotion.Relative.X * sensitivity);
            Cam.RotateX(-mouseMotion.Relative.Y * sensitivity);

            Vector3 cameraRot = Cam.Rotation;
            cameraRot.X = Mathf.Clamp(cameraRot.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
            Cam.Rotation = cameraRot;
        }

    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
        Vector3 direction = (Head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if(direction != Vector3.Zero)
        {
            velocity.X = direction.X * speed;
            velocity.Z = direction.Z * speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, acceleration);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, acceleration);
        }

        if(IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            velocity.Y = jumpForce;
        }
        
        if(!IsOnFloor())
        {
            velocity.Y -= (float)(ProjectSettings.GetSettingWithOverride("physics/3d/default_gravity").AsDouble() * delta);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}

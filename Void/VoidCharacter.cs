using Godot;

public partial class VoidCharacter : BaseCharacterDramaLlama
{
    protected const float Speed = 5f;
    protected const float JumpVelocity = 5f;
    protected const float Sensitivity = 0.01f;

    protected Node3D Head;
    protected Camera3D Cam;
    public override void _Ready()
    {
        Head = GetNode<Node3D>("Head");
        Cam = GetNode<Camera3D>("Head/Camera3D");

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion) 
        {
            InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
            Head.RotateY(-mouseMotion.Relative.X * Sensitivity);
            Cam.RotateX(-mouseMotion.Relative.Y * Sensitivity);

            Vector3 cameraRot = Cam.Rotation;
            cameraRot.X = Mathf.Clamp(cameraRot.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
            Cam.Rotation = cameraRot;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Vector3 newVelocity = Velocity;

        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            newVelocity.Y = JumpVelocity;

        Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
        Vector3 direction = (Head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if(direction != Vector3.Zero)
        {
            newVelocity.X = direction.X * Speed;
            newVelocity.Z = direction.Z * Speed;
        }
        else
        {
            newVelocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            newVelocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = newVelocity;
        MoveAndSlide();
    }
}

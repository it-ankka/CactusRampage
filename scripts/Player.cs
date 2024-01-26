using Godot;

public partial class Player : CharacterBody3D
{
  [Export] public float Speed = 5.0f;
  [Export] public float JumpVelocity = 4.5f;
  [Export] Node3D Head;
  [Export] Node3D Arms;
  [Export] HealthComponent Health;
  [Export(PropertyHint.Range, "-90,90,0.001,radians_as_degrees")] float CameraAngleMax = Mathf.Pi / 2;
  [Export(PropertyHint.Range, "-90,90,0.001,radians_as_degrees")] float CameraAngleMin = -Mathf.Pi / 2;

  [ExportGroup("Juice")]
  [Export] public float BobAmount = 0.01f;
  [Export] public float BobFreq = 0.01f;
  [Export] public float HeadRotationAmount = 0.04f;
  [Export] public float ArmsSwayAmount = 0.01f;
  [Export] public float ArmsRotationAmount = 0.04f;
  [Export] public float SlideArmsRotationAmount = 0.3f;

  Vector3 defaultArmsPos, defaultHeadPos = Vector3.Zero;
  Vector2 InputDir, MouseInput = Vector2.Zero;

  public float look_sensitivity = ProjectSettings.GetSetting("player/look_sensitivity", 0.05f).AsSingle();
  public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

  public override void _Ready()
  {
    defaultHeadPos = Head.Position;
    defaultArmsPos = Arms.Position;
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
    {
      var mouseEvent = @event as InputEventMouseMotion;
      MouseInput = mouseEvent.Relative;
      RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * look_sensitivity));
      Head.RotateX(Mathf.DegToRad(-mouseEvent.Relative.Y * look_sensitivity));
      Head.Rotation = Head.Rotation with { X = Mathf.Clamp(Head.Rotation.X, CameraAngleMin, CameraAngleMax) };
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    Vector3 velocity = Velocity;

    // Add the gravity.
    if (!IsOnFloor())
      velocity.Y -= gravity * (float)delta;

    // Handle Jump.
    if (Input.IsActionJustPressed("jump") && IsOnFloor())
      velocity.Y = JumpVelocity;

    InputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    Vector3 direction = (Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y)).Normalized();
    if (direction != Vector3.Zero && IsOnFloor())
    {
      velocity.X = direction.X * Speed;
      velocity.Z = direction.Z * Speed;
    }
    else if (IsOnFloor())
    {
      velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
      velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
    }

    Velocity = velocity;
    MoveAndSlide();

    // Apply Juice
    JuiceUtils.ApplyTilt(Head, InputDir.X, HeadRotationAmount, delta);
    JuiceUtils.ApplyTilt(Arms, InputDir.X, ArmsRotationAmount, delta);
    JuiceUtils.ApplySway(Arms, MouseInput, ArmsSwayAmount, delta);
    var playerMoving = Velocity.Length() > 0 && IsOnFloor();
    var armBob = new Vector2(BobAmount, BobAmount) * (Velocity.Length() / Speed);
    JuiceUtils.ApplyBob(Arms, playerMoving, defaultArmsPos, armBob, BobFreq, delta);
  }
}

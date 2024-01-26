using Godot;

public partial class WorldCamera : Camera3D
{
  [Export] Camera3D weaponCamera;

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    weaponCamera.GlobalPosition = GlobalPosition;
    weaponCamera.GlobalRotation = GlobalRotation;
  }
}

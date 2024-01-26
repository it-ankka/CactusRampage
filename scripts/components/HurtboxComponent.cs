using Godot;

[GlobalClass]
public partial class HurtboxComponent : Area3D
{
  [Signal] public delegate void HurtboxEnteredEventHandler(int healthChange = 0);

  public void Activate(int healthChange = 0)
  {
    EmitSignal(SignalName.HurtboxEntered, healthChange);
  }
}

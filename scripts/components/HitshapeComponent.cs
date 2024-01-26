using Godot;

public partial class HitshapeComponent : ShapeCast3D
{
  [Signal] public delegate void HitshapeHitEventHandler(int healthChange = 0);
  [Export] public int HealthChangeAmount = -1;
  [Export] public bool CanHitMultiple = false;

  public void Hit()
  {
    if (!IsColliding())
      return;
    for (int i = 0; i < GetCollisionCount(); i++)
    {
      var coll = GetCollider(i);
      if (coll is not HurtboxComponent)
        continue;
      Activate(coll as HurtboxComponent, HealthChangeAmount);
      if (!CanHitMultiple) return;
    }
  }

  public void Activate(HurtboxComponent hurtbox = null, int healthChange = 0)
  {
    if (hurtbox != null)
      hurtbox.Activate(healthChange);
    EmitSignal(SignalName.HitshapeHit, healthChange);
  }
}

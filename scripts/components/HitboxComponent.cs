using Godot;

[GlobalClass]
public partial class HitboxComponent : Area3D
{
  [Signal] public delegate void HitboxHitEventHandler(int healthChange = 0);
  [Export] public int HealthChangeAmount = 1;
  [Export] public bool CanHitMultiple = false;

  public override void _Ready()
  {
    this.AreaEntered += HitboxEnteredHandler;
  }

  public void HitboxEnteredHandler(Area3D area)
  {
    if (area is not HurtboxComponent)
      return;

    Activate(area as HurtboxComponent, HealthChangeAmount);
  }

  public void Activate(HurtboxComponent hurtbox = null, int healthChange = 0)
  {
    if (hurtbox != null)
      hurtbox.Activate(healthChange);
    EmitSignal(SignalName.HitboxHit, healthChange);
  }

  public void Hit()
  {
    var prevMonitoring = Monitoring;
    Monitoring = true;
    var areas = GetOverlappingAreas();
    foreach (var area in areas)
    {
      if (area is not HurtboxComponent)
        continue;

      if (area is HurtboxComponent)
      {
        Activate(area as HurtboxComponent, HealthChangeAmount);
        if (!CanHitMultiple) break;
      }
    }
    Monitoring = prevMonitoring;
  }
}

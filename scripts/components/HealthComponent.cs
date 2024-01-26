using Godot;

[GlobalClass]
public partial class HealthComponent : Node
{
  [Signal] public delegate void HealthUpdateEventHandler(int change, int newHealth);
  [Signal] public delegate void DamagedEventHandler(int change);
  [Signal] public delegate void HealedEventHandler(int change);
  [Signal] public delegate void DeadEventHandler();

  private int _maxHealth = 10;
  private int _health = 10;

  [Export] public bool EmitOnReady = true;
  [Export] public bool Enabled = true;
  [Export]
  public int MaxHealth
  {
    get => _maxHealth;
    set
    {
      if (!Enabled || value < 1)
        return;

      _maxHealth = value;
      if (value < _health)
        _health = value;
    }
  }

  [Export]
  public int Health
  {
    get => _health;
    set
    {
      if (!Enabled) return;
      int change = value - _health;
      _health = Mathf.Clamp(value, 0, MaxHealth);
      EmitSignal(SignalName.HealthUpdate, change, _health);
      if (change > 0) EmitSignal(SignalName.Healed, change);
      else if (change < 0) EmitSignal(SignalName.Damaged, change);
      if (_health == 0) EmitSignal(SignalName.Dead);
    }
  }

  public override void _Ready()
  {
    EmitSignal(SignalName.HealthUpdate, 0, Health);
  }

  public void UpdateHealth(int change)
  {
    if (!Enabled) return;
    Health += change;
  }
}

using Godot;

public abstract partial class EnemyState : State
{
  protected Enemy enemy;

  public override void Init(StateMachine stateMachine)
  {
    enemy = stateMachine.GetParent<Enemy>();
    base.Init(stateMachine);
  }
}

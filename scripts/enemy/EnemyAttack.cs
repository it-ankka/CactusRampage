using Godot;

[GlobalClass]
public partial class EnemyAttack : EnemyState
{
  public override void Enter()
  {
    enemy.AnimationTree.Set("parameters/EnemyBlendTree/Attack/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

    GetTree().CreateTimer(1f).Timeout += () => fsm.Back();
  }
}

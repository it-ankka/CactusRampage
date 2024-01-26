using Godot;

[GlobalClass]
public partial class EnemyWalk : EnemyState
{
  Player player;
  public override void Enter()
  {
    player = GetTree().CurrentScene.GetNodeOrNull<Player>("%Player");
  }

  public override void PhysicsProcess(double delta)
  {
    if (player == null || !player.IsInsideTree())
    {
      player = GetTree().CurrentScene.GetNodeOrNull<Player>("%Player");
      return;
    }

    var playerPosition = player.GlobalPosition;
    enemy.Velocity = enemy.Stats.Speed * enemy.GlobalPosition.DirectionTo(playerPosition with { Y = enemy.GlobalPosition.Y });
    enemy.LookAt(enemy.GlobalPosition - enemy.Velocity);

    if ((playerPosition - enemy.GlobalPosition).Length() < enemy.Stats.MeleeRange)
    {
      enemy.Velocity = Vector3.Zero;
      enemy.StateMachine.ChangeState("Attack");
    }
    enemy.AnimationTree.Set("parameters/EnemyBlendTree/Moving/blend_position", enemy.Velocity.Length() / enemy.Stats.Speed);

    enemy.MoveAndSlide();
  }
}

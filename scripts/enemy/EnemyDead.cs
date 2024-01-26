using Godot;

[GlobalClass]
public partial class EnemyDead : EnemyState
{
    [Export] double DestroyTime = 3;
    public override void Enter()
    {
        enemy.AnimationStateMachine.Travel("Dead");
        GetTree().CreateTimer(3).Timeout += () => enemy.QueueFree();
    }
}

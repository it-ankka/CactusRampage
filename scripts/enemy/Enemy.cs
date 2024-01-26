using Godot;

public partial class Enemy : CharacterBody3D
{
  [Export] public EnemyResource Stats;
  [Export] public StateMachine StateMachine;
  [Export] public HealthComponent Health;
  [Export] public HitshapeComponent Hitshape;
  [Export] public AnimationTree AnimationTree;
  public AnimationNodeStateMachinePlayback AnimationStateMachine;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    AnimationStateMachine = (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback");

    Health.MaxHealth = Stats.Health;
    Health.Health = Stats.Health;
    Hitshape.HealthChangeAmount = -Stats.Damage;
    Health.Dead += () => Die();
    Health.Damaged += (int healthChange) => AnimationTree.Set("parameters/EnemyBlendTree/HitReact/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

    if (StateMachine != null)
      StateMachine?.Init(this);
  }

  public void Die()
  {
    StateMachine.ChangeState("Dead");
  }
}

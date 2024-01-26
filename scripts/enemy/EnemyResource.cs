using Godot;

[GlobalClass]
public partial class EnemyResource : Resource
{
    [Export] public int Damage = 1;
    [Export] public int Health = 1;
    [Export] public float Speed = 3.0f;
    [Export] public float MeleeRange = 1.0f;
    [Export] public float AttackCooldownTime = 1.0f;
}

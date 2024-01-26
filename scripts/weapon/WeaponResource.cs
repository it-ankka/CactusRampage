using Godot;

public enum WeaponType
{
  Melee,
  Hitscan,
  Shotgun,
  Projectile,
}

[GlobalClass]
public partial class WeaponResource : Resource
{
  [ExportGroup("Effects")]
  [Export] public AudioStream ShotSoundAudioStream;
  [Export] public PackedScene MuzzleFlashEffectScene;
  [ExportGroup("Stats")]
  [Export] public int Damage = 1;
  [Export] public float ShotCooldown = 1.0f;
  [Export] public int Capacity = 6;
  [Export] public int TotalCapacity = 18;
  [ExportGroup("Recoil")]
  [Export] public float RecoilScale = 0.1f;
  [Export] public Vector3 RecoilAmplitude = Vector3.One;
  [Export] public Vector3 RecoilRandomness = Vector3.Zero;
  [Export] public float RecoilTweenTime = 0.05f;
  [Export] public float RecoilResetTime = 0.1f;
}

using Godot;

public enum WeaponState
{
  Ready,
  Reloading,
  OnCooldown,
}

enum BufferedAction
{
  None,
  Attack,
  Reload,
  CancelReload,
}

public partial class Weapon : Node3D
{
  [Export] public WeaponResource Res;
  [Export] public Node3D Muzzle;
  [Export] public WeaponState State;
  [Export] AnimationTree AnimationTree;
  AnimationNodeStateMachinePlayback animationStateMachine;
  BufferedAction bufferedAction = BufferedAction.None;
  WeaponManager weaponManager;
  public int CurrentAmmo;
  public int CurrentReserveAmmo;

  SceneTreeTimer cooldownTimer;

  Vector3 defaultPosition = Vector3.Zero, defaultRotation = Vector3.Zero;
  Tween recoilTween;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    defaultPosition = Position;
    defaultRotation = Rotation;
    CurrentAmmo = Res.Capacity;
    CurrentReserveAmmo = Res.TotalCapacity;
    animationStateMachine = (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback");
    AnimationTree.AnimationFinished += (StringName animationName) =>
    {
      if (animationName != "RESET") return;
      State = WeaponState.Ready;
    };
  }

  public void Init(WeaponManager manager)
  {
    weaponManager = manager;
  }

  public void Attack()
  {
    if (weaponManager == null)
      return;
    Shoot();
  }

  public override void _Process(double delta)
  {
    if (State == WeaponState.Reloading && Input.IsActionJustPressed("primary_fire"))
      bufferedAction = BufferedAction.CancelReload;
  }

  public void Shoot()
  {
    if (State == WeaponState.OnCooldown && cooldownTimer != null && cooldownTimer.TimeLeft < 0.1f)
      bufferedAction = BufferedAction.Attack;

    if (State != WeaponState.Ready) return;

    StartCooldown();

    var audioPlayer = weaponManager.WeaponAudioPlayer;
    if (CurrentAmmo < 1)
    {
      audioPlayer.Stream = weaponManager.OutOfAmmoAudioStream;
      audioPlayer.Play();
      return;
    }

    audioPlayer.Stream = Res.ShotSoundAudioStream;
    audioPlayer.Play();

    if (Res.MuzzleFlashEffectScene != null)
    {
      var muzzleFlash = Res.MuzzleFlashEffectScene.Instantiate<CpuParticles3D>();
      muzzleFlash.Ready += () => muzzleFlash.Emitting = true;
      muzzleFlash.Finished += () => muzzleFlash.QueueFree();
      Muzzle.AddChild(muzzleFlash);
    }
    ApplyVisualRecoil();

    CurrentAmmo -= 1;
    if (!weaponManager.ShootRay.IsColliding()) return;

    var collider = weaponManager.ShootRay.GetCollider();
    if (collider is HurtboxComponent)
    {
      var hurtbox = collider as HurtboxComponent;
      hurtbox.Activate(-Res.Damage);
    }
  }

  public void StartCooldown()
  {
    State = WeaponState.OnCooldown;
    cooldownTimer = GetTree().CreateTimer(Res.ShotCooldown);
    cooldownTimer.Timeout += () =>
    {
      if (State != WeaponState.Reloading)
        State = WeaponState.Ready;

      if (bufferedAction == BufferedAction.Reload)
        Reload();
      else if (bufferedAction == BufferedAction.Attack)
        Attack();

      bufferedAction = BufferedAction.None;
    };
  }

  public void Reload()
  {
    if (CurrentReserveAmmo < 1 || CurrentAmmo >= Res.Capacity) return;
    if (State != WeaponState.Ready)
    {
      bufferedAction = BufferedAction.Reload;
      return;
    }
    bufferedAction = BufferedAction.None;
    State = WeaponState.Reloading;
    animationStateMachine.Travel("reload_loop");
  }

  public void ReloadBullet()
  {
    if (State != WeaponState.Reloading) return;

    CurrentAmmo += 1;
    CurrentReserveAmmo -= 1;
    GD.Print("AMMO: ", CurrentAmmo, " / ", CurrentReserveAmmo);

    if (bufferedAction == BufferedAction.CancelReload || CurrentReserveAmmo < 1 || CurrentAmmo >= Res.Capacity)
      CancelReload();
  }

  public void CancelReload()
  {
    State = WeaponState.OnCooldown;
    animationStateMachine.Travel("default");
  }

  private void ApplyVisualRecoil()
  {
    Res.RecoilAmplitude.Y *= GD.Randf() > 0.5 ? -1 : 1;
    var recoilX = Res.RecoilAmplitude.X - (GD.Randf() * Res.RecoilRandomness.X) * Res.RecoilAmplitude.X;
    var recoilY = Res.RecoilAmplitude.Y - (GD.Randf() * Res.RecoilRandomness.Y) * Res.RecoilAmplitude.Y;
    var recoilZ = Res.RecoilAmplitude.Z - (GD.Randf() * Res.RecoilRandomness.Z) * Res.RecoilAmplitude.Z;
    Vector3 recoilDelta = new Vector3(recoilX, recoilY, Res.RecoilAmplitude.Z) * Res.RecoilScale;
    Vector3 recoil = recoilDelta + new Vector3(defaultRotation.X, defaultRotation.Z, defaultPosition.Z);
    if (recoilTween != null)
      recoilTween.Kill();

    recoilTween = CreateTween();
    recoilTween.TweenProperty(this, "position:z", recoil.Z, Res.RecoilTweenTime);
    recoilTween.Parallel().TweenProperty(this, "rotation", new Vector3(recoil.X, Rotation.Y, recoil.Y), Res.RecoilTweenTime);
    recoilTween
      .SetTrans(Tween.TransitionType.Elastic)
      .SetEase(Tween.EaseType.Out)
      .TweenProperty(this, "position", defaultPosition, Res.RecoilResetTime);
    recoilTween
      .Parallel()
      .SetTrans(Tween.TransitionType.Elastic)
      .SetEase(Tween.EaseType.Out)
      .TweenProperty(this, "rotation", defaultRotation, Res.RecoilResetTime);
  }
}

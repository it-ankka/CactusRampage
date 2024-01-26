using Godot;
using Godot.Collections;


public partial class WeaponManager : Node3D
{
  [Signal] public delegate void AmmoUpdateEventHandler(int currentAmmo, int capacity, int currentReserveAmmo);

  [Export] public Node3D WeaponHolder { get; private set; }
  [Export] public RayCast3D ShootRay { get; private set; }
  [Export] public AudioStreamPlayer WeaponAudioPlayer { get; private set; }
  [Export] public AudioStream OutOfAmmoAudioStream { get; private set; }
  public Weapon ActiveWeapon { get { return weapons[activeWeaponSlot]; } }

  Array<Weapon> weapons = new Array<Weapon>();
  int activeWeaponSlot = 0;

  public override void _Ready()
  {
    WeaponHolder = (WeaponHolder != null ? WeaponHolder : this);
    var children = WeaponHolder.GetChildren();
    foreach (var child in children)
    {
      if (child is Weapon)
      {
        var weaponChild = child as Weapon;
        weaponChild.Init(this);
        weaponChild.Visible = false;
        weapons.Add(weaponChild);
      }
    }
    weapons[0].Visible = true;
  }

  public override void _Process(double delta)
  {
    EmitSignal(SignalName.AmmoUpdate, ActiveWeapon.CurrentAmmo, ActiveWeapon.Res.Capacity, ActiveWeapon.CurrentReserveAmmo);

    if (Input.IsActionJustPressed("primary_fire"))
      ActiveWeapon.Attack();
  }

  public override void _Input(InputEvent @event)
  {
    if (@event.IsActionPressed("reload"))
    {
      ActiveWeapon.Reload();
      return;
    }

    // Weapon switch
    for (int i = 0; i < 9; i++)
    {
      if (@event.IsActionPressed("weapon_slot_" + (i + 1)))
      {
        GD.Print("Switching to weapon: ", i);
        ActiveWeapon.CancelReload();
        ActiveWeapon.State = WeaponState.Ready;
        SwitchWeapon(i);
        return;
      }
    }
  }

  public void SwitchWeapon(int slot = -1)
  {
    if (slot < 0 || slot >= weapons.Count)
      return;

    activeWeaponSlot = slot;
    for (int i = 0; i < weapons.Count; i++)
    {
      var gun = weapons[i];
      gun.Visible = i == activeWeaponSlot;
    }
  }

}

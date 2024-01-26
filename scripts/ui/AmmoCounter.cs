using Godot;

public partial class AmmoCounter : Control
{
  [Export] Container CurrentAmmoContainer;
  [Export] PackedScene ChamberEmptyControl;
  [Export] PackedScene ChamberFullControl;
  [Export] Label ReserveAmmoCountLabel;

  public void UpdateAmmoCount(int currentAmmo, int capacity, int currentReserve)
  {
    foreach (var child in CurrentAmmoContainer.GetChildren())
    {
      CurrentAmmoContainer.RemoveChild(child);
      child.QueueFree();
    }

    for (int i = 0; i < capacity - currentAmmo; i++)
      CurrentAmmoContainer.AddChild(ChamberEmptyControl.Instantiate());

    for (int i = 0; i < currentAmmo; i++)
      CurrentAmmoContainer.AddChild(ChamberFullControl.Instantiate());

    ReserveAmmoCountLabel.Text = currentReserve.ToString();
  }
}

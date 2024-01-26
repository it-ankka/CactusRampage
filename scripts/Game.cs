using Godot;
// using Godot.Collections;

public partial class Game : Node
{
  [Export] public PackedScene DesertPropScene;
  [Export] public PackedScene EnemyScene;
  [Export] public int PropCount = 20;
  [Export] public float EnemySpawnTime = 10f;
  Player player;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    player = GetNodeOrNull<Player>("Player");
    Input.MouseMode = Input.MouseModeEnum.Captured;
    SpawnProps();
    GetTree().CreateTimer(EnemySpawnTime).Timeout += SpawnEnemy;
  }

  Vector3 GetRandomLocation()
  {
    var x = GD.RandRange(10, 100) * (GD.Randf() < 0.5 ? 1 : -1);
    var z = GD.RandRange(10, 100) * (GD.Randf() < 0.5 ? 1 : -1);
    return new Vector3(x, 0, z);
  }

  void SpawnProps()
  {
    // var props = new Array<Node3D>();
    for (int i = 0; i < PropCount; i++)
    {
      var desertProp = DesertPropScene.Instantiate<Node3D>();
      desertProp.Position = GetRandomLocation();
      AddChild(desertProp);
    }
  }

  void SpawnEnemy()
  {
    var enemy = EnemyScene.Instantiate<Node3D>();
    enemy.Position = player != null ? player.Position + GetRandomLocation() : GetRandomLocation();
    AddChild(enemy);
    EnemySpawnTime *= 0.99f;
    GetTree().CreateTimer(EnemySpawnTime).Timeout += SpawnEnemy;
  }

  public override void _Input(InputEvent @event)
  {
    if (@event.IsActionPressed("ui_cancel"))
      Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
  }
}

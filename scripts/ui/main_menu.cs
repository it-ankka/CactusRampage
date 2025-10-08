using Godot;

public partial class main_menu : Node3D
{
  private SceneManager sceneManager;
  private Button startGameButton, quitGameButton;
  [Export(PropertyHint.File, "*.tscn")] string levelNodePath;
  public override void _Ready()
  {
    sceneManager = GetNode<SceneManager>("/root/SceneManager");
    startGameButton = GetNode<Button>("%StartGameButton");
    quitGameButton = GetNode<Button>("%QuitGameButton");
    startGameButton.Pressed += () => sceneManager.GotoScene(levelNodePath);
    quitGameButton.Pressed += () => GetTree().Quit();
  }
}

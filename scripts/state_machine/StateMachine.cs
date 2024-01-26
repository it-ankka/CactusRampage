﻿using Godot;
using System.Linq;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
  [Export]
  const bool DEBUG = true;

  private bool initialized = false;

  public Node Target { get; private set; }

  protected State state;
  protected List<State> states;
  protected Queue<string> history = new Queue<string>();

  public void Init(Node newTarget)
  {
    Target = newTarget;
    states = GetChildren().OfType<State>().ToList();

    if (states.Count < 1)
    {
      GD.PushWarning("No states found");
      return;
    }

    foreach (var stateNode in states)
    {
      if (state == null)
        state = stateNode;
      stateNode.Init(this);
    }

    initialized = true;
    if (DEBUG)
      GD.Print("StateMachine initialized.");

    _EnterState();
  }

  private void _EnterState()
  {
    if (DEBUG)
      GD.Print("Entering state: " + state.Name);
    state.Enter();
  }

  public void ChangeState(string nextStateName)
  {
    if (!initialized)
    {
      GD.PrintErr("STATE MACHINE NOT INITIALIZED");
      return;
    }
    var nextState = states.Find((s) => s.Name == nextStateName);
    if (nextState == null)
    {
      GD.PrintErr("INVALID STATE NAME: \"" + nextStateName + "\". STATE NOT FOUND.");
      return;
    }
    history.Enqueue(state.Name);
    state = nextState;
    _EnterState();
  }

  public void Back()
  {
    if (history.Count > 0)
    {
      state = states.Find(s => s.Name == history.Dequeue());
      _EnterState();
    }
  }

  public override void _Process(double delta)
  {
    state?.Process(delta);
  }

  public override void _PhysicsProcess(double delta)
  {
    state?.PhysicsProcess(delta);
  }

  public override void _Input(InputEvent @event)
  {
    state?.Input(@event);
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    state?.UnhandledInput(@event);
  }

  public override void _UnhandledKeyInput(InputEvent @event)
  {
    state?.UnhandledKeyInput(@event);
  }

  public override void _Notification(int what)
  {
    state?.Notification(what);
  }
}

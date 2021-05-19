using Godot;
public class TurnCounterComponent : Node
{
    int _turnCount = 0;

    public void OnTurnStarted()
    {
        _turnCount++;
        GD.Print(string.Format("Turn {0}", _turnCount));
    }
}
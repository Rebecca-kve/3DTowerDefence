using Godot;
//using System;

public partial class TurnHandler : Node3D
{
 
    //public event EventHandler startTurn;
    [Signal] public delegate void startTurnEventHandler();
    [Signal] public delegate void SurvivedTurnEventHandler();
    [Signal] public delegate void endTurnEventHandler();
    //public event EventHandler endTurn;
    public static bool isTurnActive = false;

    // Turn can only end when there are no enemyes
    public static int EnemyCountThisTurn = 0;
    public static int EnemyRemovedThisTurn = 0;
    public static int currentTurn = 0;

    public static void Reset() {
        isTurnActive = false;
        EnemyCountThisTurn = 0;
        EnemyRemovedThisTurn = 0;
        currentTurn = 0;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
        if (isTurnActive && IsTurnOver()) {
            //SurvivedTurn?.Invoke(this, EventArgs.Empty);            
            StopTheTurnEvent();
            EmitSignal(SignalName.SurvivedTurn);
        }
    }
	public void StartNextTurn() {



        if (!isTurnActive) {
            currentTurn += 1;
            //startTurn?.Invoke(typeof(TurnHandler), EventArgs.Empty);
            EmitSignal(SignalName.startTurn);
            isTurnActive = true;
        }
    }
    private void StopTheTurnEvent() {
        //endTurn?.Invoke(this, EventArgs.Empty);
        EmitSignal(SignalName.endTurn);

        // reset values for next turn
        EnemyCountThisTurn = 0;
        EnemyRemovedThisTurn = 0;

        isTurnActive = false;
    }
    private bool IsTurnOver() {
        var search = GetTree().GetNodesInGroup("enemy").Count;

        if (EnemyCountThisTurn - EnemyRemovedThisTurn <= 0 && search == 0) {
            
            return true;
        }
        else return false;
    }
}

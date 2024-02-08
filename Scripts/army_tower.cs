using Godot;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

public partial class army_tower : Node3D
{
    private int spawns = 1;

    // Called when the node enters the scene tree for the first time.        
	public override void _Ready() {

        TurnHandler turns = GetNode<TurnHandler>("/root/TurnHandler");
        turns.startTurn += ProductionEvent_SpawnEnemyes;

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }
    private void ProductionEvent_SpawnEnemyes() { // ProductionEvent_SpawnEnemyes(object sender, System.EventArgs e)
        TurnHandler.EnemyCountThisTurn += spawns;
        SpawnWave();
    }
    private async void SpawnWave() {

        var scene = GD.Load<PackedScene>("res://Scenes/Enemy/Boss.tscn");
        

        for (int i = 0; i < spawns; i++) {
            var inst = scene.Instantiate<Enemy>();            
            await ToSignal(GetTree().CreateTimer(0.5), "timeout");
            AddChild(inst, true);
        }

    }

    private void _on_static_body_3d_input_event(Variant camera, Variant @event, Vector3 position, Vector3 normal, int shape_idx) {
        if (Input.IsActionJustPressed("m1click")) GD.Print("Mouse clicked on");
    }

}

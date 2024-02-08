using Godot;
using System;

public partial class Button3D : StaticBody3D {

    [Export] private Button button;
    [Export] private BuildGridMap grid;
	private bool clickable = true;
    TurnHandler turns;
	public override void _Ready() {
        turns = GetNode<TurnHandler>("/root/TurnHandler");
        turns.endTurn += Enable;
    }
    private void _on_tree_exiting() {
        turns.endTurn -= Enable;
    }
    public override void _Process(double delta)	{
	}
    private void Enable() {
        //if (grid.mapfinished) { return; }
        clickable = true;
        Visible = true;
    }
	private void _on_input_event(Variant camera, Variant @event, Vector3 position, Vector3 normal, int shape_idx) {
        if (Input.IsActionJustPressed("m1click") && clickable) {
            button.EmitSignal("pressed");
            button.SetPressedNoSignal(true);
            clickable = false;
            Visible = false;
            turns.StartNextTurn();
        }
        else button.SetPressedNoSignal(false);
    }
    private void _on_next_turn_pressed() {
        GD.Print("pressed");
    }

}

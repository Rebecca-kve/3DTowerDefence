using Godot;
using System;

public partial class GameOver : Control
{
    [Export] private Label score;
    private void _on_castle_castle_infected() {
        score.Text = "Score: " + TurnHandler.currentTurn + " Turns.";
        Visible = true;
	}
    private void _on_back_pressed() {
        //GetTree().Quit();
        Global global = (Global)GetNode("/root/Global");
        global.GotoScene("res://Scenes/MainMenu.tscn");
    }
}

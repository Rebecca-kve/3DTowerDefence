using Godot;
using System;
using System.IO;

public partial class MainMenu : Control {




    public override void _Ready() {
	}
	public override void _Process(double delta) {
	}
    public MainMenu() {
    }

    private void _on_play_pressed() {

        Global global = (Global)GetNode("/root/Global");
        global.GotoScene("res://Scenes/Levels/level_02.tscn");
        //GetTree().ChangeSceneToFile("res://Scenes/Levels/level_02.tscn");
        //QueueFree();
        //var nextScene = (PackedScene)ResourceLoader.Load("res://Scenes/Levels/level_02.tscn");
        //GetTree().ChangeSceneToPacked(nextScene);

    }
    private void _on_quit_pressed() {
        GetTree().Quit();
    }
}

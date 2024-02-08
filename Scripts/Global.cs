using Godot;
using System;

public partial class Global : Godot.Node {
    public Node CurrentScene { get; set; }

    public override void _Ready() {
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public void GotoScene(string path) {
        CallDeferred(nameof(DeferredGotoScene), path);
    }

    public void DeferredGotoScene(string path) {
        // It is now safe to remove the current scene
        CurrentScene.QueueFree();

        // Load a new scene.
        var nextScene = (PackedScene)GD.Load(path);

        // Instance the new scene.
        CurrentScene = nextScene.Instantiate();

        // Add it to the active scene, as child of root.
        GetTree().Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene() API.
        //GetTree().UnloadCurrentScene(CurrentScene);

    }
}

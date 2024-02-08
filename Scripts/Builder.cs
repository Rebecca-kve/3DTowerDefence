using Godot;
using System;
using System.Transactions;

public partial class Builder : Node3D
{
    // Set in editor
    [Export] private Node3D selector;
    [Export] private Node3D cameraScene;
    [Export] private GridMap gridmap;
    [Export(PropertyHint.Layers2DPhysics)] public uint CollisionLayers;

    //Public 

    public bool buildMode = false;

    //Private
    private Camera3D camera;
    private Vector3 buildPosition;
    private Node3D hitcollider;

    private string towerToBuild = null;
    private int toBuildCost = 0;
    //private castle playercastle;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        //selector.Scale = new Vector3(gridmap.CellSize.X, gridmap.CellSize.Y, gridmap.CellSize.Z);
        camera = cameraScene.GetNode<Camera3D>("Angle/Camera3D");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {

        if (buildMode) UpdateBuildPosition();

    }
    private void UpdateTowerPreview() {
        if (selector.GetChildCount() > 0) selector.GetChild(0).QueueFree();
        if (towerToBuild != null) {
            // change test folder to tower prewiev folder
            var scene = GD.Load<PackedScene>("res://Scenes/Tower/Preview/" + towerToBuild + "Pre.tscn");
            var inst = scene.Instantiate<Node3D>();
            selector.AddChild(inst);
        }
    }

    // PUBLIC
    public void UpdateTowerToBuild(string tower, int cost) {
        towerToBuild = tower;
        toBuildCost = cost;
        UpdateTowerPreview();
        buildMode = true;
    }
    private bool CanBuildTower() {
        if (buildMode == false) return false;
        if (selector.Position == Vector3.Zero) return false;
        if (!selector.Visible) return false;
        if (towerToBuild == null) return false;
        if (Prices.gold < toBuildCost) return false;
        return true;
    }
    private void BuildTower() {
        if (!CanBuildTower()) return;

        Prices.gold -= toBuildCost;
        // change test folder to tower folder
        var scene = GD.Load<PackedScene>("res://Scenes/Tower/" + towerToBuild + ".tscn");
        var inst = scene.Instantiate<Node3D>();
        inst.Position = selector.Position;
        inst.Rotation = selector.Rotation;
        if (selector.Visible && towerToBuild != null) {
            AddChild(inst);
        }
        //selector.Position = Vector3.Zero;
        selector.Visible = false;

        Prices.IncreaseTax(towerToBuild); buildMode = false;
    }

    // PRIVATE
    private void UpdateBuildPosition() {

        var mousePosition = GetViewport().GetMousePosition();
        var rayorgin = camera.ProjectRayOrigin(mousePosition);
        var rayend = camera.ProjectRayNormal(mousePosition);

        PhysicsRayQueryParameters3D query = new() {
            From = rayorgin,
            To = rayorgin + rayend * 2000, // * Ray Reach
            CollideWithAreas = false,
            CollideWithBodies = true,
            CollisionMask = CollisionLayers
        };

        var hitDirectory = GetWorld3D().DirectSpaceState.IntersectRay(query);


        if (hitDirectory.Count > 0) {

            Vector3 newPosition = (Vector3)hitDirectory["position"];
            hitcollider = (Node3D)hitDirectory["collider"];
            var cellCord = gridmap.LocalToMap(newPosition);
            buildPosition = gridmap.MapToLocal(cellCord) - new Vector3(0, gridmap.CellSize.Y / 2, 0);


            // Check if item under build position is buildable
            int cellItem = gridmap.GetCellItem(new Vector3I(cellCord.X, cellCord.Y - 1, cellCord.Z));

            if (cellItem == 1 && hitcollider == gridmap) {
                selector.Visible = true;
                selector.Position = buildPosition;
            }
            if (cellItem != 1) selector.Visible = false;
        }
        else selector.Visible = false;

    }

     // test build position
    public override void _UnhandledInput(InputEvent @event) {
        if (!buildMode) return;

        if (buildMode && Input.IsActionJustPressed("cancel")) {
            buildMode = false;
            towerToBuild = null;
            UpdateTowerPreview();
        }

        if ( Input.IsActionJustPressed("m1click") && buildMode) {
            BuildTower();
        }

        RotateSelector();
    }

    private void RotateSelector() {
        if (Input.IsActionJustPressed("rotate_selector_left")) {
            selector.RotationDegrees += new Vector3(0, 90, 0);
            if (selector.RotationDegrees.Y == 360)
                selector.RotationDegrees = new Vector3(0, 0, 0);

        }
        else if (Input.IsActionJustPressed("rotate_selector_right")) {
            selector.RotationDegrees += new Vector3(0, -90, 0);
            if (selector.RotationDegrees.Y == -360)
                selector.RotationDegrees = new Vector3(0,0,0);
        }

    }
}


/* OLD CODE
//Get Used cells in grid
// Godot.Collections.Array<Vector3I> gridarray = gridmap.GetUsedCells();
//
//bool cellUnder = gridarray.Contains(new Vector3I(cellCord.X, cellCord.Y - 1, cellCord.Z));
//selector.Position = selector.Position.Lerp(buildPosition, (float)GetPhysicsProcessDeltaTime() * 50);
*/
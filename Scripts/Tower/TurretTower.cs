using Godot;
using static Godot.WebSocketPeer;

public partial class TurretTower : Node3D {

    [Export] internal string name;
    internal float range = 10f;
    internal float rotationSpeed = 20f;
    [Export] internal int attack = 10;
    internal int level = 1;

    internal int upgrade_cost;
    internal float damageInterval = 0.25f;

    internal int goldInvested;

    private Node3D rotationHelperY;
    private Node3D rotationHelperX;

    [Export] internal Node3D RotationPointY;
    [Export] internal Node3D RotationPointX;
    [Export] internal Area3D area;
    [Export] internal RayCast3D rayCast;
    [Export] private CsgCylinder3D rangepreview;

    private string enemyTag = "enemy";

    internal Node3D target = null;
    private ulong targetID;

    [ExportCategory("UI")]
    [Export] internal Control info;
    [Export] internal RichTextLabel infotext;
    [Export] internal Button uRangeB;
    [Export] internal Button uDamageB;

    [Export] internal float energy = 1.5f;
    [Export] internal float penetration = 0.6f;
    [Export] internal float blood = 0.9f;


    internal TurretTower() {
        rotationHelperY = new Node3D(); AddChild(rotationHelperY);
        rotationHelperX = new Node3D(); AddChild(rotationHelperX);
    }

    internal void OnReady() {
        UpdateTurretRange();
        upgrade_cost = Prices.GetCost(name);
        UpdateTowerInfo();
    }
    internal void UpdateTurretRange() {
        rayCast.TargetPosition = new Vector3(0, 0, -range);
        // Set the shape of the CollisionShape3D to the newly created sphere shape
        var shape = new SphereShape3D();
        shape.Radius = range;
        area.GetNode<CollisionShape3D>("CollisionShape3D").Shape = shape;
        rangepreview.Radius = range;
        // also update mesh range preview
    }

    internal void UpdateTarget() {

        // Keep current target
        if (target != null && CanHitTarget()) return;

        target = null;

        // Detect objects within the detection area
        Godot.Collections.Array<Godot.Node3D> bodies = area.GetOverlappingBodies();
        

        foreach (var body in bodies) {
            // Check if the detected object is an enemy
            if (body.IsInGroup("enemy")) {
                target = body;
                targetID = target.GetInstanceId();
                break;
            }
        }
    }

    
    internal bool CanHitTarget() {
        var collider = rayCast.GetCollider();
        if (collider != null && collider.IsClass("CharacterBody3D")) {
            var currentTarget = (CharacterBody3D)rayCast.GetCollider();
            if (currentTarget.IsInGroup("enemy")) return true;
            
        }
         return false;
    }

    // rotate needs better preformance
    internal void RotateYaxis(double delta) {
        if ( target == null || !IsInstanceIdValid(targetID) ) return;

        var dir = (target.GlobalPosition - RotationPointY.GlobalPosition).Normalized();
        rotationHelperY.LookAtFromPosition(RotationPointY.Position, dir);        

        Quaternion currentrotation = RotationPointY.Basis.GetRotationQuaternion();
        Quaternion desiredrotation = rotationHelperY.Basis.GetRotationQuaternion();        
        Quaternion rotate = currentrotation.Slerp(desiredrotation, (float)(rotationSpeed * delta));
        RotationPointY.Quaternion = new Quaternion(0,rotate.Y, 0, rotate.W);        
    }

    internal void RotateXaxis(double delta) {
        if ( target == null || !IsInstanceIdValid(targetID) ) return;

        var dir =(target.GlobalPosition - RotationPointX.GlobalPosition).Normalized();
        rotationHelperX.LookAtFromPosition(RotationPointX.Position, dir);

        Quaternion currentrotation = RotationPointX.Basis.GetRotationQuaternion();
        Quaternion desiredrotation = rotationHelperX.Basis.GetRotationQuaternion();
        Quaternion rotate = currentrotation.Slerp(desiredrotation, (float)(rotationSpeed * delta));
        RotationPointX.Quaternion = new Quaternion(rotate.X, 0, 0, rotate.W);
    }


    // FAIL: basis.looking at does not work for rotations within a rotation NOT GOOD ENOUGH Only Use in 2D.
    internal void RotateBothAxis(float delta) {
        if (target == null || !IsInstanceIdValid(targetID)) return;

        var dir = (target.GlobalPosition - RotationPointY.GlobalPosition).Normalized();
        var rot = Basis.LookingAt(dir, Vector3.Up);

        Quaternion currentrotationY = RotationPointY.Basis.GetRotationQuaternion();
        Quaternion desiredrotationY = rot.GetRotationQuaternion();
        Quaternion rotate = currentrotationY.Slerp(desiredrotationY, rotationSpeed * delta);
        RotationPointY.Quaternion = rotate;
    }


    /// <summary>
    /// UI stuff here
    /// </summary>
    private void _on_upgrade_damage_pressed() {
        if (upgrade_cost > Prices.gold) return;
        Prices.gold -= upgrade_cost;
        level++;
        goldInvested += upgrade_cost;
        upgrade_cost = (int)(Prices.GetCost(name) * Mathf.Pow(1.5, level - 1));
        attack += (int)(attack / 2.5f);
        UpdateTowerInfo();
    }
    private void _on_upgrade_range_pressed() {
        if (upgrade_cost > Prices.gold) return;
        Prices.gold -= upgrade_cost;
        level++;
        goldInvested += upgrade_cost;
        upgrade_cost = (int)(Prices.GetCost(name) * Mathf.Pow(1.5, level - 1));
        range *= 1.3f;
        UpdateTowerInfo();
        UpdateTurretRange();
    }
    private void _on_sell_pressed() {
        info.Visible = false;
        Prices.gold += GetSellPrize();
        Prices.ReduceTax(name);

        QueueFree();
    }


    private void _on_control_gui_input(Variant @event) {
        if (Input.IsActionJustPressed("cancel") || Input.IsActionJustPressed("m1click")) {
            info.Visible = false;
            rangepreview.Visible = false;
        }
    }
    private void _on_static_body_3d_input_event(Variant camera, Variant @event, Vector3 position, Vector3 normal, int shape_idx) {
        if (Input.IsActionJustPressed("m1click")) {
            UpdateTowerInfo();
            info.Visible = true;
            rangepreview.Visible = true;
        }
    }
    private int GetSellPrize() {
        int selltax = (int)(goldInvested * 0.8f);
        int buyprice = Prices.GetCost(name, true) - Prices.GetCost(name);
        int sellprize = selltax + buyprice;
        return sellprize;
    }

    private void UpdateTowerInfo() {
        infotext.Clear();
        infotext.AppendText("[b]" + name + "[/b]   " + "LV: " + level + "\n");

        var goldicon = new Texture2D();
        var bloodicon = new Texture2D();
        var punctureicon = new Texture2D();
        var electrcicon = new Texture2D();
        goldicon = GD.Load<Texture2D>("res://Assets/Icons/Goldsmall.png");
        bloodicon = GD.Load<Texture2D>("res://Assets/Icons/sBlood.png");
        punctureicon = GD.Load<Texture2D>("res://Assets/Icons/sPuncture.png");
        electrcicon = GD.Load<Texture2D>("res://Assets/Icons/sEnergyIcon.png");

        infotext.AddText("Damage:\n");
        infotext.AddImage(bloodicon);
        infotext.AddText(" " + (attack * blood).ToString("0.") + " ");
        infotext.AddImage(punctureicon);
        infotext.AddText(" " + (attack * penetration).ToString("0.") + " ");
        infotext.AddImage(electrcicon);
        infotext.AddText(" " + (attack * energy).ToString("0.") + "\n\n");

        infotext.AddText("Sell:  ");
        infotext.AddImage(goldicon);
        infotext.AddText("  " + GetSellPrize());
        infotext.AddText("\nUpgrade cost:  ");
        infotext.AddImage(goldicon);
        infotext.AddText("  " + upgrade_cost);
    }
}

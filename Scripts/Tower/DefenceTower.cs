using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

public partial class DefenceTower : Tower {

    [ExportCategory("Aim")]
    [Export] protected RayCast3D raycast;
    [Export] private Node3D RotationPointY;
    [Export] private Node3D RotationPointX;
    [Export] private OptionButton smartTarget;
    [Export] protected Area3D area;
    [Export] protected float damageInterval = 1f;

    [ExportCategory("Effects")]
    [Export] protected GpuParticles3D shooteffect;
    [Export] protected AudioStreamPlayer3D sound;

    private Node3D rotationHelperY;
    private Node3D rotationHelperX;
    private float rotationSpeed = 25f;
    private string target_priorety = "First in range";

    private protected Godot.Collections.Array<Godot.Node3D> bodies;


    // Called when the node enters the scene tree for the first time.

    internal DefenceTower() {
        rotationHelperY = new Node3D(); AddChild(rotationHelperY);
        rotationHelperX = new Node3D(); AddChild(rotationHelperX);
    }
    public override void _Ready() {
        AddToGroup("AttackTower");
        StartTower();
        AddTargetOptions();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
        UpdateButtons();        
        SelectTargetPriorety();
        DoDamage(delta);
        //if (!TurnHandler.isTurnActive) return;
        RotateYaxis(delta);
        RotateXaxis(delta);        
    }
    internal override void UpdateTurretRange() {
        raycast.TargetPosition = new Vector3(0, 0, -range);

        var rangepreview = (CsgCylinder3D)rangePreview;
        // Set the shape of the CollisionShape3D to the newly created sphere shape
        var shape = new SphereShape3D();
        shape.Radius = range;
        area.GetNode<CollisionShape3D>("Detection").Shape = shape;
        rangepreview.Radius = range;
    }

    internal virtual void DoDamage(double delta) {
        //shooteffect.Visible = false;
        damageTimer += (float)delta;
        if (CanHitTarget()) {
            
            if (damageTimer >= damageInterval) {
                shooteffect.Emitting = true;
                sound.Play();
                var currentTarget = (Enemy)raycast.GetCollider();
                if (currentTarget.TakeDamage(attack, blood, puncture, energy)) {
                    target = null;
                    //GD.Print("Turret got a kill");
                }
                
                damageTimer = 0;
            }
        }
    }


    internal bool CanHitTarget() {
        var collider = raycast.GetCollider();
        if (collider != null && collider.IsClass("CharacterBody3D")) {
            var currentTarget = (CharacterBody3D)raycast.GetCollider();
            if (currentTarget.IsInGroup("enemy")) return true;

        }
        return false;
    }
    // rotate needs better preformance
    internal void RotateYaxis(double delta) {
        if (target == null || !IsInstanceIdValid(targetID)) return;

        var dir = (target.GlobalPosition - RotationPointY.GlobalPosition).Normalized();
        rotationHelperY.LookAtFromPosition(RotationPointY.Position, dir);

        Quaternion currentrotation = RotationPointY.Basis.GetRotationQuaternion();
        Quaternion desiredrotation = rotationHelperY.Basis.GetRotationQuaternion();
        Quaternion rotate = currentrotation.Slerp(desiredrotation, (float)(rotationSpeed * delta));
        RotationPointY.Quaternion = new Quaternion(0, rotate.Y, 0, rotate.W);
    }

    internal void RotateXaxis(double delta) {
        if (target == null || !IsInstanceIdValid(targetID)) return;

        var dir = (target.GlobalPosition - RotationPointX.GlobalPosition).Normalized();
        rotationHelperX.LookAtFromPosition(RotationPointX.Position, dir);

        Quaternion currentrotation = RotationPointX.Basis.GetRotationQuaternion();
        Quaternion desiredrotation = rotationHelperX.Basis.GetRotationQuaternion();
        Quaternion rotate = currentrotation.Slerp(desiredrotation, (float)(rotationSpeed * delta));
        RotationPointX.Quaternion = new Quaternion(rotate.X, 0, 0, rotate.W);
    }

    // simple rotation for preformace when there are over 100
    private void simplerotate() {
        if (target == null || !IsInstanceIdValid(targetID)) return;
        RotationPointX.LookAt(target.GlobalPosition, Vector3.Right);
        RotationPointY.LookAt(target.GlobalPosition, Vector3.Up);
    }

    internal void AddTargetOptions() {
        smartTarget.AddItem("First in range");
        smartTarget.AddItem("Progress");
        smartTarget.AddItem("Blood");
        smartTarget.AddItem("Puncture");
        smartTarget.AddItem("Energy");
        smartTarget.AddItem("Closest");
        smartTarget.AddItem("Speed");

        if (Upgrades.smart_turret == false) {
            for (int i = 0; i < smartTarget.ItemCount - 1; i++)
                smartTarget.SetItemDisabled(i+1, true);
        }
    }
    public void EnableTargetPriorety() {
        if (smartTarget == null) return;
        for (int i = 0; i < smartTarget.ItemCount - 1; i++)
            smartTarget.SetItemDisabled(i + 1, false);
    }
    private void _on_option_button_item_selected(int id) {
        target_priorety = smartTarget.GetItemText(id);
    }

    //upgrade Smart Turret
    internal void SelectTargetPriorety() {
        // Detect objects within the detection area
        bodies = area.GetOverlappingBodies();
        ExcludeTargets();

        switch (target_priorety) {
            case "Progress":
                SelectTargetProgress(); break;
            case "First in range":   // glues to one target
                SelectTargetFirst(); break;
            case "Closest":
                SelectTargetClosest(); break;
            case "Blood":
                SelectTargetHealth(target_priorety); break;
            case "Puncture":
                SelectTargetHealth(target_priorety); break;
            case "Energy":
                SelectTargetHealth(target_priorety); break;
            case "Speed": SelectTargetSpeed(); break;
        }
    }

    protected virtual void ExcludeTargets() { }
    private void SelectTargetFirst() {
        // Keep current target
        if (target != null && CanHitTarget()) return;
        target = null;

        foreach (var body in bodies) {
            if (!body.IsInGroup("enemy")) continue;
            target = (Enemy)body;
            targetID = target.GetInstanceId();
            break;
        }
    }
    private void SelectTargetProgress() {
        target = null;
        float progress = Mathf.Inf;

        foreach (var body in bodies) {
            if (!body.IsInGroup("enemy")) continue;
            
            var agent = body.GetNode<NavigationAgent3D>("NavigationAgent3D");
            float agentprogress = agent.DistanceToTarget();  // not work in maces, need distace traveled.

            //Select target based on progress
            if (agentprogress < progress) {
                progress = agentprogress;
                target = (Enemy)body;
                targetID = target.GetInstanceId();
            }
        }
    }
    private void SelectTargetClosest() {
        target = null;
        float distance = Mathf.Inf;        

        foreach (var body in bodies) {
            if (!body.IsInGroup("enemy")) continue;

            float agentdistance = raycast.Position.DistanceTo(body.Position); // ?? Raycast before rotation???
            if (agentdistance < distance) {
                distance = agentdistance;
                target = (Enemy)body;
                targetID = target.GetInstanceId();
            }
        }
    }

    private void SelectTargetHealth(string type) { // I dont know why this dont work
        target = null;
        float healthtype = -1;

        foreach (var body in bodies) {
            if (!body.IsInGroup("enemy")) continue;
            Enemy enemy = (Enemy)body;

            float enemyhealthtype = 0;
            switch (type) {
                case "energy": enemyhealthtype = enemy.energy_health_now; break;
                case "puncture": enemyhealthtype = enemy.penetration_health_now; break;
                case "blood": enemyhealthtype = enemy.blood_health_now; break;
            }

            if (enemyhealthtype > healthtype) {
                healthtype = enemyhealthtype;
                target = (Enemy)body;
                targetID = target.GetInstanceId();
            }
        }
    }

    private void SelectTargetSpeed() {
        target = null;
        float speed = 0;

        foreach (var body in bodies) {
            if (!body.IsInGroup("enemy")) continue;

            Enemy enemy = (Enemy)body;
            float enemyspeed = enemy.max_speed;


            if (enemyspeed > speed) {
                speed = enemyspeed;
                target = (Enemy)body;
                targetID = target.GetInstanceId();
            }
        }
    }
}

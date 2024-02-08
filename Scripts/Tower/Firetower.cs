using Godot;
using System;
using System.Numerics;
using System.Reflection.Emit;

public partial class Firetower : DefenceTower {

    private PackedScene burnStatusEffect = GD.Load<PackedScene>("res://Scenes/Tower/projectiles/BurnStatusEffect.tscn");

    internal override void UpdateTurretRange() {
        var shape = new BoxShape3D();
        var rangepreview = (CsgBox3D)rangePreview;

        shape.Size = new Godot.Vector3(3,2,range);
        rangepreview.Size = shape.Size;
        area.Position = new Godot.Vector3(0, 0, -range/2 - 1f);

        area.GetNode<CollisionShape3D>("Detection").Shape = shape;  
        shooteffect.Lifetime = 1/(50 / range);
    }

    public override void _Ready() {
        AddToGroup("AttackTower");
        StartTower();
    }
	public override void _Process(double delta) {
        UpdateTarget();
        UpdateButtons();
    }
    private void UpdateTarget() {

        // Detect objects within the detection area
        Godot.Collections.Array<Godot.Node3D> bodies = base.area.GetOverlappingBodies();
        if (bodies.Count < 1) { shooteffect.Emitting = false; sound.Playing = false; return; }
        shooteffect.Emitting = true;
        if (sound.Playing == false) sound.Playing = true;



        foreach (var body in bodies) {
            // Check if the detected object is an enemy
            if (body.IsInGroup("enemy")) {
                target = (Enemy)body;
                targetID = target.GetInstanceId();
                BurnEnemy(target);
            }

        }
    }

    private void BurnEnemy(Enemy enemy) {
        if (!IsInstanceIdValid(targetID)) return;
        var damage = (float)(attack * GetProcessDeltaTime());
        //var damage = (float)(attack * GetPhysicsProcessDeltaTime());
        enemy.TakeDamage(damage, blood, puncture, energy);
    } 

    private void _on_area_3d_body_entered(Node3D body) {

        // Do nothing if we dont have upgrade, its not an enemy or enemy is already burned
        if (!Upgrades.BurnStatus) return;
        if (!body.IsInGroup("enemy")) return;
        if (body.GetNodeOrNull<GpuParticles3D>("FireStatusEffect") != null) return;

        var status_effect = burnStatusEffect.Instantiate<Curse>();
        status_effect.Initialize(5, 0, attack, blood, puncture, energy);
        body.AddChild(status_effect);
    }

}

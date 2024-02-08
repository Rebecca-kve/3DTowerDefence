using Godot;
using System;

public partial class PlagueTower : DefenceTower {

	private PackedScene Plague = GD.Load<PackedScene>("res://Scenes/Tower/projectiles/curse.tscn");
    private PackedScene projectile = GD.Load<PackedScene>("res://Scenes/Tower/projectiles/plaugeball.tscn");

    private int Second = 5;
    private int Slow = 50;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
        UpdateButtons();
        SelectTargetPriorety();
        DoDamage(delta);
    }
    internal override void DoDamage(double delta) {

        damageTimer += (float)delta;

        if (!IsInstanceIdValid(targetID)) target = null;
        if (target == null) return;
        if (target.GetNodeOrNull<GpuParticles3D>("Curse") != null) return;
            
        if (damageTimer >= damageInterval) {

            var arrow = projectile.Instantiate<Projectile>();
            raycast.AddChild(arrow);
            //arrow.statusEffectPath = Plague;
            arrow.Initialize(0, 0, 0, 0, target);

            var status_effect = Plague.Instantiate<Curse>();
            status_effect.Initialize(Second, Slow, attack, blood, puncture, energy);
            target.AddChild(status_effect);

            damageTimer = 0;
        }       
    }
    protected override void ExcludeTargets() {
        for (int i = 0; i < bodies.Count; i++) {
            if (!bodies[i].IsInGroup("enemy")) continue;

            if (bodies[i].GetNodeOrNull<GpuParticles3D>("Curse") != null) {
                bodies.Remove(bodies[i]); i--;
                if (i == bodies.Count) break;
                //GD.Print("Removed");
            }
        }
    }
    private protected override void _on_upgrade_damage_pressed() {
        base._on_upgrade_damage_pressed();
        Second += 1;
        if (Slow < 90) Slow += 5;
        UpdateTowerInfo();
    }
    internal override void ExtraInfo() {
        infotext.AddText("Slow: " + Slow + "%\n");
        infotext.AddText("Plague Time: " + Second);
    }
}

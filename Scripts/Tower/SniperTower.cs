using Godot;
using System;

public partial class SniperTower : DefenceTower {

    private PackedScene projectile = GD.Load<PackedScene>("res://Scenes/Tower/projectiles/arrow.tscn");

    internal override void DoDamage(double delta) {
        damageTimer += (float)delta;
        if (CanHitTarget()) {

            if (damageTimer >= damageInterval) {
                var arrow = projectile.Instantiate<Projectile>();
                raycast.AddChild(arrow);
                sound.Play();

                var currentTarget = (Enemy)raycast.GetCollider();
                if (Upgrades.ExplosiveArrows) arrow.splashDamage = true;
                arrow.Initialize(attack, blood, puncture,energy, currentTarget);

                damageTimer = 0;
            }
        }
    }

    /*/ Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	*/
}

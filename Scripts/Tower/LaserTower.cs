using Godot;
using System;
using System.Reflection.Emit;

public partial class LaserTower : DefenceTower {

    // Laser
    [Export] private laser beam;

    private Enemy currentTarget;
    private float damage = 0;

    internal override void DoDamage(double delta) {
        
        if (CanHitTarget()) {
            //var damage = (float)(attack / Engine.GetFramesPerSecond());
            var damage = DamageOverTimeBonus(delta);
            var currentTarget = (Enemy)raycast.GetCollider();
            if (currentTarget.TakeDamage(damage, blood, puncture, energy)) {
                target = null;
                //GD.Print("Turret got a kill");
            }
        }
        LaserBeamDisplay();
    }

    private float DamageOverTimeBonus(double delta) {
        if (!Upgrades.laserTime) return (float)(attack * delta);
        if (damage == 0) damage = attack;

        if (currentTarget == null || currentTarget != target) {
            currentTarget = target;
            damage = attack;
        }
        else if (currentTarget == target) damageTimer += (float)delta;

        if (damageTimer >= damageInterval) {
            damage *= 1.5f;
            damageTimer = 0;
        }

        return (float)(damage * delta);
    }

    private void LaserBeamDisplay() {

        if (target == null) { beam.HideLaser(); sound.Playing = false; }
        else if (raycast.IsColliding()) {
            beam.DisplayLaser();
            if (sound.Playing == false) sound.Playing = true;
        }
        else { beam.HideLaser(); sound.StreamPaused = true; }
    }



}
/*
 *     private float CalculateDamage(float duration) {
        // Calculate damage proportionally to the contact duration
        return attack * (float)Mathf.Clamp(duration, damageInterval, maxContactDuration);
    }
private void DoDamage() {
    LaserBeamDisplay();
    if (CanHitTarget()) {
        damageTimer += (float)GetPhysicsProcessDeltaTime();
        if (damageTimer >= damageInterval) {
            var currentTarget = (Enemy)rayCast.GetCollider();
            if (currentTarget.NewTakeDamage(attack, blood, penetration, energy)) {
                target = null;
                //GD.Print("Turret got a kill");
            }
            damageTimer = 0.0f;
        }
    }

    private void LaserDamageOnContact() {
        LaserBeamDisplay();

        if (CanHitTarget()) {
            StartLaser();

            damageTimer += (float)GetPhysicsProcessDeltaTime();
            if (damageTimer >= damageInterval) {
                StopLaser();
                damageTimer = 0.0f;
            }
        }
    }
    

    public void StartLaser() {
        if (!CanHitTarget()) return;

        //isLaserActive = true;
        contactStartTime = (float)Time.GetTicksMsec() / 1000.0f;
    }

    public void StopLaser() {
        //isLaserActive = false;
        float contactDuration = ((float)Time.GetTicksMsec() / 1000.0f) - contactStartTime;
        float damage = CalculateDamage(contactDuration);
        var currentTarget = (Enemy)rayCast.GetCollider();
        if (currentTarget.TakeDamage(damage)) {
            target = null;
            GD.Print("Turret got a kill");
        }

        //if (CanHitTarget()) {}
    }
}*/
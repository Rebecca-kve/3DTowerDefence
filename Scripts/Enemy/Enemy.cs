using Godot;
using System;
using System.Net.Security;
using static System.Net.Mime.MediaTypeNames;

public partial class Enemy : CharacterBody3D {

    [ExportCategory("Property")]      
    [Export] private float start_speed = 10.0f;    
    [Export] public int attack = 1;
    [Export] private string split;

    [ExportCategory("Health")]
    [Export] private HealthBar3D healtbar;
    [Export] private float start_blood_health = 10;
    [Export] private float start_penetrate_health = 10;
    [Export] private float start_energy_health = 10;
    [Export] private int regeneration = 0;
    [Export] private bool layered_health = false;
    //helper
    private const int max_level = 100;
    private bool isalive = true;

    // Upgrade
    private int level = 1;
    private readonly float upgrade_bonus = 1.15f;
    private readonly float upgrade_speed_bonus = 1.007f;

    //max stats
    public float max_speed;
    private float max_blood_health;
    private float max_penetrate_health;
    private float max_energy_health;

    // current stats
    public float speed_now;
    public float blood_health_now;
    public float penetration_health_now;
    public float energy_health_now;

    private PackedScene explosion = GD.Load<PackedScene>("res://Assets/Particles/explosion.tscn");

    private Enemy() { // Sadly cant use public Enemy(int lv) here
        Initialize(level);
    }
    public void Initialize(int lv) {
        level = lv;
        max_speed = start_speed * Mathf.Pow(upgrade_speed_bonus, lv - 1);
        max_blood_health = start_blood_health * Mathf.Pow(upgrade_bonus, lv - 1);
        max_penetrate_health = start_penetrate_health * Mathf.Pow(upgrade_bonus, lv - 1);
        max_energy_health = start_energy_health * Mathf.Pow(upgrade_bonus, lv -1);
        regeneration = (int)(regeneration * Mathf.Pow(upgrade_bonus, lv - 1));
    }
    public override void _Ready() {
        speed_now = max_speed;
        blood_health_now = max_blood_health;
        penetration_health_now = max_penetrate_health;
        
        healtbar.text.Text = "LV: " + level.ToString();
        penetration_health_now = max_penetrate_health;
        energy_health_now = max_energy_health;

        UpdateHealth();
    }
    public override void _Process(double delta) {
        HealthRegeneration(delta);
    }
    public void Slow(int slow) {
        float new_speed = speed_now - (speed_now * slow / 100);
        speed_now = new_speed;
    }
    public void ResetSpeed(int slow) {
        float new_speed = speed_now + speed_now / (slow / 100);
        if (new_speed > max_speed) speed_now = max_speed;
        else speed_now = new_speed;
    }
    private bool TakeDamageunlayered(float damage, float blood, float penetration, float energy) {
        if (!isalive) return false;
        float damage_left = damage * blood;
        float b_left = damage * blood;
        float p_left = damage * penetration;
        float e_left = damage * energy;


        if (energy_health_now > 0) { // energy damage
            energy_health_now -= e_left;
            if (energy_health_now <= 0) energy_health_now = 0;
        }

        if (penetration_health_now > 0 ) { // penetration damage
            penetration_health_now -= p_left;
            if (penetration_health_now <= 0) penetration_health_now = 0;
        }
        if (blood_health_now > 0) { // blood damage
            blood_health_now -= b_left;
            if (blood_health_now <= 0) blood_health_now = 0;
        }
        var life = blood_health_now + penetration_health_now + energy_health_now;

        UpdateHealth();

        if (life <= 0) {
            if (blood_health_now <= 0) { Die(); return true; }
        }
        return false;
    }
    private bool TakeDamageHealhlayered(float damage, float blood, float penetration, float energy) {
        if (!isalive) return false;
        float damage_left = damage;


        if (energy_health_now > 0) { // energy damage
            damage_left *= energy;
            float s = energy_health_now;
            energy_health_now -= damage_left;
            damage_left -= s / energy;
            if (energy_health_now <= 0) energy_health_now = 0;
            healtbar.UpdateEnergyHealthBar(energy_health_now, max_energy_health);
            if (damage_left <= 0) return false;
        }
        // only break armor after shield
        if (penetration_health_now > 0 && energy_health_now <= 0) { // penetration damage
            damage_left *= penetration;
            float a = penetration_health_now;
            penetration_health_now -= damage_left;
            damage_left -= a / penetration;
            if (penetration_health_now <= 0) penetration_health_now = 0;
            healtbar.UpdatePenetrateHealthBar(penetration_health_now, max_penetrate_health);
            if (damage_left <= 0) return false;
        }

        // only brak health after armor
        damage_left *= blood;                    // bleed damage
        blood_health_now -= damage_left;
        healtbar.UpdateBloodHealthBar(blood_health_now, max_blood_health);

        if (blood_health_now <= 0) { Die(); return true; }
        return false;
    }

    public bool TakeDamage(float damage, float blood, float penetration, float energy) {
        if (layered_health) return TakeDamageHealhlayered(damage, blood, penetration, energy);
        else return TakeDamageunlayered(damage, blood, penetration, energy);
    }

    private void UpdateHealth() {
        healtbar.UpdateBloodHealthBar(blood_health_now, max_blood_health);
        healtbar.UpdatePenetrateHealthBar(penetration_health_now, max_penetrate_health);
        healtbar.UpdateEnergyHealthBar(energy_health_now, max_energy_health);
    }

    private void Die() {
        if (!isalive) return;
        isalive = false;
        Split();
        TurnHandler.EnemyRemovedThisTurn++;
        DeathEffect();
        QueueFree();
    }
    private void DeathEffect() {        
        var effect = explosion.Instantiate<GpuParticles3D>();
        effect.Position = Position;
        GetParent<Node3D>().AddChild(effect);
        effect.Emitting = true;
    }
    private void Split() {
        if (split == null) return;
        TurnHandler.EnemyCountThisTurn++;
        var scene = GD.Load<PackedScene>("res://Scenes/Enemy/" + split + ".tscn");
        var inst = scene.Instantiate<Enemy>();
        inst.Position = Position;
        inst.Initialize(level);
        Node3D spawn = GetParent<Node3D>();
        spawn.AddChild(inst, true);
    }
    private void HealthRegeneration(double delta) {
        if (regeneration <= 0) return;
        //string result = (time < 18) ? "Good day." : "Good evening.";
        float regen = (float)(regeneration * delta);
        if (blood_health_now < max_blood_health) blood_health_now += regen;
        if (penetration_health_now < max_penetrate_health) penetration_health_now += regen;
        if (energy_health_now < max_energy_health) energy_health_now += regen;
        UpdateHealth();
    }
}
// Not used Code
/*
private void _on_navigation_agent_3d_navigation_finished() {
    if (!isalive) return;
    isalive = false;
    castle target = (castle)GetTree().Root.GetNode("Level_01").GetNode<Node3D>("Castle");
    target.TakeDamage(damage);
}
    public void SetRandomUpgrades( int l ) { // used by automatic portals
    if (l > max_level || l <= 0) return;
    level = l;

    var random = new RandomNumberGenerator();
    random.Randomize();
    var h_upgrade = random.RandiRange(1, level);
    var s_upgrade = random.RandiRange(0, level- h_upgrade);
    var a_upgrade = level - h_upgrade - s_upgrade;

    UpgradeSpeed(s_upgrade);
    UpgradeHealth(h_upgrade);
    UpgradeArmor(a_upgrade);
}
public void ApplyeUpgrades(int s, int h, int a) { // speed, health, armor
    if (s + h + a > max_level) return;
    UpgradeSpeed(a); UpgradeHealth(h); UpgradeArmor(a);
    level = s + h + a;
}


private void UpgradeSpeed(int Upgrades) {
    speed = start_speed * Mathf.Pow(upgrade_speed_bonus, Upgrades);
}
private void UpgradeHealth(int Upgrades) {
    health = start_health * Mathf.Pow(upgrade_bonus, Upgrades);

}
private void UpgradeArmor(int Upgrades) {
    armor = (int)( start_armor * Mathf.Pow(upgrade_bonus, Upgrades) );
}
    public bool TakeDamageHealhpriorety(int damage, float bleed, float penetration, float energy) {
        if (!isalive) return false;
        float damage_left = damage;


        if (current_shield > 0) { // energy damage
            damage_left *= energy;
            float s = current_shield;
            current_shield -= damage_left;
            damage_left = damage_left - s / energy;
            if (damage_left <= 0) return false;
        }
        // only break armor after shield
        if (current_armor > 0 && current_shield <= 0) { // penetration damage
            damage_left *= energy;
            float a = current_armor;
            current_armor -= damage_left;
            damage_left = damage_left - a / penetration;
            if (damage_left <= 0) return false;
        }

        UpdateHealth();
        // only brak health after armor
        damage_left *= bleed;                    // bleed damage
        current_health -= damage_left;
        healtbar.UpdateHealthBar((int)current_health, (int)max_health);

        if (current_health <= 0) { Die(); return true; }
        return false;
    }




    public bool TakeDamagePerSecond(float damage, float timeInterval) {
        if (!isalive) return false;
        float damageLeft = damage;

        // Armor
        if (start_armor >= damageLeft) damageLeft = 1;
        else damageLeft -= start_armor;

        // Calculate damage per second
        float damagePerSecond = damageLeft / timeInterval;

        blood_health_now -= damagePerSecond * timeInterval;
        healtbar.UpdateBloodHealthBar((int)blood_health_now, (int)max_health);

        if (blood_health_now <= 0) { Die(); return true; }
        return false;
    }

    public bool TakeDamage(float damage) {
        if (!isalive) return false;
        float damageLeft = damage;
        // Armor
        if (start_armor >= damageLeft) damageLeft = 1;
        else damageLeft -= start_armor;

        blood_health_now -= damageLeft;
        healtbar.UpdateBloodHealthBar((int)blood_health_now, (int)max_health);

        if (blood_health_now <= 0) { Die(); return true; }
        return false;
    }
*/
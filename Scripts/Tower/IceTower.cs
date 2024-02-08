using Godot;


public partial class IceTower : DefenceTower {
    // Called when the node enters the scene tree for the first time.
    private int freezeChance = 20;
    private int freezeTime = 5;


    private PackedScene freeze = GD.Load<PackedScene>("res://Scenes/Tower/projectiles/frz.tscn");
    public override void _Ready() {
        AddToGroup("IceTower");
        AddToGroup("AttackTower");
        StartTower();
    }
    internal override void UpdateTurretRange() {
        var shape = new BoxShape3D();
        var rangepreview = (CsgBox3D)rangePreview;

        shape.Size = new Vector3(range, 2, range);
        rangepreview.Size = shape.Size;

        area.GetNode<CollisionShape3D>("Detection").Shape = shape;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)	{
        UpdateTarget();
        UpdateButtons();
    }

    private void UpdateTarget() {

        // Detect objects within the detection area
        Godot.Collections.Array<Godot.Node3D> bodies = area.GetOverlappingBodies();
        if (bodies.Count < 1) { shooteffect.Emitting = false; sound.Playing = false; return; }
        shooteffect.Emitting = true;
        if (sound.Playing == false) sound.Playing = true;



        foreach (var body in bodies) {
            // Check if the detected object is an enemy
            if (body.IsInGroup("enemy")) {
                target = (Enemy)body;
                targetID = target.GetInstanceId();
                var damage = (float)(attack * GetProcessDeltaTime());
                target.TakeDamage(damage, blood, puncture, energy);
            }

        }
    }

    // slow effect
    private void _on_area_3d_body_entered(Node3D body) {
        if (!body.IsInGroup("enemy")) return;
        var enemy = (Enemy)body;

        var random = new RandomNumberGenerator();
        random.Randomize();
        if (random.RandiRange(1, 100) <= freezeChance) {

            if (enemy.GetNodeOrNull<GpuParticles3D>("FRZ") != null) return;
            var status_effect = freeze.Instantiate<Curse>();
            status_effect.Initialize(freezeTime, 90, 0);
            enemy.AddChild(status_effect);
        }
    }
    private protected override void _on_upgrade_damage_pressed() {
        base._on_upgrade_damage_pressed();
        freezeChance += 2;
        UpdateTowerInfo();
    }
    private protected override void _on_upgrade_range_pressed() {
        base._on_upgrade_range_pressed();
        freezeTime += 2;
        UpdateTowerInfo();
    }
    internal override void ExtraInfo() {
        infotext.AddText("Freeze chance: " + freezeChance + "%\n");
        infotext.AddText("Freeze time: " + freezeTime);
    }
}

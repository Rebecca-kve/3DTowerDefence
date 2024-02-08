using Godot;

public partial class Projectile : Area3D {

	[Export] private int speed = 30;
    [Export] private bool homing = true;
    [Export] private int homingSpeed = 10;      // 0 = inf
    [Export] private int time = 5;
    [Export] private Area3D Explosion;
    [Export] public bool splashDamage = false;

    //public string statusEffectPath;
    //private PackedScene statusEffect;
    private Enemy target;    

    private float Damage = 50;
    private float Blood = 1;
    private float Puncture = 1;
    private float Energy = 1;

    private double timeout = 0;
    private ulong targetID;

    public void Initialize(int damage, float blood = 1, float puncture = 1, float energy = 1, Enemy enemy = null) {
        Damage = damage;
        Blood = blood;
        Puncture = puncture;
        Energy = energy;
        target = enemy;

        if (target != null) targetID = target.GetInstanceId();
        Explosion.GetNode<CollisionShape3D>("CollisionShape3D").Disabled = (splashDamage) ? false : true;
        //if (statusEffectPath != null) statusEffect = GD.Load<PackedScene>(statusEffectPath);
    }

    public override void _Ready() {
        TopLevel = true;       
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
        
        if (homing) Homing(delta);

        // add reduce velocity over time
        Vector3 moveDir = Transform.Basis * new Vector3(0, 0, -1);
        Position += moveDir * (float)(speed * delta);        

        // Remove projectile if it misses.
        timeout += delta; if (timeout >= time) QueueFree();
    }

    private void Homing(double delta) {
        
        if (target == null) return;
        if (!IsInstanceIdValid(targetID)) { target = null; return; }

        var dir = (target.GlobalPosition - GlobalPosition).Normalized();
        var rotate = Basis.LookingAt(dir);
        Quaternion currentrotation = Basis.GetRotationQuaternion();
        Quaternion desiredrotation = rotate.GetRotationQuaternion();
        Quaternion = currentrotation.Slerp(desiredrotation, (float)(homingSpeed * delta));
        //Basis = Basis.Slerp(rotate, (float)(rotationSpeed * delta));
    }

    private void _on_body_entered(Node3D body) { // her is the prob
        
        if (body.IsInGroup("enemy")) {
            Enemy enemy = (Enemy)body;
            enemy.TakeDamage(Damage, Blood, Puncture, Energy);

        }


        QueueFree();
    }

    private void _on_explosion_body_entered(Node3D body) {
        if (body.IsInGroup("enemy")) {
            Enemy enemy = (Enemy)body;
            enemy.TakeDamage(Damage / 4, Blood, Puncture, Energy);
        }
    }

}

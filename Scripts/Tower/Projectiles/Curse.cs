using Godot;
using System;


// negative status effects
public partial class Curse : GpuParticles3D {

	
	[Export] private float Damage = 0;
	private float Blood = 1;
	private float Puncture = 1;
	private float Energy = 1;
    [Export] private int Slow = 50;
	[Export] private int Seconds = 10;

	private double SecondTimer = 0;
    private Enemy cursedEnemy;

    public void Initialize(int seconds, int slow, int damage, float blood = 1, float puncture = 1, float energy = 1) {
		Seconds = seconds;
        Slow = slow;
        Damage = damage;
        Blood = blood;
        Puncture = puncture;
		Energy = energy;		
	}

    public override void _Ready() {
        cursedEnemy = GetParent<Enemy>();
        if (Slow > 0) cursedEnemy.Slow(Slow);
    }
  
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        float damage = (float)(Damage * delta);
        cursedEnemy.TakeDamage(damage, Blood, Puncture, Energy);

        SecondTimer += delta;
        if (SecondTimer >= Seconds && !IsQueuedForDeletion()) {
            cursedEnemy.ResetSpeed(Slow);
            QueueFree();
            
		}
	}
}

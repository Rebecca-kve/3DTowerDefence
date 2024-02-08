using Godot;
using System;

public partial class HealthBar3D : Sprite3D
{
	[Export] private SubViewport subViewport;
	[Export] private TextureProgressBar bloodbar;
    [Export] private TextureProgressBar penetratebar;
    [Export] private TextureProgressBar energybar;
    [Export] public Label text;


    [ExportCategory("Icons")]
    [Export] private TextureRect bloodIcon;
    [Export] private TextureRect punctureIcon;
    [Export] private TextureRect energyIcon;

    public void UpdateBloodHealthBar(float current, float max) {
        bloodbar.MaxValue = max + 0.01;
		bloodbar.Value = current;
        //Texture = subViewport.GetTexture();
        if (bloodIcon != null) bloodIcon.Visible = (bloodbar.Value < 1) ? false : true;
    }
    public void UpdateEnergyHealthBar(float current, float max) {
        energybar.MaxValue = max + 0.01;
        energybar.Value = current;
        //Texture = subViewport.GetTexture();
        energyIcon.Visible = (energybar.Value < 1) ? false : true;
    }
    public void UpdatePenetrateHealthBar(float current, float max) {
        penetratebar.MaxValue = max+0.01;
        penetratebar.Value = current;
        //Texture = subViewport.GetTexture();
        punctureIcon.Visible = (penetratebar.Value < 1) ? false : true;
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		bloodbar.Value = bloodbar.MaxValue;
        //energybar.Value = energybar.MaxValue;
        //penetratebar.Value = penetratebar.MaxValue;
        Texture = subViewport.GetTexture();
        if (punctureIcon != null) IconUpdate();
    }
    private void IconUpdate() {
        bloodIcon.Visible = (bloodbar.Value < 1) ? false : true;
        punctureIcon.Visible = (penetratebar.Value < 1) ? false : true;
        energyIcon.Visible =  (energybar.Value < 1) ? false : true;
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

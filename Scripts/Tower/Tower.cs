using Godot;
using System;

public partial class Tower : Node3D {

    [ExportCategory("Tower")]
    [Export] internal string name;
    [Export] internal Node3D rangePreview;

    [ExportCategory("Combat")] 
    [Export] internal int attack = 10;
    [Export] public float energy = 1f;
    [Export] public float puncture = 1f; 
    [Export] public float blood = 1f;
    [Export] internal float range = 10f;

    [ExportCategory("UI")]
    [Export] private Control info;
    [Export] internal RichTextLabel infotext;
    [Export] protected Button uDamageB;
    [Export] private Button uRangeB;
    [Export] private Button sellB;
    [Export] protected float sellReturn = 0.8f;




    protected int goldInvested;
    protected int level = 1;
    internal int upgrade_cost;
    internal float damageTimer = 0.0f;
    internal Enemy target = null;
    internal ulong targetID;


    internal void StartTower() {
        AddToGroup("Tower");
        UpdateTurretRange();
        upgrade_cost = Prices.GetCost(name);
        UpdateTowerInfo();
        info.Visible = false;
        blood += 0.1f * Upgrades.blood_upgrades;
        puncture += 0.1f * Upgrades.puncture_upgrades;
        energy += 0.1f * Upgrades.energy_upgrades;
    }
    internal virtual void UpdateTurretRange() { }

    internal void UpdateButtons() {
        if (Prices.gold < upgrade_cost) { if(uRangeB != null) uRangeB.Disabled = true; uDamageB.Disabled = true; }
        else { if (uRangeB != null)  uRangeB.Disabled = false; uDamageB.Disabled = false; }
    }
    private int GetSellPrize() {
        int selltax = (int)(goldInvested * sellReturn);
        int buyprice = Prices.GetCost(name, true) - Prices.GetCost(name);
        int sellprize = selltax + buyprice;
        return sellprize;
    }
    // UI Stuff    public void _on_control_gui_input(Variant @event) {
    private void _on_control_gui_input(Variant @event) {
        if (Input.IsActionJustPressed("cancel") || Input.IsActionJustPressed("m1click")) {
            info.Visible = false;
            if (rangePreview != null) rangePreview.Visible = false;
        }
    }
    private void _on_static_body_3d_input_event(Variant camera, Variant @event, Vector3 position, Vector3 normal, int shape_idx) {
        if (Input.IsActionJustPressed("m1click")) {
            UpdateTowerInfo();
            info.Visible = true;
            if (rangePreview != null) rangePreview.Visible = true;
        }

    }
    internal void UpdateTowerInfo() {
        infotext.Clear();
        infotext.AppendText("[b]" + name + "[/b]   " + "LV: " + level + "\n");

        var goldicon = new Texture2D();
        var bloodicon = new Texture2D();
        var punctureicon = new Texture2D();
        var electrcicon = new Texture2D();
        goldicon = GD.Load<Texture2D>("res://Assets/Icons/Goldsmall.png");
        bloodicon = GD.Load<Texture2D>("res://Assets/Icons/sBlood.png");
        punctureicon = GD.Load<Texture2D>("res://Assets/Icons/sPuncture.png");
        electrcicon = GD.Load<Texture2D>("res://Assets/Icons/sEnergyIcon.png");

        infotext.AddText("Damage:\n");
        infotext.AddImage(bloodicon);
        infotext.AddText(" " + (attack * blood).ToString("0.") + " ");
        infotext.AddImage(punctureicon);
        infotext.AddText(" " + (attack * puncture).ToString("0.") + " ");
        infotext.AddImage(electrcicon);
        infotext.AddText(" " + (attack * energy).ToString("0.") + "\n\n");


        uDamageB.Text = upgrade_cost + " +Damage";
        if (uRangeB != null) uRangeB.Text = upgrade_cost + " +Range";
        sellB.Text = GetSellPrize() + " Sell";

        ExtraInfo();
    }
    internal virtual void ExtraInfo() {}

    private protected virtual void _on_upgrade_damage_pressed() {
        if (upgrade_cost > Prices.gold) return;
        Prices.gold -= upgrade_cost;
        level++;
        goldInvested += upgrade_cost;
        upgrade_cost = (int)(Prices.GetCost(name) * Mathf.Pow(1.5, level - 1));
        attack += (int)(attack/2.5f);
        UpdateTowerInfo();
    }
    private protected virtual void _on_upgrade_range_pressed() {
        if (upgrade_cost > Prices.gold) return;
        Prices.gold -= upgrade_cost;
        level++;
        goldInvested += upgrade_cost;
        upgrade_cost = (int)(Prices.GetCost(name) * Mathf.Pow(1.5, level - 1));
        range *= 1.3f;
        UpdateTowerInfo();
        UpdateTurretRange();
    }
    private void _on_sell_pressed() {
        info.Visible = false;
        Prices.gold += GetSellPrize();
        Prices.ReduceTax(name);

        QueueFree();
    }

}

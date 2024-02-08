using Godot;
using System;

public partial class Upgrades : Control {

    private const int MAXDAMAGEPERKS = 5;
    private const int MAXINTERESTPERKS = 5;
    private const int MAXSLOW = 1;

    // Upgrades:
    public static int blood_upgrades = 0;
    public static int puncture_upgrades = 0;
	public static int energy_upgrades = 0;
	private static int interest_upgrades = 0;
    public static int ice_upgrades = 0;
	public static bool smart_turret = false;

    // Tower Specific upgrade
    [Signal] public delegate void IceTowerEventHandler();
    public static bool BurnStatus = false;
    public static bool laserTime = false;
    public static bool ExplosiveArrows = false;
    public static bool sellerperk = false;

    // show/hide perktree
    [Export] private Label displayPerkPoints;
	[Export] private CheckButton PerkButton;

	// in perktree
	[Export] private MarginContainer PerkTree;

    // Other
    public static int perkPoints = 0;
	private TurnHandler turns;


    //new version
    [Export] private Control greed;
    [Export] private Control weaponry;
    [Export] private tower_shop towerShop;


	// Enter/Exit    
    public override void _Ready() {
        turns = GetNode<TurnHandler>("/root/TurnHandler");
		turns.SurvivedTurn += AddUpgradePoint;
		PerkTree.Visible = false;
        greed.Visible = false;
        weaponry.Visible = true;

    }
    public override void _Process(double delta) {
        displayPerkPoints.Text = perkPoints.ToString();
    }
    private void _on_tree_exiting() { turns.SurvivedTurn -= AddUpgradePoint; }


    // add perk point
    private void AddUpgradePoint() {
		if (TurnHandler.currentTurn % 3 == 0) {
            perkPoints++;
        }
    }


    // Upgrades
    private void ApplyeDamageUpgrade(int damagetype) {
		//if (perkPoints < 1) return;

        // keep track of upgrades
        if (damagetype == 1) { blood_upgrades++; }
        if (damagetype == 2) { puncture_upgrades++; }
        if (damagetype == 3) { energy_upgrades++; }

        // applye upgrade to all
        var search = GetTree().GetNodesInGroup("AttackTower");
        foreach (var node in search) {
			Tower tower = node as Tower;
			if (damagetype == 1) { tower.blood += 0.1f; }
			if (damagetype == 2) { tower.puncture += 0.1f; }
			if (damagetype == 3) { tower.energy += 0.1f; }
        }
    }


	// Enable/disable buttons
	public static void UpdateButtons() {
        
    }

    // button pressed
 

    // display/hide
    private void _on_perks_button_toggled(bool pressed) {
        if (pressed) PerkTree.Visible = true;
        if (!pressed) PerkTree.Visible = false;
    }
    private void _on_close_button_pressed() {
		PerkTree.Visible = false;
        PerkButton.ButtonPressed = false;
    }
    private void _on_perk_tree_gui_input(Variant @event) {
        if (Input.IsActionJustPressed("cancel") || Input.IsActionJustPressed("m1click")) {
            PerkTree.Visible = false;
            PerkButton.ButtonPressed = false;
        }
    }




    // new version:
    private void _on_greed_pressed() {
        greed.Visible = true;
        weaponry.Visible = false;
    }
    private void _on_weaponry_pressed() {
        greed.Visible = false;
        weaponry.Visible = true;
    }

    // Greed perks:
    private void _on_interest_1_pressed() {
        castle.interest += 0.05f;
    }
    private void _on_interest_2_pressed() {
        castle.interest += 0.05f;
    }
    private void _on_interest_3_pressed() {
        castle.interest += 0.05f;
    }
    private void _on_interest_4_pressed() {
        castle.interest += 0.05f;
    }

    // Weaponry perks:
    private void _on_smart_tower_pressed() {
        smart_turret = true;

        var search = GetTree().GetNodesInGroup("AttackTower");
        foreach (DefenceTower tower in search) {
            //DefenceTower tower = node as DefenceTower;
            tower.EnableTargetPriorety();
        }
    }

    private void _on_health_perk_pressed() {
        ApplyeDamageUpgrade(1);
    }
    private void _on_puncture_perk_pressed() {
        ApplyeDamageUpgrade(2);
    }
    private void _on_energy_perk_pressed() {
        ApplyeDamageUpgrade(3);
    }
    private void _on_uninterrupted_pressed() {        
        laserTime = true;
    }
    private void _on_ice_tower_pressed() {
        towerShop.UnlockTower("IceTower");
        //EmitSignal(SignalName.IceTower);
    }
    private void _on_explosivee_pressed() {
        ExplosiveArrows = true;
    }
    private void _on_burner_pressed() {
        BurnStatus = true;
    }
    private void _on_plauge_tower_pressed() {
        towerShop.UnlockTower("PlaugeTower");
    }
    private void _on_gold_tower_pressed() {
        towerShop.UnlockTower("GoldTower");
    }
    private void _on_seller_pressed() {
        sellerperk = true;
    }


    // ------------- OLD STUFF ---------------
    /*
    private void _on_health_damage_pressed() {
        ApplyeDamageUpgrade(1);
        UpdateButtons();
    }
    private void _on_puncture_damage_pressed() {
        ApplyeDamageUpgrade(2);
        UpdateButtons();
    }
    private void _on_energy_damage_pressed() {
        ApplyeDamageUpgrade(3);
        UpdateButtons();
    }
    private void _on_smar_tower_pressed() {
        smart_turret = true;

        var search = GetTree().GetNodesInGroup("AttackTower");
        foreach (DefenceTower tower in search) {
            //DefenceTower tower = node as DefenceTower;
            tower.EnableTargetPriorety();
        }

        perkPoints -= 1;
        UpdateButtons();
    }
    private void _on_interest_pressed() {
        interest_upgrades++;
        castle.interest = interest_upgrades * 0.05f;
        perkPoints -= 2;
        UpdateButtons();
    }

    /*private void _on_ice_tower_pressed() {
        ice_upgrades++;
        if (ice_upgrades == 1) EmitSignal(SignalName.IceTower);
        perkPoints -= 2;
        UpdateButtons();
    }
        // Tower upgrades
    private void _on_burn_pressed() {
        BurnStatus = true;
        perkPoints -= 2;
        UpdateButtons();
    }*/
}

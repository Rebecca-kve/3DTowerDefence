using Godot;
using System;

public partial class tower_shop : Control {

    [Export] private Label playergold;
	[Export] private Label goldtower_cost;
	[Export] private Label lasertower_cost;
    [Export] private Label firetower_cost;
    [Export] private Label snipertower_cost;
    [Export] private Label icetower_cost;
    [Export] private Label plaugetower_cost;
    [Export] private Label turn;

    [Export] private Builder builder;

    [Export] private Button buyIceTower;
    [Export] private Button buyPlaugetower;
    [Export] private Button buyGoldTower;
	public override void _Ready() {
        buyIceTower.Visible = false;
        buyPlaugetower.Visible = false;
        buyGoldTower.Visible = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		goldtower_cost.Text = Prices.GoldTowertaxed.ToString();
		lasertower_cost.Text = Prices.LaserTowertaxed.ToString();
        firetower_cost.Text = Prices.FireTowertaxed.ToString();
        snipertower_cost.Text = Prices.SniperTowertaxed.ToString();
        icetower_cost.Text = Prices.IceTowertaxed.ToString();
        plaugetower_cost.Text = Prices.PlagueTowertaxed.ToString();
        playergold.Text = Prices.gold.ToString();
        turn.Text = "Turn: " + TurnHandler.currentTurn.ToString();
    }

	private void _on_gold_tower_pressed() {
        builder.UpdateTowerToBuild("GoldTower", Prices.GoldTowertaxed);
    }
	private void _on_laser_tower_pressed() {
        builder.UpdateTowerToBuild("LaserTower", Prices.LaserTowertaxed);
    }
	private void _on_fire_tower_pressed() {
        builder.UpdateTowerToBuild("FireTower", Prices.FireTowertaxed);
    }
    private void _on_sniper_tower_pressed() {
        builder.UpdateTowerToBuild("SniperTower", Prices.SniperTowertaxed);
    }
    private void _on_ice_tower_pressed() {
        builder.UpdateTowerToBuild("IceTower", Prices.IceTowertaxed);
    }
    private void _on_plague_tower_pressed() {
        builder.UpdateTowerToBuild("PlagueTower", Prices.PlagueTowertaxed);
    }

    private void _on_speed_toggled(bool toggle_mode) {
        if (toggle_mode) Engine.TimeScale = 2; 
        else Engine.TimeScale = 1;
    }

    private void _on_upgrades_ice_tower() {
        buyIceTower.Visible = true;
    }
    public void UnlockTower(string tower) {

        switch (tower) {
            case ("IceTower"):
                buyIceTower.Visible = true;
                break;
            case ("PlaugeTower"):
                buyPlaugetower.Visible = true;
                break;
            case ("GoldTower"):
                buyGoldTower.Visible = true;
                break;
        }
    }

}

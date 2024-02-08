using Godot;
using System;

public partial class game_ui : Control {

	[Signal]
	public delegate void nextTurnEventHandler();

	[Export] private Label turnCounter;
	[Export] private Button nextTurnButton;
	[Export] private PanelContainer shop;
	[Export] private Label gold;

	//private Prices price;
    private Builder builder;
	private TurnHandler turnHandler;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {

        //GameObject turns = GameObject.Find("/GameManager");
        turnHandler = GetNode<TurnHandler>("/root/TurnHandler");
		builder = GetNode<Builder>("../Builder");
		//price = GetNode<Prices>("/root/Prices");
        //TurnHandler productionEvent = turns.GetNode<TurnHandler>("TurnHandler");
        turnHandler.endTurn += EventTurnEnd;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		gold.Text = Prices.gold.ToString();
    }

	private void _on_next_turn_button_up() {
		turnHandler.StartNextTurn();
        turnCounter.Text = "Turn: " + TurnHandler.currentTurn;
        nextTurnButton.Disabled = true;

    }
    private void EventTurnEnd() { //EventTurnEnd(object sender, System.EventArgs e)
        nextTurnButton.Disabled = false;
	}
    private void _on_shop_button_up() {
		if (!shop.Visible)
			shop.Visible = true;
		else shop.Visible = false;
	}

	private void _on_towertest_button_up() {
		builder.UpdateTowerToBuild("LaserTower", Prices.LaserTowertaxed);

    }
	private void _on_buy_gold_tower_button_up() {
        builder.UpdateTowerToBuild("GoldTower", Prices.GoldTowertaxed);
    }
    private void _on_fire_tower_button_up() {
        builder.UpdateTowerToBuild("FireTower", Prices.FireTowertaxed);
    }
}

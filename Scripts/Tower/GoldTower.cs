using Godot;
using System;
using System.Diagnostics;
using System.Xml.Linq;

public partial class GoldTower : Tower {
   
    // consts
    private const int base_gold_income = 10;
	private const float upgrade_bonus = 1.5f;
    private const float upgrade_cost_increse = 1.5f;

    //private Prices price;

	public int gold_income = base_gold_income;
    private int sellbonus = 0;
    //private castle player;
    private TurnHandler turns;

    [Export] private AnimationPlayer goldEffect;
    [Export] private Label goldearned;
    [Export] private Label levellabel;

    public void Upgrade() {
        if (level >= 100) return;
		if (upgrade_cost > Prices.gold) return;
        Prices.gold -= upgrade_cost;
        goldInvested += upgrade_cost;
        LevelUp();
    }
	private void LevelUp() {
        if (level >= 100) return;
		level++;
		gold_income += (int)( base_gold_income * Mathf.Pow(upgrade_bonus, level - 1) );
        upgrade_cost = gold_income*level + (int)(Prices.GoldTower * Mathf.Pow(upgrade_cost_increse, level - 1) );
        UpdateTowerInfo();
    }


    public override void _Ready() {

        AddToGroup("Tower");
        AddToGroup("GoldTower");
        turns = GetNode<TurnHandler>("/root/TurnHandler");
        turns.endTurn += GetIncome;

        upgrade_cost = Prices.GetCost(name);

        UpdateTowerInfo();
    }
    private void _on_tree_exiting() {
        turns.endTurn -= GetIncome;
    }
    private void GetIncome() {
        Prices.gold += gold_income;
        goldearned.Text = gold_income.ToString();
        
        goldEffect.Play("GoldTowerIncome");
        
        if (Upgrades.sellerperk) {
            goldInvested += gold_income * 2;
            sellbonus += gold_income;
        }
        UpdateTowerInfo();
    }

    private protected override void _on_upgrade_damage_pressed() {
        Upgrade();
    }

    public override void _Process(double delta) {
        UpdateButtons();
    }
    internal override void ExtraInfo() {
        var goldicon = new Texture2D();
        goldicon = GD.Load<Texture2D>("res://Assets/Icons/Goldsmall.png");
        
        infotext.AddText("Income: " + gold_income + " "); infotext.AddImage(goldicon);
        if (Upgrades.sellerperk) infotext.AddText("\nsellbonus Bonus :" + sellbonus);
        uDamageB.Text = upgrade_cost + " Upgrade";
        levellabel.Text = level.ToString();
    }
    //public void UpdateSellReturn(float invesreturn) { sellReturn = invesreturn; }

}

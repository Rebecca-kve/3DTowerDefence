using Godot;
using System.Drawing;

public partial class castle : Node3D {

    //public int gold;
    private int level = 0; // turnssurvived
    [Signal] public delegate void CastleInfectedEventHandler();

    // Base stats
    private const int startHealth = 5;
    private const float baseinterests = 0.05f;
    private const int basegoldIncome = 20;
    private const int baseRepairCost = 10;

    // current stats
    private int maxHealth = startHealth;
    private int currentHealth = startHealth;
    public static float interest = 0;
    private int goldIncome = basegoldIncome;
    private int repairCost = baseRepairCost;
    //private int levelUpCost;

    [ExportCategory("HPbar")]
    [Export] private HealthBar3D healtbar;

    [ExportCategory("BigUI")]
    [Export] private RichTextLabel goldinfo;
    [Export] private TextureProgressBar bigHealthBar;
    [Export] private Button bigRepair;
    [Export] private Label healthtext;

    private TurnHandler turns;

    //private castle() { interests = WhatIsMyInterest(); }
    public override void _Ready() {
        turns = GetNode<TurnHandler>("/root/TurnHandler");
        turns.SurvivedTurn += CastleSurvivedTurn;
        UpdateRepairButton();
    }
    public override void _Process(double delta) {
        UpdateGoldInfo();
        if (CanRepairCastle()) bigRepair.Disabled = false;
        else bigRepair.Disabled = true;
    }
    private void _on_tree_exiting() {
        turns.SurvivedTurn -= CastleSurvivedTurn;
    }


    private void LevelUp() {
        if (level >= 100 || currentHealth <= 0) return; //max level
                                  //gold -= levelUpCost;
        level++;
        maxHealth++; currentHealth++;
        //interests = WhatIsMyInterest();
        goldIncome = basegoldIncome + (basegoldIncome / 4) * level;
        repairCost = (int)(baseRepairCost * Mathf.Pow(1.05, level));
        UpdateRepairButton();
    }

    private float WhatIsMyInterest() {
        return (baseinterests + interest) * currentHealth / maxHealth;
    }
    private void UpdateRepairButton() {

        float healthinterestvalue = baseinterests * 1 / maxHealth;
        if (currentHealth != maxHealth) {
            bigRepair.Text = "Repair Castle " + repairCost + " gold\n+" + (healthinterestvalue*100).ToString("0.00") + "% interest";
        }
        else bigRepair.Text = "Repair Castle " + repairCost + " gold";
    }
    private void repairCastel() {
        if (!CanRepairCastle()) return;

        Prices.gold -= repairCost;
        currentHealth++;
        UpdateRepairButton();
    }
    private bool CanRepairCastle() {
        if (Prices.gold >= repairCost && currentHealth < maxHealth) return true;
        else return false;
    }


    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            EmitSignal(SignalName.CastleInfected);
        }
        UpdateRepairButton();
    }
    private int TowerIncome() {
        int goldincome = 0;
        var towers = GetTree().GetNodesInGroup("GoldTower");
        goldincome += goldIncome;
        foreach (GoldTower gt in towers) {
            goldincome += gt.gold_income;
        }

        return goldincome;
    }
    private void UpdateGoldInfo() {
        goldinfo.Clear();
        UpdateHealthBar();
        //interests = WhatIsMyInterest();
        var intrests_income = (Prices.gold + TowerIncome()) * WhatIsMyInterest();
        var tower_income = TowerIncome();
        

        var goldicon = new Texture2D();
        goldicon = GD.Load<Texture2D>("res://Assets/Icons/Goldsmall.png");
        goldinfo.AddImage(goldicon);
        goldinfo.AppendText("  [b]Gold: [/b]" + Prices.gold.ToString() + "\n");
        goldinfo.AppendText("[b]Total Income:  [/b]" + (tower_income + intrests_income).ToString("0.") + "\n");
        goldinfo.AppendText("[b]Tower Income:  [/b]" + tower_income + "\n");
        goldinfo.AppendText("[b]Interest income:  [/b]");
        goldinfo.AddText(intrests_income.ToString("0.") + "\n");
        goldinfo.AppendText("[b]Interest:  [/b]" + (WhatIsMyInterest() * 100).ToString("0.00") + "%" + "\n");
        goldinfo.AddText("FPS: " + Engine.GetFramesPerSecond().ToString("0.0"));


        //bigRepair.Text = "Repair Castle: " + repairCost.ToString() + " gold\n";
    }
    public void UpdateHealthBar() {
        healthtext.Text = "Castle" + "  |  " + "LV: " + level + "   |   " + currentHealth + " / " + maxHealth;
        healtbar.text.Text = currentHealth + " / " + maxHealth;
        healtbar.UpdateBloodHealthBar(currentHealth,maxHealth);
        bigHealthBar.MaxValue = maxHealth;
        bigHealthBar.Value = currentHealth;
        //Texture = subViewport.GetTexture();
    }

    // Events
    private void _on_repair_castle_pressed() {
        repairCastel();
    }
    private void CastleSurvivedTurn() {
        Prices.gold += goldIncome;
        Prices.gold += (int)(Prices.gold * WhatIsMyInterest());
        LevelUp();
    }
}
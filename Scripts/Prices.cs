using Godot;
using System;
public partial class Prices : Node {

    public const int LaserTower = 15;
    public const int GoldTower = 25;
    public const int FireTower = 30;
    public const int SniperTower = 50;
    public const int IceTower = 100;
    public const int PlagueTower = 150;

    //private const int goldstart = 200;
    public static int gold; // How much gold the player have

	public static int LaserTowertaxed;
	public static int GoldTowertaxed;
	public static int FireTowertaxed;
    public static int SniperTowertaxed;
    public static int IceTowertaxed;
    public static int PlagueTowertaxed;

    // Towers
    private const string Laser = "LaserTower";
    private const string Gold = "GoldTower";
    private const string Fire = "FireTower";
    private const string Sniper = "SniperTower";
    private const string Ice = "IceTower";
    private const string Plauge = "PlagueTower";

    private Prices() {
        LaserTowertaxed = LaserTower;
        GoldTowertaxed = GoldTower;
        FireTowertaxed = FireTower;
        SniperTowertaxed = SniperTower;
        IceTowertaxed = IceTower;
        PlagueTowertaxed = PlagueTower;
        //gold = goldstart;
    }
    public static void Reset() {
        LaserTowertaxed = LaserTower;
        GoldTowertaxed = GoldTower;
        FireTowertaxed = FireTower;
        SniperTowertaxed = SniperTower;
        IceTowertaxed = IceTower;
        PlagueTowertaxed = PlagueTower;
        gold = 0;
    }
    public static void IncreaseTax(string tower) {
		if (tower == Laser) LaserTowertaxed += LaserTower;
        if (tower == Gold) GoldTowertaxed += GoldTower;
        if (tower == Fire) FireTowertaxed += FireTower;
        if (tower == Sniper) SniperTowertaxed += SniperTower;
        if (tower == Ice) IceTowertaxed += IceTower;
        if (tower == Plauge) PlagueTowertaxed += PlagueTower;
    }

    public static void ReduceTax(string tower) {
        if (tower == Laser) LaserTowertaxed -= LaserTower;
        if (tower == Gold) GoldTowertaxed -= GoldTower;
        if (tower == Fire) FireTowertaxed -= FireTower;
        if (tower == Sniper) SniperTowertaxed -= SniperTower;
        if (tower == Ice) IceTowertaxed -= IceTower;
        if (tower == Plauge) PlagueTowertaxed -= PlagueTower;
    }

    public static int GetCost(string tower, bool tax = false) {
        if (tax) {
            if (tower == Laser) return LaserTowertaxed;
            if (tower == Gold) return GoldTowertaxed;
            if (tower == Fire) return FireTowertaxed;
            if (tower == Sniper) return SniperTowertaxed;
            if (tower == Ice) return IceTowertaxed;
            if (tower == Plauge) return PlagueTowertaxed;
        }
        if (tower == Laser) return LaserTower;
        if (tower == Gold) return GoldTower;
        if (tower == Fire) return FireTower;
        if (tower == Sniper) return SniperTower;
        if (tower == Ice) return IceTower;
        if (tower == Plauge) return PlagueTower;
        else return 0;
    }
}

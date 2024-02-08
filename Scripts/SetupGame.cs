using Godot;
using System;
using System.Threading;

public partial class SetupGame : Node3D {
	// Called when the node enters the scene tree for the first time.

	[Export] private int startgold = 200;
	[Export] private int startPerkpoints = 0;
	public override void _Ready() {
		TurnHandler.Reset();
		Prices.Reset(); Prices.gold = startgold;
		var gameover = GetNode<Control>("%Game Over");
		gameover.Visible = false;
		Upgrades.perkPoints = 0;
	}
}

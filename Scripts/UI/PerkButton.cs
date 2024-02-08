using Godot;
using System;

public partial class PerkButton : Button {

	[Export] private Line2D line;

	public bool unlocked = false;

	public override void _Ready() {

		if (GetParent() is PerkButton) {
            var previousperk = GetParent<PerkButton>();
            line.AddPoint(Size / 2);
			line.AddPoint(previousperk.GlobalPosition + (previousperk.Size / 2) - GlobalPosition);
        }

    }

	private void _on_pressed() {
		unlocked = true;
        Upgrades.perkPoints--;
    }


	private void UnlockButton() {
        if (unlocked) {
            Disabled = true;
            ThemeTypeVariation = "Unlock";
            return;
        }
        else if (Upgrades.perkPoints == 0) {
            Disabled = true; return;
        }
        else Disabled = false;

        if (GetParent() is PerkButton) {
            if (GetParent<PerkButton>().unlocked) Disabled = false;
            else Disabled = true;
        }
    }

    public override void _Process(double delta) {
        UnlockButton();
    }
}

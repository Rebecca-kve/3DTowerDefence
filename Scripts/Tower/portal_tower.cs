using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Godot.OpenXRHand;

public partial class portal_tower : Node3D {

    
    private int counter = 0;
    private int level = 1;
    private TurnHandler turns;
    [Export] Timer timer;
    

    public override void _Ready() {
        turns = GetNode<TurnHandler>("/root/TurnHandler");
        turns.startTurn += ProductionEvent_SpawnEnemyes;
        //turns.endTurn += SetUpNextTurn;

    }
    private void _on_tree_exiting() {
        turns.startTurn -= ProductionEvent_SpawnEnemyes;
    }

    public override void _Process(double delta) {
    }

    private void ProductionEvent_SpawnEnemyes() { // ProductionEvent_SpawnEnemyes(object sender, System.EventArgs e)
        SetStage(TurnHandler.currentTurn);
    }
    private async void SpawnWaves(List<Wave> wave) {
        if (wave.Count < 1) return;
        foreach (Wave w in wave) {
            //await ToSignal(GetTree().CreateTimer(w.Time), "timeout");            
            //SpawnWave(w);
            var scene = GD.Load<PackedScene>("res://Scenes/Enemy/" + w.Enemy + ".tscn");

            for (int i = 0; i < w.Num; i++) {
                var inst = scene.Instantiate<Enemy>();
                inst.Initialize(w.Level);
                timer.Start(w.Time);
                await ToSignal(timer, "timeout");
                AddChild(inst, true);
            }
        } 
    }

    private async void SpawnWave(Wave wave) {
        var scene = GD.Load<PackedScene>("res://Scenes/Enemy/" + wave.Enemy + ".tscn");

        for (int i = 0; i < wave.Num; i++) {
            var inst = scene.Instantiate<Enemy>();
            inst.Initialize(wave.Level);
            timer.Start(wave.Time);
            await ToSignal(timer,"timeout");
            //await ToSignal(GetTree().CreateTimer(wave.Time), SceneTreeTimer.SignalName.Timeout);
            AddChild(inst, true);
        }
    }
    private void SetStage(int stage) {

        var diff = stage - wavearray.Count<Wave>();

        if (diff > 0) {
            AddMoreStagesToList(diff);
        }
        else {
            counter = wavearray[stage - 1].Num;
            TurnHandler.EnemyCountThisTurn += counter;
            SpawnWave(wavearray[stage - 1]);
        }
    }

    private void AddMoreStagesToList(int diff) {
        
        List<Wave> waves = new List<Wave>();
        Random random = new Random();
        level++;
        Wave[] hard = HardArray(level);
        int rannum; int alimint = hard.Count<Wave>();

        int min = 5;
        int max = 50;
        if (diff < min) diff = min;
        if (diff > max) diff = max;

        counter = diff;
        TurnHandler.EnemyCountThisTurn += counter;

        for (int i = 0; i < diff; i++) {
            rannum = random.Next(0, alimint);
            waves.Add(hard[rannum]);
        }
        SpawnWaves(waves);
    }
    private class Wave {
        public int Num;
        public int Level;
        public float Time;
        public string Enemy;

        public Wave(int num, string enemy, float time, int level) {
            Num = num;
            Level = level;
            Time = time;
            Enemy = enemy;
        }

    }
    private Wave[] wavearray = {
            new Wave(1, "Pawn", 0.5f, 1),
            new Wave(2, "Pawn", 1f, 1),
            new Wave(4, "Pawn", 0.4f, 2),
            new Wave(6, "Pawn", 0.3f, 2),
            new Wave(4, "Knight", 0.6f, 1),
            new Wave(5, "Knight", 0.4f, 2),
            new Wave(6, "Knight", 0.3f, 2),
            new Wave(10, "Pawn", 0.5f, 3),
            new Wave(4, "Rook", 0.5f, 1),
            new Wave(6, "Rook", 0.5f, 2),
            new Wave(4, "Boss", 0.5f, 1),
            new Wave(8, "Splitter10", 1f, 1),
            new Wave(8, "Splitter10", 1f, 2),
            new Wave(8, "Splitter10", 1f, 3),
    };
    private Wave[] HardArray(int lv) {
        Wave[] hardarray = {
            new Wave(1, "Knight", 0.4f, lv+2),
            new Wave(1, "Rook", 0.5f, lv+1),
            new Wave(1, "Boss", 0.6f, lv),
            new Wave(1, "Splitter10", 1f, lv),
        };
        return hardarray;
    }

}



/* Scrapped
 *     //private int spawns = 1;
    //private string[] enemytypes = { "Pawn", "Knight", "Rook", "Boss" }; // change to enum?
    //private int[] numarray = { 4, 3, 2, 1 };
    //private int resetcount = 0;
    //private bool secondwaveused = false;
    //private List<Wave> wavedata;

private IDictionary<int, string> GetWaveData() {
    counter++;
    if (counter == 1) wave.Add(1, "pawn");
    if (counter == 2) wave.Add(1, "knight");
    wave.Remove(0);
    int t = wave.ElementAt(0).Key;
    string s = wave.ElementAt(0).Value;


    return wave;
}
IDictionary<int, string> wave = new Dictionary<int, string>();

 private void SetUpNextTurn() {
        secondwaveused = false;
        if (counter == numarray.Length - 1) {
            counter = 0; resetcount++;

            if (level < 100) level++;

            for (int i = 0; i < numarray.Length; i++) {
                numarray[i]++;
            }

        }
        else counter++;
    }

    private async void SpawnWaveold() {

        var scene = GD.Load<PackedScene>("res://Scenes/Enemy/" + enemytypes[counter] + ".tscn");

        for (int i = 0; i < numarray[counter]; i++) {
            var inst = scene.Instantiate<Enemy>();
            inst.Initialize(level);
            await ToSignal(GetTree().CreateTimer(0.5), "timeout");
            AddChild(inst, true);
        }
    }

*/
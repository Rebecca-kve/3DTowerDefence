using Godot;
using System.Collections.Generic;

public partial class BuildGridMap : GridMap {
    private int posx = 0;
    private int posz = 0;
    private Vector3 lastposition = new Vector3I(6, 1, 5);

    private string direction = "down"; // 0 1 2 3
    private bool firstbuild = true;
    public bool mapfinished = false;
    public Vector3 spawnposition;

    [Export] Node3D nextturnbutton;
    [Export] Node3D test;
    private Vector3 nextposition = new Vector3(6, 0.5f, 6);

    private List<int[,]> orient = new();




    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        spawnposition = GetSpawnPosition();
        nextturnbutton.Position = nextposition;
        /* test
        BuildBlock(blockI);
        firstbuild = false;
        while (!mapfinished) {
            SelectAndBuild();
        }*/
    }
    public override void _Process(double delta) {


    }

    private void BuildBlock(int[,] block, int[,] orientation) {
        for (int y = 0; y < 6; y++) {
            for (int x = 0; x < 6; x++) {
                SetCellItem(new Vector3I(x + posx, 0, y + posz), block[y, x], orientation[y, x]);
            }
        }
        updatePosition(block);
        spawnposition = GetSpawnPosition();
        test.Position = GetSpawnPosition();
        nextturnbutton.Position = nextposition;
    }
    private List<int[,]> AddOkBlocks() {
        var acceptedblocks = new List<int[,]>();
        orient.Clear();
        //accepteblocks = allblocks;

        var down = new Vector3I(posx, 0, posz + 6);
        var right = new Vector3I(posx + 6, 0, posz);
        var up = new Vector3I(posx, 0, posz - 6);
        var left = new Vector3I(posx - 6, 0, posz);


        // add connections to last build
        if (direction == "up") {
            acceptedblocks.Add(blockL); orient.Add(blockLor);
            acceptedblocks.Add(blockJ); orient.Add(blockJor);
            acceptedblocks.Add(blockI); orient.Add(blockIor);
            acceptedblocks.Add(blockI); orient.Add(blockIor);
        }
        if (direction == "left") {
            acceptedblocks.Add(block7); orient.Add(block7or);
            acceptedblocks.Add(blockJ); orient.Add(blockJor);
            acceptedblocks.Add(block_); orient.Add(block_or);
            acceptedblocks.Add(block_); orient.Add(block_or);
        }
        if (direction == "down") {
            acceptedblocks.Add(blockI); orient.Add(blockIor);
            acceptedblocks.Add(blockI); orient.Add(blockIor);
            acceptedblocks.Add(blockC); orient.Add(blockCor);
            acceptedblocks.Add(block7); orient.Add(block7or);
        }
        if (direction == "right") {
            acceptedblocks.Add(blockL); orient.Add(blockLor);
            acceptedblocks.Add(blockC); orient.Add(blockCor);
            acceptedblocks.Add(block_); orient.Add(block_or);
            acceptedblocks.Add(block_); orient.Add(block_or);
        }

        // remove blocked paths
        if (GetCellItem(up) != -1 && direction != "up") {
            acceptedblocks.Remove(blockL); orient.Remove(blockLor);
            acceptedblocks.Remove(blockJ); orient.Remove(blockJor);
            acceptedblocks.Remove(blockI); orient.Remove(blockIor);
            acceptedblocks.Remove(blockI); orient.Remove(blockIor);
        }
        if (GetCellItem(left) != -1 && direction != "left") {
            acceptedblocks.Remove(block7); orient.Remove(block7or);
            acceptedblocks.Remove(blockJ); orient.Remove(block_or);
            acceptedblocks.Remove(block_); orient.Remove(blockJor);
            acceptedblocks.Remove(block_); orient.Remove(block_or);
        }
        if (GetCellItem(down) != -1 && direction != "down") {
            acceptedblocks.Remove(blockI); orient.Remove(blockIor);
            acceptedblocks.Remove(blockI); orient.Remove(blockIor);
            acceptedblocks.Remove(blockC); orient.Remove(blockCor);
            acceptedblocks.Remove(block7); orient.Remove(block7or);
        }
        if (GetCellItem(right) != -1 && direction != "right") {
            acceptedblocks.Remove(blockL); orient.Remove(blockLor);
            acceptedblocks.Remove(blockC); orient.Remove(blockCor);
            acceptedblocks.Remove(block_); orient.Remove(block_or);
            acceptedblocks.Remove(block_); orient.Remove(block_or);
        }
        // Add last block when all paths is blocked
        if (acceptedblocks.Count == 0) {
            acceptedblocks.Add(blocklast); orient.Add(blocklastor);
            mapfinished = true;
            nextturnbutton.Visible = false;
        }


        return acceptedblocks;
    }

    private void SelectAndBuild() {
        List<int[,]> acceptedblocks = AddOkBlocks();

        var random = new RandomNumberGenerator();
        random.Randomize();
        //pick a block by random and build
        var ran = random.RandiRange(0, acceptedblocks.Count - 1);
        BuildBlock(acceptedblocks[ran], orient[ran]);
    }
    private void _on_next_turn_pressed() {
        if (mapfinished) return;
        /*
        if (firstbuild) {
            lastposition = new Vector3I(posx, 0, posz);
            BuildBlock(blockI, blockIor);
            firstbuild = false;
        } */
        else SelectAndBuild();
    }

    private void updatePosition(int[,] lastblockused) {

        string dirnew = "Error"; //dummy

        if (direction == "down") {
            if (lastblockused == blockC) dirnew = "left";
            if (lastblockused == block7) dirnew = "right";
            if (lastblockused == blockI) dirnew = "down";
        }
        if (direction == "right") {
            if (lastblockused == blockL) dirnew = "down";
            if (lastblockused == blockC) dirnew = "up";
            if (lastblockused == block_) dirnew = "right";
        }
        if (direction == "up") {
            if (lastblockused == blockL) dirnew = "left";
            if (lastblockused == blockJ) dirnew = "right";
            if (lastblockused == blockI) dirnew = "up";
        }
        if (direction == "left") {
            if (lastblockused == block7) dirnew = "up";
            if (lastblockused == blockJ) dirnew = "down";
            if (lastblockused == block_) dirnew = "left";
        }
        direction = dirnew;
        UpdateNextPosition(dirnew);
    }

    private void UpdateNextPosition(string dir) {
        lastposition = ToGlobal(MapToLocal(new Vector3I(posx, 0, posz))) + new Vector3I(5, 1, 5);
        if (dir == "down") posz -= 6;
        if (dir == "right") posx -= 6;
        if (dir == "up") posz += 6;
        if (dir == "left") posx += 6;
        nextposition = ToGlobal(MapToLocal(new Vector3I(posx, 0, posz))) + new Vector3I(5, 0, 5);
    }

    private Vector3 GetSpawnPosition() {
        Vector3 difference;
        difference = nextposition-lastposition;
        return spawnposition = lastposition + difference/3;
    }


    // Might be usefull
    private int[,] RotateGridClockW(int[,] grid) {
        int[,] result = new int[grid.GetLength(0), grid.GetLength(1)];

        for (int row = 0; row < grid.GetLength(0); row++) {
            for (int col = 0; col < grid.GetLength(1); col++) {
                result[col, grid.GetLength(0) - row - 1] = grid[row, col];
            }
        }

        return result;
    }
    private int[,] RotateGridCClockW(int[,] grid) {
        int[,] result = new int[grid.GetLength(0), grid.GetLength(1)];

        for (int row = 0; row < grid.GetLength(0); row++) {
            for (int col = 0; col < grid.GetLength(1); col++) {
                result[grid.GetLength(1) - col - 1, row] = grid[row, col];
            }
        }
        return result;
    }



    // block names is block + characther that look like the shape
    private int[,] blockL = {
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 5, 3, 3 },
        { 1, 1, 4, 3, 3, 3 },
        { 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1 },
    };
    private int[,] blockC = {
        { 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1 },
        { 1, 1, 4, 3, 3, 3 },
        { 1, 1, 3, 5, 3, 3 },
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
    };

    private int[,] block7 = {
        { 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1 },
        { 3, 3, 3, 4, 1, 1 },
        { 3, 3, 5, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
    };
    private int[,] blockJ = {
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
        { 3, 3, 5, 3, 1, 1 },
        { 3, 3, 3, 4, 1, 1 },
        { 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1 },
    };
    private int[,] blockI = {
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
        { 1, 1, 3, 3, 1, 1 },
    };
    private int[,] block_ = {
        { 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1 },
        { 3, 3, 3, 3, 3, 3 },
        { 3, 3, 3, 3, 3, 3 },
        { 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1 },
    };
    private int[,] blocklast = {
        { 2, 2, 2, 2, 2, 2 },
        { 2, 2, 2, 2, 2, 2 },
        { 2, 2, 2, 2, 2, 2 },
        { 2, 2, 2, 2, 2, 2 },
        { 2, 2, 2, 2, 2, 2 },
        { 2, 2, 2, 2, 2, 2 },
    };

    // rotation of the grid pieces, this cant be rotated
    private int[,] blockIor = { //test ok
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
    };
    private int[,] block_or = { //test ok
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 22, 22, 22, 22, 22, 22 },
        { 16, 16, 16, 16, 16, 16 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
    };
    private int[,] blockLor = { //test ok
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 16, 22, 22 },
        { 0, 0, 16, 16, 16, 16 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
    };
    private int[,] blockCor = { //test ok
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 22, 22, 22 },
        { 0, 0, 0, 0, 16, 16 },
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
    };
    private int[,] block7or = { //test ok
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 22, 22, 22, 22, 0, 0 },
        { 16, 16, 22, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
    };
    private int[,] blockJor = { //test ok
        { 0, 0, 0, 10, 0, 0 },
        { 0, 0, 0, 10, 0, 0 },
        { 22, 22, 10, 10, 0, 0 },
        { 16, 16, 16, 10, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
    };
    private int[,] blocklastor = {
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0 },
    };
}

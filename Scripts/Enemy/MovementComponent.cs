using Godot;
using System;

public partial class MovementComponent : Node3D
{
    //[Export] private Node3D HQ;
    [Export] private NavigationAgent3D _navAgent;
    [Export] private Enemy enemy;

    //private float _movementSpeed = 15.0f;
    private Vector3 _TargetPosition;


    public Vector3 MovementTarget {
        get { return _navAgent.TargetPosition; }
        set { _navAgent.TargetPosition = value; }
    }

    public override void _Ready() {
        base._Ready();

        // root not in same scene
        Node3D HQ = GetNode<Node3D>("../../../Castle");
        _TargetPosition = HQ.Position;

        // Make sure to not await during _Ready.
        Callable.From(ActorSetup).CallDeferred();
        _navAgent.NavigationFinished += () => DestinationReached();
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);

        if (_navAgent.IsNavigationFinished()) {
            return;
        }

        Vector3 currentAgentPosition = GlobalTransform.Origin;
        Vector3 nextPathPosition = _navAgent.GetNextPathPosition();

        Vector3 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= enemy.speed_now;

        enemy.LookAt(GlobalPosition - newVelocity, Vector3.Up);

        enemy.Velocity = newVelocity;

        enemy.MoveAndSlide();

    }

    private async void ActorSetup() {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        MovementTarget = _TargetPosition;
    }
    
    private void DestinationReached() {
        //castle playercastle = (castle)GetTree().Root.GetNode("Level_01").GetNode<Node3D>("Castle");
        castle playercastle = (castle)GetNode<Node3D>("../../../Castle");
        playercastle.TakeDamage(enemy.attack);
        TurnHandler.EnemyRemovedThisTurn++;
        enemy.QueueFree();
    }
    
}

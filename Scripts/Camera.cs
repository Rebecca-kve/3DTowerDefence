using Godot;
using System;
using System.Reflection.Metadata;

public partial class Camera : Node3D {
    private Vector3 scale = new Vector3(1, 1, 1);
    private int newzoom = 75;
    private Vector3 cameradistance = new Vector3(0, 0, 15);

    private Node3D angle;
    private Camera3D camera;

    public override void _Ready() {
        angle = GetNode<Node3D>("Angle");
        camera = GetNode<Camera3D>("Angle/Camera3D");

        Position = new Vector3I(6, 1, 19);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        CameraMovement((float)delta);
        CameraRotation((float)delta);
        //CameraFov();
        //CameraZoom();
        MoveCameraZoom();
    }

    private void CameraMovement(float delta) {

        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.IsActionPressed("move_forward")) inputDir.Z = -1f;
        if (Input.IsActionPressed("move_back")) inputDir.Z = +1f;
        if (Input.IsActionPressed("move_left")) inputDir.X = -1f;
        if (Input.IsActionPressed("move_right")) inputDir.X = +1f;
        
        const float movementspeed = 40.0f;

    // handle angles
    Vector3 moveDir = Transform.Basis.Z * inputDir.Z + Transform.Basis.X * inputDir.X;
        Position += moveDir * movementspeed * delta; 
    }

    private void CameraRotation(float delta) {

        // rotate around 
        float rotateDir = 0f;
        if (Input.IsActionPressed("rotate_left")) rotateDir -= 1f;
        if (Input.IsActionPressed("rotate_right")) rotateDir += 1f;


        const float rotationspeed = 1.0f;
        RotateY(rotateDir * rotationspeed * delta);
        /*
        private float rotation = 0;

        if (Input.IsActionPressed("rotate_left") && rotation >= -10) {
            //RotateY(-1 * rotationspeed * (float)delta);
            rotation -= 1 * rotationspeed * (float)delta;
        }
        if (Input.IsActionPressed("rotate_right") && rotation <= 10) {
            //RotateY(1 * rotationspeed * (float)delta);
            rotation += 1 * rotationspeed * (float)delta;
        }
        RotateY(rotation);
        */
    }

    private void CameraZoom() {
        Vector3 min = new Vector3(0.1f, 0.1f, 0.1f);
        Vector3 max = new Vector3(2, 2, 2);
        Vector3 zoomspeed = new Vector3(0.1f, 0.1f, 0.1f);

        if (Input.IsActionJustPressed("zoom_in")) {
            scale -= zoomspeed;
        }
        if (Input.IsActionJustPressed("zoom_out")) {
            scale += zoomspeed;
        }
        angle.Scale = scale.Clamp(min, max);
    }

    private void MoveCameraZoom() {
        Vector3 min = new Vector3(0, 0, 5);
        Vector3 max = new Vector3(0, 0, 100);
        Vector3 zoomspeed = new Vector3(0, 0, 2);

        if (cameradistance.Z >= min.Z && Input.IsActionJustPressed("zoom_in")) {
            cameradistance -= zoomspeed;
        }
        if (cameradistance.Z <= max.Z && Input.IsActionJustPressed("zoom_out")) {
            cameradistance += zoomspeed;
        }
        camera.Position = cameradistance;

    }

    private void CameraFov() {
        int min = 20;
        int max = 120;
        int zoomspeed = 5;

        if (newzoom >= min && Input.IsActionJustPressed("zoom_in")) {
            newzoom -= zoomspeed;
        }
        if (newzoom <= max && Input.IsActionJustPressed("zoom_out")) {
            newzoom += zoomspeed;
        }
        camera.Fov = newzoom;
    }

    // New Code To use mouse drag
    private bool DragCamera = false;
    private Vector2 mouseLockPosition;

    public override void _UnhandledInput(InputEvent @event) {
        //if (InputMap.ActionHasEvent("Grab_Camera", @event)) { mouseLockPosition = GetViewport().GetMousePosition(); }


        // Check if the "Drag_Camera" action is pressed
        if (@event.IsActionPressed("Grab_Camera")) {
            mouseLockPosition = GetViewport().GetMousePosition();
            DragCamera = true;

            Input.MouseMode = Input.MouseModeEnum.Captured; // Hide the mouse
        }

        // Check if the "Drag_Camera" action is released
        if (@event.IsActionReleased("Grab_Camera")) {
            DragCamera = false;

            Input.MouseMode = Input.MouseModeEnum.Visible; // Show the mouse
            GetViewport().WarpMouse(mouseLockPosition); // Restore mouse position
        }

        // Handle mouse movement when DragCamera is true
        if (DragCamera && @event is InputEventMouseMotion mouseMotion) {
            Vector3 inputDir = new Vector3(0, 0, 0);
            const float movementspeed = 4.0f;

            inputDir.X = -mouseMotion.Relative.X; // Inverting the X direction
            inputDir.Z = -mouseMotion.Relative.Y; // Inverting the Y direction


            if (DragCamera) {
                Vector3 moveDir = Transform.Basis.Z * inputDir.Z + Transform.Basis.X * inputDir.X;
                Position += moveDir * movementspeed * (float)GetProcessDeltaTime();
            }

        }

    }
}

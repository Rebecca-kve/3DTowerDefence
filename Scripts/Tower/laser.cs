using Godot;
using System;

public partial class laser : RayCast3D
{

    [Export] private CsgCylinder3D beamMesh;
    [Export] private GpuParticles3D endPoint;
    [Export] private GpuParticles3D beamParticles;

    public void DisplayLaser() {
        Vector3 castPoint = Position;
        ForceRaycastUpdate();

        beamMesh.Visible = true;
        endPoint.Visible = true;
        beamParticles.Visible = true;

        castPoint = ToLocal(GetCollisionPoint());
        beamMesh.Height = castPoint.Z;
        beamMesh.Position = castPoint / 2;
        endPoint.Position = castPoint;
        beamParticles.Position = castPoint;
    }
    public void HideLaser() {
        beamMesh.Visible = false;
        endPoint.Visible = false;
        beamParticles.Visible = false;
    }
}

using Godot;
using System;

public partial class BusTeaser : RigidBody3D
{
    public void Boop()
    {
        ApplyForce(new Vector3(40, 30, 0));
    }
}

using Godot;
using System;

public partial class VoidCharacter : CharacterBody3D
{
    const float speed = 5f;
    const float gravity = 30f;
    const float jumpForce = 10f;
    const float acceleration = .5f;
    const float sensitivity = 0.01f;

    public Node3D Head;
    public Camera3D Cam;

}

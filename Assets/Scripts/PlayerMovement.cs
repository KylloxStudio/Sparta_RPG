using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;

    public Vector3 MoveVector { get; set; }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (!_controller.isGrounded)
        {
            MoveVector = MoveVector.Change(y: MoveVector.y + (Physics.gravity.y * Time.deltaTime));
        }
        
        _controller.Move(MoveVector * Time.deltaTime);
    }
}

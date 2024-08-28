using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerMovementFusion : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 movement;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");


            movement = new Vector2(moveX, moveY);
            transform.position += (Vector3)(movement * moveSpeed * Runner.DeltaTime);
        }
    }
}


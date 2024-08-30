using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerMovementFusion : NetworkBehaviour
{
    private Rigidbody2D _rb; // Refer�ncia ao Rigidbody2D para movimento 2D

    public float moveSpeed = 5f; // Velocidade de movimento

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // Normaliza a dire��o para evitar acelera��o excessiva
            data.direction.Normalize();

            // Move o jogador aplicando for�a ao Rigidbody2D
            _rb.velocity = data.direction * moveSpeed;
        }
    }
}
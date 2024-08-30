using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerMovementFusion : NetworkBehaviour
{
    private Rigidbody2D _rb; // Referência ao Rigidbody2D para movimento 2D

    public float moveSpeed = 5f; // Velocidade de movimento

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // Normaliza a direção para evitar aceleração excessiva
            data.direction.Normalize();

            // Move o jogador aplicando força ao Rigidbody2D
            _rb.velocity = data.direction * moveSpeed;
        }
    }
}
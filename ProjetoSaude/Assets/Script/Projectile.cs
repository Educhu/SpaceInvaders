using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Projectile : NetworkBehaviour
{
    private PlayerMovementFusion owner; // Refer�ncia ao jogador que disparou o proj�til
    [SerializeField] private int pointsForKill = 50;  // Pontos por destruir um inimigo

    public void SetOwner(PlayerMovementFusion player)
    {
        owner = player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o proj�til colidiu com um inimigo
        if (collision.CompareTag("Enemy"))
        {
            // Chama o m�todo OnEnemyHit no jogador para adicionar pontos
            if (owner != null)
            {
                owner.OnEnemyHit(pointsForKill);
            }
            // Destr�i o inimigo
            Runner.Despawn(collision.GetComponent<NetworkObject>());

            // Destr�i o proj�til
            Runner.Despawn(Object);
        }
    }


}

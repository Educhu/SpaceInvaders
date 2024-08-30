using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerCollisionHandler : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se a colis�o foi com um inimigo
        if (other.CompareTag("Enemy"))
        {
            // Obt�m o NetworkObject do objeto com o qual colidiu
            NetworkObject enemyNetworkObject = other.GetComponent<NetworkObject>();

            // Se o NetworkObject for encontrado, processa a colis�o
            if (enemyNetworkObject != null)
            {
                // Se for o Host, lida com a colis�o diretamente
                if (Object.HasStateAuthority)
                {
                    HandleCollision(enemyNetworkObject);
                }
                else
                {
                    // Se for um cliente, envia um RPC para o Host
                    RPC_HandleCollision(enemyNetworkObject);
                }
            }
        }
    }

    // M�todo local para lidar com a colis�o no Host
    private void HandleCollision(NetworkObject enemyNetworkObject)
    {
        Debug.Log($"Colis�o detectada entre o Player e o Enemy: {enemyNetworkObject.name} pelo Host.");

        // Implementar l�gica adicional aqui, como reduzir a vida, aplicar dano, etc.
    }

    // RPC enviado por um cliente e processado pelo Host
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_HandleCollision(NetworkObject enemyNetworkObject)
    {
        Debug.Log($"Colis�o detectada entre o Player e o Enemy: {enemyNetworkObject.name} por um Cliente.");

        // L�gica de colis�o � executada no Host
        HandleCollision(enemyNetworkObject);
    }
}
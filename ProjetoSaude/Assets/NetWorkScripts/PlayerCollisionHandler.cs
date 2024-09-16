using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCollisionHandler : NetworkBehaviour
{
    private PlayerDataNetworked _playerData;

    private void Start()
    {
        _playerData = GetComponent<PlayerDataNetworked>();

        if (_playerData == null)
        {
            Debug.LogError("PlayerDataNetworked não encontrado no GameObject. Certifique-se de que este componente está presente.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug para mostrar o nome do objeto com o qual o player colidiu
        Debug.Log($"O player colidiu com: {other.gameObject.name}");

        // Verifica se a colisão foi com um inimigo
        if (other.CompareTag("Enemy"))
        {
            NetworkObject enemyNetworkObject = other.GetComponent<NetworkObject>();

            if (enemyNetworkObject != null)
            {
                // Se o cliente tem autoridade de estado, trata a colisão e destrói localmente
                if (Object.HasStateAuthority)
                {
                    Debug.Log("O player tem autoridade de estado. Lidando com a colisão localmente.");
                    HandleCollision(enemyNetworkObject);
                }
                else
                {
                    // Caso contrário, solicita ao host (StateAuthority) para destruir o objeto
                    Debug.Log("Cliente não tem autoridade de estado. Solicitando ao host para destruir o inimigo.");
                    RPC_DestroyEnemyObject(enemyNetworkObject.Id);
                }
            }
            else
            {
                Debug.LogWarning("Nenhum NetworkObject encontrado no objeto colidido.");
            }
        }
    }

    // Lida com a colisão e destrói o objeto na rede
    private void HandleCollision(NetworkObject enemyNetworkObject)
    {
        if (HasStateAuthority)
        {
            Debug.Log($"Colisão detectada entre o Player e o Enemy: {enemyNetworkObject.name} pelo Host.");

            // Reduz a vida do jogador se _playerData não for nulo
            if (_playerData != null)
            {
                _playerData.SubtractLife(); // Reduz a vida do jogador
            }
            else
            {
                Debug.LogWarning("PlayerDataNetworked não está presente. A vida do jogador não será subtraída.");
            }

            // Destrói o objeto de rede
            Debug.Log("Destruindo o inimigo na rede.");
            Runner.Despawn(enemyNetworkObject); // Usa Runner.Despawn para remover o objeto de rede
        }
    }

    // Define um RPC para destruir o objeto de rede no host
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_DestroyEnemyObject(NetworkId enemyNetworkObjectId)
    {
        Debug.Log($"Cliente solicitou destruição do objeto com NetworkId: {enemyNetworkObjectId}");

        // Apenas o cliente com autoridade de estado deve processar esta destruição
        if (HasStateAuthority)
        {
            // Obtém o objeto de rede a partir do NetworkId e o destrói
            NetworkObject enemyNetworkObject = Runner.FindObject(enemyNetworkObjectId);
            if (enemyNetworkObject != null)
            {
                Debug.Log("Objeto de rede encontrado. Destruindo o inimigo.");
                Runner.Despawn(enemyNetworkObject); // Remove o objeto de rede
            }
            else
            {
                Debug.LogError("NetworkObject não encontrado para o NetworkId: " + enemyNetworkObjectId);
            }
        }
    }
}
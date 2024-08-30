using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Unity.Collections.Unicode;


public class Enemy : NetworkBehaviour
{
    [Networked] private bool IsCollidingWithWall { get; set; }
    [Networked] private bool IsAlive { get; set; } = true; // Propriedade para controlar se o inimigo está vivo

    [SerializeField] private float despawnTime = 5.0f; // Tempo antes de despawn (em segundos)
    private TickTimer despawnTimer;

    private bool isInitialized = false;

    public override void Spawned()
    {
        base.Spawned();
        isInitialized = true;
        // Inicie o timer de despawn
        despawnTimer = TickTimer.CreateFromSeconds(Runner, despawnTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isInitialized || !Runner.IsRunning) return;

        // Verifica se a colisão é com uma parede
        if (collision.CompareTag("Wall"))
        {
            IsCollidingWithWall = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!isInitialized || !Object.HasStateAuthority) return;

        if (IsCollidingWithWall)
        {
            DespawnEnemy();
            IsCollidingWithWall = false; // Reseta o estado após o despawn
        }

        if (despawnTimer.Expired(Runner))
        {
            DespawnEnemy();
            despawnTimer = TickTimer.None; // Opcional: Desativa o timer após o despawn
        }
    }

    private void DespawnEnemy()
    {
        if (!isInitialized || !Object.HasStateAuthority) return;

        // Notifique todos os clientes para despawn o inimigo
        RPC_DespawnEnemy();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DespawnEnemy()
    {
        if (!isInitialized) return;

        // Desativa o GameObject para todos os clientes
        gameObject.SetActive(false);

        // Alternativamente, se preferir destruir o objeto
        // Runner.Despawn(Object);
    }
}
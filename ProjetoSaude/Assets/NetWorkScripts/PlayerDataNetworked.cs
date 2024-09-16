using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDataNetworked : NetworkBehaviour
{
    [Networked] public int Lives { get; set; }
    [Networked] public bool IsInvulnerable { get; set; }

    private float invulnerabilityDuration = 1.0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float blinkInterval = 0.1f;
    private Coroutine blinkCoroutine;

    private TextMeshProUGUI livesText;

    private void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        livesText = GetComponentInChildren<TextMeshProUGUI>(); // Certifique-se de que o TextMeshPro está no mesmo GameObject ou em um filho
    }

   

    public void TakeDamage(int damage)
    {
        if (HasStateAuthority && !IsInvulnerable)
        {
            Lives -= damage;
            Debug.Log("Player tomou " + damage + " de dano.");

            if (Lives <= 0)
            {
                Debug.Log("Player morreu.");
                RPC_NotifyDeath(); // Notifica todos os clientes sobre a morte do jogador
            }
            else
            {
                ActivateInvulnerability();
                // Atualiza o texto da vida em todos os clientes
                RPC_UpdateLivesText(Lives);
            }
        }
    }

    public void SubtractLife()
    {
        TakeDamage(1);
    }

    private void ActivateInvulnerability()
    {
        if (HasStateAuthority)
        {
            IsInvulnerable = true;
            Debug.Log("Player está invulnerável por " + invulnerabilityDuration + " segundos.");

            // Inicia ou reinicia o Coroutine de piscar
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
            }
            blinkCoroutine = StartCoroutine(Blink());

            Runner.StartCoroutine(DeactivateInvulnerabilityAfterDelay(invulnerabilityDuration));
        }
    }

    private IEnumerator DeactivateInvulnerabilityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (HasStateAuthority)
        {
            IsInvulnerable = false;
        }
    }

    private void OnInvulnerabilityStateChanged()
    {
        if (IsInvulnerable)
        {
            // Garante que o efeito de piscar é visível
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
        else
        {
            // Garante que a cor original é restaurada
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    private IEnumerator Blink()
    {
        while (IsInvulnerable)
        {
            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color == originalColor ? new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f) : originalColor;
                spriteRenderer.color = newColor;
                Debug.Log("Blinking: " + spriteRenderer.color);
            }
            yield return new WaitForSeconds(blinkInterval);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            Debug.Log("Restored Color: " + spriteRenderer.color);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyDeath()
    {
        // Usar NetworkRunner para despawn o objeto
        Runner.Despawn(Object);
        Debug.Log("Notificação de morte para todos os clientes.");
        // Aqui você pode adicionar qualquer lógica específica de morte que você deseja replicar
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateLivesText(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "" + lives;
            Debug.Log("Atualizando vidas para: " + lives);
        }
    }
}
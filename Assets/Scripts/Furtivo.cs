using UnityEngine;

public class Furtivo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            PlayerMovement scriptPlayer = outro.GetComponent<PlayerMovement>();
            if (scriptPlayer != null)
            {
                scriptPlayer.estaEscondido = true;
                
                // --- CORREÇÃO: Respeita a cor original do jogador ---
                SpriteRenderer sr = outro.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Pega a cor atual que o jogador já tem
                    Color corAtual = sr.color;
                    // Mantém R, G, B originais e muda o Alpha para 0.5f (transparente)
                    sr.color = new Color(corAtual.r, corAtual.g, corAtual.b, 0.5f);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            PlayerMovement scriptPlayer = outro.GetComponent<PlayerMovement>();
            if (scriptPlayer != null)
            {
                scriptPlayer.estaEscondido = false;
                
                // --- CORREÇÃO: Respeita a cor original do jogador ---
                SpriteRenderer sr = outro.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Pega a cor atual (que está transparente)
                    Color corAtual = sr.color;
                    // Mantém R, G, B originais e volta o Alpha para 1f (opaco)
                    sr.color = new Color(corAtual.r, corAtual.g, corAtual.b, 1.0f);
                }
            }
        }
    }
}
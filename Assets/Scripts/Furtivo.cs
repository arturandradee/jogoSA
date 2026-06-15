using UnityEngine;

public class Furtivo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            MovimentoJogador scriptJogador = outro.GetComponent<MovimentoJogador>();
            if (scriptJogador != null)
            {
                scriptJogador.estaEscondido = true;
                
                SpriteRenderer sr = outro.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color corAtual = sr.color;
                    sr.color = new Color(corAtual.r, corAtual.g, corAtual.b, 0.5f);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            MovimentoJogador scriptJogador = outro.GetComponent<MovimentoJogador>();
            if (scriptJogador != null)
            {
                scriptJogador.estaEscondido = false;
                
                SpriteRenderer sr = outro.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color corAtual = sr.color;
                    sr.color = new Color(corAtual.r, corAtual.g, corAtual.b, 1.0f);
                }
            }
        }
    }
}
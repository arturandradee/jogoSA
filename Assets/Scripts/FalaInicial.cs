using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FalaInicial : MonoBehaviour
{
    [Header("Configurações da Fala")]
    public Text textoUI; // Onde o texto vai aparecer
    public string mensagem = "Onde... onde eu estou? Minha cabeça dói...";
    public float tempoNaTela = 4f; // Quantos segundos a frase vai ficar na tela

    private bool jaDeuOPrimeiroPasso = false;

    void Start()
    {
        // Garante que o texto comece invisível quando o jogo abre
        if (textoUI != null)
        {
            textoUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Se ainda não falou, fica vigiando o teclado
        if (!jaDeuOPrimeiroPasso)
        {
            // Input.GetAxisRaw verifica se o jogador pressionou WASD ou as setinhas
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                DarOPrimeiroPasso();
            }
        }
    }

    void DarOPrimeiroPasso()
    {
        jaDeuOPrimeiroPasso = true; // Trava para não repetir mais

        if (textoUI != null)
        {
            textoUI.text = mensagem;
            textoUI.gameObject.SetActive(true); // Mostra o texto
            StartCoroutine(ApagarTextoDepoisDeUmTempo());
        }
    }

    IEnumerator ApagarTextoDepoisDeUmTempo()
    {
        // Espera os segundos que você configurou
        yield return new WaitForSeconds(tempoNaTela);
        
        // Esconde o texto
        if (textoUI != null)
        {
            textoUI.gameObject.SetActive(false);
        }

        // Destrói apenas ESTE script, pois não precisamos mais dele!
        Destroy(this); 
    }
}
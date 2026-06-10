using UnityEngine;

public class CameraSeguir: MonoBehaviour
{
    [Header("Configurações da Câmera")]
    public Transform alvo; // Quem a câmera vai seguir (o Jogador)
    public float suavizacao = 5f; // Velocidade que a câmera alcança o jogador (quanto maior, mais rápida)
    
    // A câmera 2D precisa ficar "para trás" da tela (no eixo Z), geralmente no -10.
    public Vector3 compensacao = new Vector3(0f, 0f, -10f); 

    void Start()
    {
        // Se você esquecer de arrastar o jogador, o código acha ele sozinho!
        if (alvo == null)
        {
            GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
            if (objJogador != null)
            {
                alvo = objJogador.transform;
            }
        }
    }

    void LateUpdate()
    {
        // Só tenta seguir se o jogador ainda existir (se ele não foi eliminado)
        if (alvo != null)
        {
            // Calcula o ponto exato onde a câmera deveria estar
            Vector3 posicaoDesejada = alvo.position + compensacao;
            
            // Move a câmera suavemente da posição atual até a posição desejada
            Vector3 posicaoSuavizada = Vector3.Lerp(transform.position, posicaoDesejada, suavizacao * Time.deltaTime);
            
            // Aplica o movimento na câmera
            transform.position = posicaoSuavizada;
        }
    }
}
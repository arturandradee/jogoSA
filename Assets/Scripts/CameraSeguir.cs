using UnityEngine;

public class CameraSeguir : MonoBehaviour
{
    [Header("Configurações da Câmera")]
    public Transform alvo; 
    public float suavizacao = 5f; 
    
    public Vector3 compensacao = new Vector3(0f, 0f, -10f); 

    void Start()
    {
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
        if (alvo != null)
        {
            Vector3 posicaoDesejada = alvo.position + compensacao;
            Vector3 posicaoSuavizada = Vector3.Lerp(transform.position, posicaoDesejada, suavizacao * Time.deltaTime);
            transform.position = posicaoSuavizada;
        }
    }
}
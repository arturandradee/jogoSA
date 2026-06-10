using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referências")]
    public GameObject[] enemyPrefabs; 
    public Transform player;       

    [Header("Limites do Mapa Jogável")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Configurações de Segurança")]
    public float distanciaMinimaPlayer = 4f; 

    [Header("Configurações do Sistema de Ondas")]
    [SerializeField] private int quantidadeParaSpawnar = 2; 

    void Update()
    {
        GameObject[] inimigosVivos = GameObject.FindGameObjectsWithTag("Enemy");

        if (inimigosVivos.Length == 0)
        {
            IniciarProximaOnda();
        }
    }

    void IniciarProximaOnda()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Nenhum prefab de inimigo foi colocado no EnemySpawner!");
            return;
        }

        Debug.Log("Onda limpa! Nova onda iniciada. Nascendo " + quantidadeParaSpawnar + " inimigos.");

        for (int i = 0; i < quantidadeParaSpawnar; i++)
        {
            SpawnarInimigoIndividual();
        }

        quantidadeParaSpawnar++; 
    }

    void SpawnarInimigoIndividual()
    {
        Vector3 posicaoSorteada = Vector3.zero;
        bool posicaoValida = false;
        int tentativas = 0;

        // --- CORRIGIDO AQUI: mudado de 'tentatives' para 'tentativas' ---
        while (!posicaoValida && tentativas < 100)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            posicaoSorteada = new Vector3(randomX, randomY, 0f);

            float distanciaDoPlayer = Vector3.Distance(posicaoSorteada, player.position);

            if (distanciaDoPlayer >= distanciaMinimaPlayer)
            {
                posicaoValida = true;
            }

            tentativas++;
        }

        int indiceAleatorio = Random.Range(0, enemyPrefabs.Length);
        GameObject inimigoSorteado = enemyPrefabs[indiceAleatorio];

        Instantiate(inimigoSorteado, posicaoSorteada, Quaternion.identity);
    }
}
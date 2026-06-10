using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações do Inimigo")]
    public Transform jogador;
    public float velocidade = 3f;
    public float distanciaPerseguicao = 5f;
    
    [Tooltip("Distância que ele para de andar para não tremer em cima do alvo")]
    public float distanciaParada = 1.2f; 
    
    public float intervaloDanoLobo = 1f; 

    [Header("Configurações de Passeio")]
    public float raioPasseio = 3f;
    public float tempoPasseio = 2f;

    [Header("Status")]
    public int vida = 3;

    private float cronometroPasseio;
    private float tempoProximoAtaqueLobo; 
    
    private Vector2 alvoPasseio;
    
    private Rigidbody2D rb;
    private SpriteRenderer sr; 
    
    private PlayerMovement scriptJogador;
    private PetAI scriptLobo;
    private Transform alvoAtual;
    private bool estaPiscando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); 
        cronometroPasseio = tempoPasseio;

        // Procura o jogador na cena
        GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
        if (objJogador != null)
        {
            jogador = objJogador.transform;
            scriptJogador = objJogador.GetComponent<PlayerMovement>();
        }

        // Procura o lobo na cena
        GameObject objLobo = GameObject.FindGameObjectWithTag("Pet");
        if (objLobo != null)
        {
            scriptLobo = objLobo.GetComponent<PetAI>();
        }

        DefinirNovoAlvoPasseio();
    }

    void FixedUpdate()
    {
        // 1. Escolhe quem ele vai tentar atacar
        escolherAlvo();

        // Se não tiver alvo nenhum, fica passeando
        if (alvoAtual == null)
        {
            Passear();
            return;
        }

        // Calcula a distância exata até o alvo
        float distancia = Vector2.Distance(transform.position, alvoAtual.position);
        
        // Verifica se o alvo é o jogador E se o jogador está escondido na moita
        bool alvoEscondido = false;
        if (alvoAtual == jogador && scriptJogador != null && scriptJogador.estaEscondido)
        {
            alvoEscondido = true;
        }

        // 2. Se estiver na área de visão e o alvo não estiver escondido, persegue!
        if (distancia <= distanciaPerseguicao && !alvoEscondido)
        {
            // --- NOVA LÓGICA DE ATAQUE ---
            // Se o alvo for o lobo e estivermos colados nele (na distância de parada), ataca!
            if (alvoAtual == scriptLobo.transform && distancia <= distanciaParada + 0.2f)
            {
                AtacarLobo();
            }

            // O inimigo só anda se estiver mais longe que a distância de parada (Evita o tremor)
            if (distancia > distanciaParada)
            {
                Vector2 direcao = ((Vector2)alvoAtual.position - rb.position).normalized;
                Vector2 novaPosicao = rb.position + direcao * velocidade * Time.fixedDeltaTime;
                rb.MovePosition(novaPosicao);
            }
        }
        else
        {
            // Se estiver muito longe ou escondido, volta a passear
            Passear();
        }
    }

    void escolherAlvo()
    {
        // Prioridade: Tenta focar no lobo primeiro
        if (scriptLobo != null && !scriptLobo.desmaiado)
        {
            float distLobo = Vector2.Distance(transform.position, scriptLobo.transform.position);
            
            // Se o lobo estiver perto, o inimigo foca nele
            if (distLobo < 2.5f) 
            { 
                alvoAtual = scriptLobo.transform;
                return;
            }
        }
        
        // Se o lobo estiver longe ou desmaiado, o alvo passa a ser o jogador
        alvoAtual = jogador;
    }

    void Passear()
    {
        cronometroPasseio += Time.fixedDeltaTime;
        
        // Sorteia um novo ponto para andar se o tempo acabar ou se ele chegar no ponto atual
        if (cronometroPasseio >= tempoPasseio || Vector2.Distance(rb.position, alvoPasseio) < 0.2f)
        {
            DefinirNovoAlvoPasseio();
            cronometroPasseio = 0;
        }
        
        // Anda devagarzinho até o ponto sorteado
        Vector2 direcao = (alvoPasseio - rb.position).normalized;
        Vector2 novaPosicao = rb.position + direcao * (velocidade * 0.5f) * Time.fixedDeltaTime;
        rb.MovePosition(novaPosicao);
    }

    void DefinirNovoAlvoPasseio()
    {
        Vector2 direcaoAleatoria = Random.insideUnitCircle * raioPasseio;
        alvoPasseio = rb.position + direcaoAleatoria;
    }

    // --- SISTEMA DE DANO DO INIMIGO ---
    public void ReceberDano(int quantidadeDano)
    {
        vida -= quantidadeDano;
        
        if (!estaPiscando) 
        { 
            StartCoroutine(FlashDanoSuperVisivel()); 
        }
        
        if (vida <= 0) 
        { 
            Morrer(); 
        }
    }

    IEnumerator FlashDanoSuperVisivel()
    {
        if (sr != null)
        {
            estaPiscando = true;
            Color corOriginal = sr.color;
            
            sr.color = Color.white; 
            yield return new WaitForSeconds(0.1f);
            
            sr.color = new Color(0, 0, 0, 0); 
            yield return new WaitForSeconds(0.05f);
            
            sr.color = corOriginal;
            estaPiscando = false;
        }
    }

    void Morrer() 
    { 
        Debug.Log("Inimigo derrotado!"); 
        Destroy(gameObject); 
    }

    // --- NOVO SISTEMA DE ATAQUE AO LOBO (POR DISTÂNCIA) ---
    void AtacarLobo()
    {
        // Checa se o relógio global já passou do tempo de espera do ataque
        if (Time.time >= tempoProximoAtaqueLobo)
        {
            // Só dá o dano se o lobo existir e NÃO estiver desmaiado
            if (scriptLobo != null && !scriptLobo.desmaiado) 
            {
                scriptLobo.ReceberDano(1);
                Debug.Log("Inimigo mordeu o Lobo (Matemática pura)!"); 
                
                // Agenda quando será o próximo ataque
                tempoProximoAtaqueLobo = Time.time + intervaloDanoLobo;
            }
        }
    }

    // --- ATAQUE NO JOGADOR (COLISÃO FÍSICA) ---
    // Encostou = Game Over
    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over! O Inimigo tocou no Jogador."); 
            Destroy(colisao.gameObject); 
        }
    }
}
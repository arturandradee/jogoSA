using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações do Inimigo")]
    public Transform jogador;
    public float velocidade = 3f;
    public float distanciaPerseguicao = 5f;
    public float distanciaParada = 1.2f; 
    public float intervaloDanoLobo = 1f; 

    [Header("Configurações de Passeio")]
    public float raioPasseio = 3f;
    public float tempoPasseio = 2f;

    [Header("Status")]
    public int vida = 3;
    private int vidaMaxima;

    [Header("UI da Vida")]
    public GameObject canvasVida; 
    public Slider barraVidaUI;    

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
        vidaMaxima = vida;

        if (barraVidaUI != null)
        {
            barraVidaUI.maxValue = vidaMaxima;
            barraVidaUI.value = vidaMaxima;
        }

        if (canvasVida != null)
        {
            canvasVida.SetActive(false);
        }

        // Encontra o Jogador automaticamente pela Tag e pega o script de movimento
        GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
        if (objJogador != null)
        {
            jogador = objJogador.transform;
            scriptJogador = objJogador.GetComponent<PlayerMovement>();
        }

        // Encontra o Lobo automaticamente pela Tag
        GameObject objLobo = GameObject.FindGameObjectWithTag("Pet");
        if (objLobo != null)
        {
            scriptLobo = objLobo.GetComponent<PetAI>();
        }

        DefinirNovoAlvoPasseio();
    }

    void FixedUpdate()
    {
        escolherAlvo();

        if (alvoAtual == null)
        {
            Passear();
            return;
        }

        float distancia = Vector2.Distance(transform.position, alvoAtual.position);
        
        bool alvoEscondido = false;
        
        // Se o alvo for o jogador, checa se ele está escondido na moita (ativado pelo Furtivo.cs)
        if (alvoAtual == jogador && scriptJogador != null && scriptJogador.estaEscondido)
        {
            alvoEscondido = true;
        }

        // Se estiver perto e o alvo não estiver escondido, persegue
        if (distancia <= distanciaPerseguicao && !alvoEscondido)
        {
            if (alvoAtual == scriptLobo.transform && distancia <= distanciaParada + 0.2f)
            {
                AtacarLobo();
            }

            if (distancia > distanciaParada)
            {
                Vector2 direcao = ((Vector2)alvoAtual.position - rb.position).normalized;
                Vector2 novaPosicao = rb.position + direcao * velocidade * Time.fixedDeltaTime; 
                rb.MovePosition(novaPosicao);
            }
        }
        else
        {
            // Se o jogador se esconder ou se afastar, o inimigo volta a passear de boa
            Passear();
        }
    }

    void escolherAlvo()
    {
        // Prioriza o lobo se ele estiver vivo e muito perto (menos de 2.5 unidades de distância)
        if (scriptLobo != null && !scriptLobo.desmaiado)
        {
            float distLobo = Vector2.Distance(transform.position, scriptLobo.transform.position);
            
            if (distLobo < 2.5f) 
            { 
                alvoAtual = scriptLobo.transform;
                return;
            }
        }
        
        // Se o lobo não estiver por perto ou estiver desmaiado, o foco volta a ser o jogador
        alvoAtual = jogador;
    }

    void Passear()
    {
        cronometroPasseio += Time.fixedDeltaTime;
        
        if (cronometroPasseio >= tempoPasseio || Vector2.Distance(rb.position, alvoPasseio) < 0.2f)
        {
            DefinirNovoAlvoPasseio();
            cronometroPasseio = 0;
        }
        
        Vector2 direcao = (alvoPasseio - rb.position).normalized;
        Vector2 novaPosicao = rb.position + direcao * (velocidade * 0.5f) * Time.fixedDeltaTime;
        rb.MovePosition(novaPosicao);
    }

    void DefinirNovoAlvoPasseio()
    {
        Vector2 direcaoAleatoria = Random.insideUnitCircle * raioPasseio;
        alvoPasseio = rb.position + direcaoAleatoria;
    }

    public void ReceberDano(int quantidadeDano)
    {
        if (vida <= 0) return; 

        vida -= quantidadeDano;
        
        if (canvasVida != null) canvasVida.SetActive(true); 
        if (barraVidaUI != null) barraVidaUI.value = vida; 
        
        if (!estaPiscando) StartCoroutine(FlashDanoSuperVisivel()); 
        if (vida <= 0) Morrer(); 
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

    // Método de morte simplificado para conversar com o EnemySpawner
    void Morrer() 
    { 
        Debug.Log("Inimigo derrotado!"); 
        Destroy(gameObject); // O Spawner vai notar a falta deste objeto automaticamente
    }

    void AtacarLobo()
    {
        if (Time.time >= tempoProximoAtaqueLobo)
        {
            if (scriptLobo != null && !scriptLobo.desmaiado) 
            {
                scriptLobo.ReceberDano(1);
                tempoProximoAtaqueLobo = Time.time + intervaloDanoLobo;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        // Se encostar fisicamente no jogador, derrota ele imediatamente
        if (colisao.gameObject.CompareTag("Player"))
        {
            Destroy(colisao.gameObject); 
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteligenciaInimigo : MonoBehaviour
{
    public Transform jogador;
    public float velocidade = 3f;
    public float distanciaPerseguicao = 5f;
    public float distanciaParada = 1.2f; 
    public float intervaloDanoLobo = 1f; 

    public float raioPasseio = 3f;
    public float tempoPasseio = 2f;

    public int vida = 3;
    private int vidaMaxima;

    public GameObject canvasVida; 
    public Slider barraVidaUI;    

    private float cronometroPasseio;
    private float tempoProximoAtaqueLobo; 
    private Vector2 alvoPasseio;
    private Rigidbody2D rb;
    private SpriteRenderer sr; 
    
    private MovimentoJogador scriptJogador; 
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

        GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
        if (objJogador != null)
        {
            jogador = objJogador.transform;
            scriptJogador = objJogador.GetComponent<MovimentoJogador>();
        }

        GameObject objLobo = GameObject.FindGameObjectWithTag("Pet");
        if (objLobo != null)
        {
            scriptLobo = objLobo.GetComponent<PetAI>();
        }

        DefinirNovoAlvoPasseio();
    }

    void FixedUpdate()
    {
        EscolherAlvo();

        if (alvoAtual == null)
        {
            Passear();
            return;
        }

        float distancia = Vector2.Distance(transform.position, alvoAtual.position);
        bool alvoEscondido = false;
        
        if (alvoAtual == jogador && scriptJogador != null && scriptJogador.estaEscondido)
        {
            alvoEscondido = true;
        }

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
            Passear();
        }
    }

    void EscolherAlvo()
    {
        if (scriptLobo != null && !scriptLobo.desmaiado)
        {
            float distLobo = Vector2.Distance(transform.position, scriptLobo.transform.position);
            
            if (distLobo < 2.5f) 
            { 
                alvoAtual = scriptLobo.transform;
                return;
            }
        }
        
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

    void Morrer() 
    { 
        Debug.Log("Inimigo derrotado!"); 
        Destroy(gameObject); 
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
        if (colisao.gameObject.CompareTag("Player"))
        {
            Destroy(colisao.gameObject); 
        }
    }
}
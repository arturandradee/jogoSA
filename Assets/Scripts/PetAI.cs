using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PetAI : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public Transform jogador;
    public float velocidade = 6f;
    public float distanciaSeguir = 1.5f;

    [Header("Configurações de Ataque")]
    public float alcanceAtaque = 1.5f; 
    public float intervaloAtaque = 0.5f; 
    public float raioAutoAtaque = 3.0f; 

    [Header("Sistema de Vida e Regeneração")]
    public int vidaMaxima = 5;
    public Slider barraVidaLobo;
    public float tempoParaRegenerar = 5f; 
    public float intervaloCura = 1f; 
    public float tempoInativo = 20f; 

    [Header("UI de Texto (TextMeshPro)")]
    public TextMeshProUGUI textoCronometro; 

    private float cronometroForaCombate;
    private float cronometroCura;
    private int vidaAtual;
    private float cronometroAtaque;

    private SpriteRenderer sr;
    private SpriteRenderer srJogador;
    private Animator anim;

    private Transform inimigoAlvo;
    private InteligenciaInimigo scriptInimigo;

    private bool focadoNoRetorno = false;

    [HideInInspector]
    public bool desmaiado = false;

    private enum EstadoPet { Seguindo, Perseguindo, Atacando }
    private EstadoPet estadoAtual = EstadoPet.Seguindo;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (jogador != null)
            srJogador = jogador.GetComponent<SpriteRenderer>();

        vidaAtual = vidaMaxima;

        if (barraVidaLobo != null)
        {
            barraVidaLobo.maxValue = vidaMaxima;
            barraVidaLobo.value = vidaAtual;
        }

        if (textoCronometro != null)
            textoCronometro.gameObject.SetActive(false);
    }

    void Update()
    {
        if (desmaiado) return;

        if (cronometroAtaque > 0)
            cronometroAtaque -= Time.deltaTime;

        // 🔥 COPIA DIREÇÃO DO PLAYER (COM INVERSÃO)
        VirarIgualJogador();

        // Botão G = voltar
        if (Input.GetKeyDown(KeyCode.G))
        {
            inimigoAlvo = null;
            estadoAtual = EstadoPet.Seguindo;
            focadoNoRetorno = true;
        }

        // Botão F = atacar
        if (Input.GetKeyDown(KeyCode.F) && estadoAtual == EstadoPet.Seguindo)
        {
            focadoNoRetorno = false;
            EncontrarInimigoMaisProximo();
        }

        // Auto ataque
        if (estadoAtual == EstadoPet.Seguindo && !focadoNoRetorno)
        {
            VigiarArredores();
        }

        // Regeneração
        if (vidaAtual < vidaMaxima && estadoAtual != EstadoPet.Atacando)
        {
            ContarRegeneracao();
        }
        else
        {
            cronometroForaCombate = 0;
            cronometroCura = 0;
        }
    }

    void FixedUpdate()
    {
        if (desmaiado) return;

        switch (estadoAtual)
        {
            case EstadoPet.Seguindo: SeguirJogador(); break;
            case EstadoPet.Perseguindo: PerseguirInimigo(); break;
            case EstadoPet.Atacando: AtacarInimigo(); break;
        }
    }

    // 🔥 AQUI ESTÁ A CORREÇÃO DO INVERTIDO
    void VirarIgualJogador()
    {
        if (srJogador == null) return;

        sr.flipX = !srJogador.flipX; // 👈 inversão aplicada
    }

    void SeguirJogador()
    {
        if (jogador == null) return;

        float dist = Vector2.Distance(transform.position, jogador.position);

        if (dist > distanciaSeguir)
        {
            Vector2 direcao = (jogador.position - transform.position).normalized;

            transform.position = Vector2.MoveTowards(
                transform.position,
                jogador.position,
                velocidade * Time.fixedDeltaTime
            );

            anim.SetFloat("MoveX", direcao.x);
            anim.SetFloat("MoveY", direcao.y);
            anim.SetFloat("Speed", 1);
        }
        else
        {
            anim.SetFloat("Speed", 0);

            if (focadoNoRetorno)
                focadoNoRetorno = false;
        }
    }

    void PerseguirInimigo()
    {
        if (inimigoAlvo == null)
        {
            estadoAtual = EstadoPet.Seguindo;
            return;
        }

        float dist = Vector2.Distance(transform.position, inimigoAlvo.position);

        if (dist <= alcanceAtaque)
        {
            estadoAtual = EstadoPet.Atacando;
        }
        else
        {
            Vector2 direcao = (inimigoAlvo.position - transform.position).normalized;

            transform.position = Vector2.MoveTowards(
                transform.position,
                inimigoAlvo.position,
                velocidade * Time.fixedDeltaTime
            );

            anim.SetFloat("MoveX", direcao.x);
            anim.SetFloat("MoveY", direcao.y);
            anim.SetFloat("Speed", 1);
        }
    }

    void AtacarInimigo()
    {
        if (inimigoAlvo == null)
        {
            estadoAtual = EstadoPet.Seguindo;
            return;
        }

        float dist = Vector2.Distance(transform.position, inimigoAlvo.position);

        if (dist > alcanceAtaque + 0.5f)
        {
            estadoAtual = EstadoPet.Perseguindo;
            return;
        }

        anim.SetFloat("Speed", 0);
        cronometroForaCombate = 0;

        if (cronometroAtaque <= 0)
        {
            if (scriptInimigo != null)
                scriptInimigo.ReceberDano(1);

            cronometroAtaque = intervaloAtaque;
        }
    }

    void VigiarArredores()
    {
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject inimigo in inimigos)
        {
            float dist = Vector2.Distance(transform.position, inimigo.transform.position);

            if (dist <= raioAutoAtaque)
            {
                inimigoAlvo = inimigo.transform;
                scriptInimigo = inimigo.GetComponent<InteligenciaInimigo>();
                estadoAtual = EstadoPet.Perseguindo;
                break;
            }
        }
    }

    void EncontrarInimigoMaisProximo()
    {
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Enemy");

        float menorDistancia = Mathf.Infinity;
        Transform maisProximo = null;

        foreach (GameObject inimigo in inimigos)
        {
            float dist = Vector2.Distance(transform.position, inimigo.transform.position);

            if (dist < menorDistancia)
            {
                menorDistancia = dist;
                maisProximo = inimigo.transform;
            }
        }

        if (maisProximo != null)
        {
            inimigoAlvo = maisProximo;
            scriptInimigo = maisProximo.GetComponent<InteligenciaInimigo>();
            estadoAtual = EstadoPet.Perseguindo;
        }
    }

    public void ReceberDano(int dano)
    {
        if (desmaiado) return;

        vidaAtual -= dano;

        if (barraVidaLobo != null)
            barraVidaLobo.value = vidaAtual;

        cronometroForaCombate = 0;

        StartCoroutine(PiscarDanoLobo());

        if (vidaAtual <= 0)
            StartCoroutine(RotinaDesmaio());
    }

    void ContarRegeneracao()
    {
        cronometroForaCombate += Time.deltaTime;

        if (cronometroForaCombate >= tempoParaRegenerar)
        {
            cronometroCura += Time.deltaTime;

            if (cronometroCura >= intervaloCura)
            {
                vidaAtual = Mathf.Min(vidaAtual + 1, vidaMaxima);

                if (barraVidaLobo != null)
                    barraVidaLobo.value = vidaAtual;

                cronometroCura = 0;
            }
        }
    }

    IEnumerator PiscarDanoLobo()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    IEnumerator RotinaDesmaio()
    {
        desmaiado = true;

        if (barraVidaLobo != null)
            barraVidaLobo.gameObject.SetActive(false);

        if (textoCronometro != null)
            textoCronometro.gameObject.SetActive(true);

        sr.color = new Color(1, 1, 1, 0.3f);
        transform.rotation = Quaternion.Euler(0, 0, 90);

        float tempoRestante = tempoInativo;

        while (tempoRestante > 0)
        {
            if (textoCronometro != null)
                textoCronometro.text = "Descansando: " + tempoRestante.ToString("F0") + "s";

            yield return new WaitForSeconds(1f);
            tempoRestante--;
        }

        vidaAtual = vidaMaxima;

        if (barraVidaLobo != null)
        {
            barraVidaLobo.value = vidaAtual;
            barraVidaLobo.gameObject.SetActive(true);
        }

        if (textoCronometro != null)
            textoCronometro.gameObject.SetActive(false);

        sr.color = Color.white;
        transform.rotation = Quaternion.identity;

        desmaiado = false;
        estadoAtual = EstadoPet.Seguindo;
    }
}
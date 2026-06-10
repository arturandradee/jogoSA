using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidadeMovimento = 5f;
    public float velocidadeCorrida = 8f;

    [Header("Configurações de Estamina")]
    public Slider barraEstamina;
    public float estaminaMaxima = 100f;
    public float taxaConsumo = 20f;
    public float taxaRegeneracao = 15f;
    
    private float estaminaAtual;
    private bool podeCorrer = true;

    [HideInInspector]
    public bool estaEscondido = false;

    private Rigidbody2D rb;
    private Vector2 movimento;
    private float velocidadeAtual;

    // 🔥 NOVO
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // 🔥

        estaminaAtual = estaminaMaxima;

        if (barraEstamina != null)
        {
            barraEstamina.maxValue = estaminaMaxima;
            barraEstamina.value = estaminaMaxima;
        }
    }

    void Update()
    {
        float moverX = Input.GetAxis("Horizontal"); 
        float moverY = Input.GetAxis("Vertical");   

        movimento = new Vector2(moverX, moverY).normalized;

        // 🔥 FAZ O PLAYER VIRAR
        if (moverX > 0.1f)
            sr.flipX = false;
        else if (moverX < -0.1f)
            sr.flipX = true;

        ControlarEstamina();
    }

    void ControlarEstamina()
    {
        bool tentandoCorrer = Input.GetKey(KeyCode.LeftShift) && movimento.magnitude > 0;

        if (tentandoCorrer && podeCorrer && estaminaAtual > 0)
        {
            velocidadeAtual = velocidadeCorrida;
            estaminaAtual -= taxaConsumo * Time.deltaTime;

            if (estaminaAtual <= 0)
            {
                estaminaAtual = 0;
                podeCorrer = false;
            }
        }
        else
        {
            velocidadeAtual = velocidadeMovimento;
            
            if (estaminaAtual < estaminaMaxima)
            {
                estaminaAtual += taxaRegeneracao * Time.deltaTime;
            }

            if (!podeCorrer && estaminaAtual >= (estaminaMaxima * 0.2f))
            {
                podeCorrer = true;
            }
        }

        if (barraEstamina != null)
        {
            barraEstamina.value = estaminaAtual;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movimento * velocidadeAtual;
    }
}
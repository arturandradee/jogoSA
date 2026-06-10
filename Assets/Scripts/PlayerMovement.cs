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

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
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
        float moverX = Input.GetAxisRaw("Horizontal"); 
        float moverY = Input.GetAxisRaw("Vertical");   

        movimento = new Vector2(moverX, moverY).normalized;

        // 🔥 FAZ O PLAYER VIRAR
        if (moverX > 0.1f){
            anim.SetBool("isWalking", true);
            sr.flipX = false;
        }
        else if (moverX < -0.1f){
            anim.SetBool("isWalking", true);
            sr.flipX = true;
            }
        else if(moverX == 0f && moverX == 0f){
            anim.SetBool("isWalking", false);
        }

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
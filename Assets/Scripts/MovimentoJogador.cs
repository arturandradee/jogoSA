using UnityEngine;
using UnityEngine.UI;

public class MovimentoJogador : MonoBehaviour
{
    public float velocidadeMovimento = 5f;
    public float velocidadeCorrida = 8f;

    public KeyCode teclaCima = KeyCode.W;
    public KeyCode teclaBaixo = KeyCode.S;
    public KeyCode teclaEsquerda = KeyCode.A;
    public KeyCode teclaDireita = KeyCode.D;
    public KeyCode teclaCorrer = KeyCode.LeftShift;

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

    private SpriteRenderer sr;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        estaminaAtual = estaminaMaxima;

        if (barraEstamina != null)
        {
            barraEstamina.maxValue = estaminaMaxima;
            barraEstamina.value = estaminaMaxima;
        }
    }

    void Update()
    {
        float moverX = 0f;
        float moverY = 0f;

        if (Input.GetKey(teclaDireita)) moverX = 1f;
        else if (Input.GetKey(teclaEsquerda)) moverX = -1f;

        if (Input.GetKey(teclaCima)) moverY = 1f;
        else if (Input.GetKey(teclaBaixo)) moverY = -1f;

        movimento = new Vector2(moverX, moverY).normalized;

        if (movimento.magnitude > 0)
        {
            if (Mathf.Abs(moverX) > 0.1f)
            {
                anim.SetBool("AndandoFrente", false);
                anim.SetBool("AndandoCostas", false);
                anim.SetBool("andando", true);
                sr.flipX = (moverX < -0.1f);
            }
            else if (moverY > 0.1f)
            {
                anim.SetBool("andando", false);
                anim.SetBool("AndandoFrente", false);
                anim.SetBool("AndandoCostas", true);
            }
            else if (moverY < -0.1f)
            {
                anim.SetBool("andando", false);
                anim.SetBool("AndandoCostas", false);
                anim.SetBool("AndandoFrente", true);
            }
        }
        else
        {
            anim.SetBool("andando", false);
            anim.SetBool("AndandoFrente", false);
            anim.SetBool("AndandoCostas", false);
        }

        ControlarEstamina();
    }

    void ControlarEstamina()
    {
        bool tentandoCorrer = Input.GetKey(teclaCorrer) && movimento.magnitude > 0;

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
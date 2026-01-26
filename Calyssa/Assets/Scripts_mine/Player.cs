using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 12f;

    [Header("Objetos de Animação (Arraste da Hierarchy)")]
    public GameObject objetoOcioso;
    public GameObject objetoAndando;
    public GameObject objetoPular;
    public GameObject objetoAtacar;

    [Header("Verificação de Chão")]
    public Transform verificadorChao;
    public float raioVerificacao = 0.2f;
    public LayerMask camadaChao;

    private Rigidbody2D rb;
    private float inputH;
    private bool estaNoChao;
    private bool estaAtacando;
    private float tempoAtaque = 0.5f; // Ajuste conforme a duração da sua animação
    private float cronometroAtaque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (estaAtacando)
        {
            cronometroAtaque -= Time.deltaTime;
            if (cronometroAtaque <= 0) estaAtacando = false;
            
            // Durante o ataque, o personagem não se move ou pula
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            inputH = Input.GetAxisRaw("Horizontal");

            // Comando de Pulo
            if (Input.GetButtonDown("Jump") && estaNoChao)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            }

            // Comando de Ataque (Botão X)
            if (Input.GetKeyDown(KeyCode.X))
            {
                Atacar();
            }

            // Inverter Escala (Lado)
            if (inputH > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (inputH < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        GerenciarObjetosAnimacao();
    }

    void FixedUpdate()
    {
        if (!estaAtacando)
        {
            rb.linearVelocity = new Vector2(inputH * velocidade, rb.linearVelocity.y);
        }
        estaNoChao = Physics2D.OverlapCircle(verificadorChao.position, raioVerificacao, camadaChao);
    }

    void Atacar()
    {
        estaAtacando = true;
        cronometroAtaque = tempoAtaque;
    }

    void GerenciarObjetosAnimacao()
    {
        // Desativa todos primeiro
        objetoOcioso.SetActive(false);
        objetoAndando.SetActive(false);
        objetoPular.SetActive(false);
        objetoAtacar.SetActive(false);

        // Ativa apenas o correto baseado no estado
        if (estaAtacando)
        {
            objetoAtacar.SetActive(true);
        }
        else if (!estaNoChao)
        {
            objetoPular.SetActive(true);
        }
        else if (Mathf.Abs(inputH) > 0.1f)
        {
            objetoAndando.SetActive(true);
        }
        else
        {
            objetoOcioso.SetActive(true);
        }
    }
}
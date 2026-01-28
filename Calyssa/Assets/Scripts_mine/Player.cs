using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 12f;
    public int pulosExtrasMax = 1; // Quantidade de pulos extras (1 = Pulo Duplo)

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
    private float tempoAtaque = 0.5f; 
    private float cronometroAtaque;
    
    private int pulosRestantes; // Contador de pulos

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pulosRestantes = pulosExtrasMax; // Inicializa os pulos
    }

    void Update()
    {
        // Reseta o contador de pulos quando encosta no chão
        if (estaNoChao)
        {
            pulosRestantes = pulosExtrasMax;
        }

        if (estaAtacando)
        {
            cronometroAtaque -= Time.deltaTime;
            if (cronometroAtaque <= 0) estaAtacando = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            inputH = Input.GetAxisRaw("Horizontal");

            // Lógica de Pulo Duplo
            if (Input.GetButtonDown("Jump"))
            {
                if (estaNoChao)
                {
                    Pular();
                }
                else if (pulosRestantes > 0)
                {
                    Pular();
                    pulosRestantes--; // Consome um pulo extra
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Atacar();
            }

            if (inputH > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (inputH < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        GerenciarObjetosAnimacao();
    }

    void Pular()
    {
        // Reseta a velocidade vertical para o segundo pulo ter a mesma força que o primeiro
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);

        // Se for o pulo no ar, reiniciamos o objeto de animação para dar o feedback visual
        if (!estaNoChao)
        {
            objetoPular.SetActive(false);
            objetoPular.SetActive(true);
        }
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
        // Se estivermos atacando, apenas o objeto de ataque fica ativo
        if (estaAtacando)
        {
            SetAnimacaoAtiva(objetoAtacar);
            return;
        }

        if (!estaNoChao)
        {
            SetAnimacaoAtiva(objetoPular);
        }
        else if (Mathf.Abs(inputH) > 0.1f)
        {
            SetAnimacaoAtiva(objetoAndando);
        }
        else
        {
            SetAnimacaoAtiva(objetoOcioso);
        }
    }

    // Função auxiliar para evitar repetir código de SetActive
    void SetAnimacaoAtiva(GameObject ativo)
    {
        objetoOcioso.SetActive(objetoOcioso == ativo);
        objetoAndando.SetActive(objetoAndando == ativo);
        objetoPular.SetActive(objetoPular == ativo);
        objetoAtacar.SetActive(objetoAtacar == ativo);
    }
}
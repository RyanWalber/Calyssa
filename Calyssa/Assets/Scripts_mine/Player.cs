using UnityEngine;
using DragonBones; 
// Esta linha abaixo resolve o erro da imagem!
using Transform = UnityEngine.Transform; 

public class Player : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 12f;
    public int pulosExtrasMax = 1; 

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
    
    private int pulosRestantes; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pulosRestantes = pulosExtrasMax; 
    }

    void Update()
    {
        // Verifica o chão
        estaNoChao = Physics2D.OverlapCircle(verificadorChao.position, raioVerificacao, camadaChao);

        if (estaNoChao && rb.linearVelocity.y <= 0.1f)
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

            if (Input.GetButtonDown("Jump"))
            {
                if (estaNoChao)
                {
                    Pular();
                }
                else if (pulosRestantes > 0)
                {
                    pulosRestantes--; 
                    Pular();
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);

        // Reinicia a animação de pulo para dar o efeito visual
        SetAnimacaoAtiva(objetoPular);
        
        UnityArmatureComponent armature = objetoPular.GetComponent<UnityArmatureComponent>();
        if (armature != null && armature.animation.animationNames.Count > 0)
        {
            // Toca a animação de pulo apenas 1 VEZ (sem repetir no ar)
            armature.animation.Play(armature.animation.animationNames[0], 1);
        }
    }

    void FixedUpdate()
    {
        if (!estaAtacando)
        {
            rb.linearVelocity = new Vector2(inputH * velocidade, rb.linearVelocity.y);
        }
    }

    void Atacar()
    {
        estaAtacando = true;
        cronometroAtaque = tempoAtaque;

        SetAnimacaoAtiva(objetoAtacar);
        UnityArmatureComponent armature = objetoAtacar.GetComponent<UnityArmatureComponent>();
        if (armature != null && armature.animation.animationNames.Count > 0)
        {
            armature.animation.Play(armature.animation.animationNames[0], 1);
        }
    }

    void GerenciarObjetosAnimacao()
    {
        if (estaAtacando)
        {
            SetAnimacaoAtiva(objetoAtacar);
            return;
        }

        // Se não estiver no chão, mantém o objeto de pulo
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

    void SetAnimacaoAtiva(GameObject ativo)
    {
        // Só muda se o objeto já não for o ativo (evita resets desnecessários)
        if (objetoOcioso.activeSelf != (objetoOcioso == ativo)) objetoOcioso.SetActive(objetoOcioso == ativo);
        if (objetoAndando.activeSelf != (objetoAndando == ativo)) objetoAndando.SetActive(objetoAndando == ativo);
        if (objetoPular.activeSelf != (objetoPular == ativo)) objetoPular.SetActive(objetoPular == ativo);
        if (objetoAtacar.activeSelf != (objetoAtacar == ativo)) objetoAtacar.SetActive(objetoAtacar == ativo);
    }
}
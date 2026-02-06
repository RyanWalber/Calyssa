using UnityEngine;
using DragonBones;
using System.Collections;
using Transform = UnityEngine.Transform;

public class Player : MonoBehaviour
{
    [Header("VIDA E STATUS")] // --- NOVO ---
    public int vidaAtual = 3;
    public int vidaMaxima = 3;
    public bool invencivel = false; // Para evitar dano duplo

    [Header("Configurações de Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 12f;
    public int pulosExtrasMax = 1;

    [Header("Objetos de Animação")]
    public GameObject objetoOcioso;
    public GameObject objetoAndando;
    public GameObject objetoPular;
    public GameObject objetoAtacar;

    [Header("Verificação de Chão")]
    public Transform verificadorChao;
    public float raioVerificacao = 0.2f;
    public LayerMask camadaChao;

    [Header("Sons (Arraste os arquivos aqui)")]
    public AudioClip somAtaqueLanterna;
    public AudioClip somPular;
    public AudioClip somDano;
    public AudioClip somAndar;

    [Header("Controle de Volume (0 a 1)")]
    [Range(0f, 1f)] public float volPassos = 0.3f;
    [Range(0f, 1f)] public float volPulo = 0.8f;
    [Range(0f, 1f)] public float volAtaque = 1.0f;
    [Range(0f, 1f)] public float volDano = 1.0f;

    [Header("Suavidade dos Passos")]
    public float velocidadeFade = 6f;

    // Componentes
    private AudioSource audioSourceSFX;
    private AudioSource audioSourcePassos;
    private Rigidbody2D rb;

    // Variáveis de controle
    private float inputH;
    private bool estaNoChao;
    private bool estaAtacando;
    private float tempoAtaque = 0.5f;
    private float cronometroAtaque;
    private int pulosRestantes;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaAtual = vidaMaxima; // Garante que começa cheio

        // 1. Canal de Efeitos
        audioSourceSFX = GetComponent<AudioSource>();
        if (audioSourceSFX == null) audioSourceSFX = gameObject.AddComponent<AudioSource>();

        // 2. Canal de Passos
        audioSourcePassos = gameObject.AddComponent<AudioSource>();
        audioSourcePassos.clip = somAndar;
        audioSourcePassos.loop = true;
        audioSourcePassos.playOnAwake = false;
        audioSourcePassos.volume = 0f;

        pulosRestantes = pulosExtrasMax;

        // Atualiza a UI no começo do jogo
        AtualizarInterface();
    }

    void Update()
    {
        estaNoChao = Physics2D.OverlapCircle(verificadorChao.position, raioVerificacao, camadaChao);

        // Verifica chão e reseta pulos
        // NOTA: Se usar Unity antiga e der erro no linearVelocity, mude para rb.velocity
        if (estaNoChao && Mathf.Abs(rb.linearVelocity.y) <= 0.1f)
        {
            pulosRestantes = pulosExtrasMax;
        }

        if (estaAtacando)
        {
            ProcessarAtaque();
        }
        else
        {
            ProcessarMovimento();
        }

        GerenciarObjetosAnimacao();
    }

    void ProcessarMovimento()
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

        // Virar o personagem
        if (inputH > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (inputH < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Som de passos
        bool estaAndando = estaNoChao && Mathf.Abs(inputH) > 0.1f;
        ControlarVolumePassos(estaAndando);
    }

    void ProcessarAtaque()
    {
        cronometroAtaque -= Time.deltaTime;
        if (cronometroAtaque <= 0) estaAtacando = false;

        // Para o player enquanto ataca
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        ControlarVolumePassos(false);
    }

    void ControlarVolumePassos(bool deveTocar)
    {
        float alvo = deveTocar ? volPassos : 0f;

        if (deveTocar && !audioSourcePassos.isPlaying)
            audioSourcePassos.Play();

        audioSourcePassos.volume = Mathf.Lerp(audioSourcePassos.volume, alvo, Time.deltaTime * velocidadeFade);

        if (!deveTocar && audioSourcePassos.volume < 0.01f)
            audioSourcePassos.Pause();
    }

    void Pular()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);

        if (somPular != null) audioSourceSFX.PlayOneShot(somPular, volPulo);

        SetAnimacaoAtiva(objetoPular);
        TocarAnimacaoDragonBones(objetoPular, 1);
    }

    // --- SISTEMA DE VIDA E CURA (NOVO) ---

    public void Curar(int quantidade)
    {
        if (vidaAtual < vidaMaxima)
        {
            vidaAtual += quantidade;
            if (vidaAtual > vidaMaxima) vidaAtual = vidaMaxima;

            Debug.Log("Player Curado! Vida atual: " + vidaAtual);
            AtualizarInterface();
        }
    }

    public void ReceberDano(int dano = 1)
    {
        if (invencivel) return;

        vidaAtual -= dano;
        Debug.Log("Player tomou dano! Vida restante: " + vidaAtual);

        if (somDano != null) audioSourceSFX.PlayOneShot(somDano, volDano);

        AtualizarInterface();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void AtualizarInterface()
    {
        // AQUI VOCÊ CONECTA SUA UI
        // Exemplo: Se tiver um script "BarraDeVida", procure ele e atualize.

        // GameObject uiVida = GameObject.Find("BarraVida");
        // if (uiVida != null) uiVida.SendMessage("AtualizarVida", vidaAtual, SendMessageOptions.DontRequireReceiver);
    }

    void Morrer()
    {
        Debug.Log("GAME OVER");
        // Coloque aqui a lógica de reiniciar fase ou mostrar tela de fim
        this.enabled = false; // Desativa o player
    }

    // -------------------------------------

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

        if (somAtaqueLanterna != null)
        {
            audioSourceSFX.PlayOneShot(somAtaqueLanterna, volAtaque);
        }

        SetAnimacaoAtiva(objetoAtacar);
        TocarAnimacaoDragonBones(objetoAtacar, 1);
    }

    void GerenciarObjetosAnimacao()
    {
        if (estaAtacando) return; // Se atacando, não muda nada

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
        if (objetoOcioso.activeSelf != (objetoOcioso == ativo)) objetoOcioso.SetActive(objetoOcioso == ativo);
        if (objetoAndando.activeSelf != (objetoAndando == ativo)) objetoAndando.SetActive(objetoAndando == ativo);
        if (objetoPular.activeSelf != (objetoPular == ativo)) objetoPular.SetActive(objetoPular == ativo);
        if (objetoAtacar.activeSelf != (objetoAtacar == ativo)) objetoAtacar.SetActive(objetoAtacar == ativo);
    }

    void TocarAnimacaoDragonBones(GameObject obj, int loops)
    {
        UnityArmatureComponent armature = obj.GetComponent<UnityArmatureComponent>();
        if (armature != null && armature.animation.animationNames.Count > 0)
        {
            // Verifica se a animação já não está tocando para não reiniciar
            if (!armature.animation.isPlaying)
                armature.animation.Play(armature.animation.animationNames[0], loops);
        }
    }
}
using UnityEngine;
using DragonBones;
using System.Collections; // Necessário para Coroutines
using Transform = UnityEngine.Transform;

public class Inimigo : MonoBehaviour
{
    [Header("CONFIGURAÇÃO VISUAL")]
    public UnityArmatureComponent visualVivo;
    public UnityArmatureComponent visualMorto;

    [Header("STATUS")]
    public int vida = 3;
    public int danoNoPlayer = 1;

    [Header("MOVIMENTO")]
    public float velocidade = 2f;
    public float distanciaPatrulha = 5f;

    [Header("CONFIGURAÇÃO DOS STINGERS (Sons)")]
    // --- STINGER ALERTA (Quando vê o player) ---
    public AudioClip somAlerta;
    [Range(0f, 1f)] public float volumeAlerta = 1f;
    public float duracaoStingerAlerta = 2.0f; // Quanto tempo dura o som total

    // --- STINGER MORTE ---
    public AudioClip somMorte;
    [Range(0f, 1f)] public float volumeMorte = 1f;
    public float duracaoStingerMorte = 2.0f; // Quanto tempo dura o som total

    [Header("AJUSTES DE ÁUDIO")]
    [Tooltip("Distância para o inimigo notar o player e tocar o alerta")]
    public float distanciaParaSom = 60f;
    [Tooltip("Tempo final do áudio usado para baixar o volume suavemente")]
    public float tempoDeFadeOut = 0.5f; // O finalzinho suave

    private AudioSource audioSource;
    private Transform playerTransform;
    private bool jaAlertou = false;

    private Vector3 posicaoInicial;
    private bool indoParaDireita = true;
    private bool estaMorto = false;

    void Start()
    {
        posicaoInicial = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        if (visualVivo != null)
        {
            visualVivo.gameObject.SetActive(true);
            if (visualVivo.animation.animationNames.Count > 0)
                visualVivo.animation.Play(visualVivo.animation.animationNames[0], 0);
        }

        if (visualMorto != null) visualMorto.gameObject.SetActive(false);
    }

    void Update()
    {
        if (estaMorto) return;

        VerificarProximidadePlayer();

        // Lógica de Patrulha
        float limiteDireita = posicaoInicial.x + distanciaPatrulha;
        float limiteEsquerda = posicaoInicial.x;

        if (indoParaDireita)
        {
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
            if (transform.position.x >= limiteDireita) indoParaDireita = false;
        }
        else
        {
            transform.Translate(Vector2.left * velocidade * Time.deltaTime);
            if (transform.position.x <= limiteEsquerda) indoParaDireita = true;
        }
    }

    void VerificarProximidadePlayer()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        if (playerTransform == null) return;
        if (somAlerta == null || jaAlertou) return;

        float distancia = Vector2.Distance(transform.position, playerTransform.position);

        if (distancia < distanciaParaSom)
        {
            jaAlertou = true;
            // Toca o alerta como Stinger
            if (gameObject.activeInHierarchy)
                StartCoroutine(TocarStinger(somAlerta, volumeAlerta, duracaoStingerAlerta));
        }
    }

    public void ReceberDano(int dano = 1)
    {
        if (estaMorto) return;

        vida -= dano;
        if (vida <= 0) Morrer();
    }

    void Morrer()
    {
        if (estaMorto) return;
        estaMorto = true;

        // Toca a morte como Stinger (interrompe o alerta se estiver tocando)
        if (somMorte != null && gameObject.activeInHierarchy)
        {
            StartCoroutine(TocarStinger(somMorte, volumeMorte, duracaoStingerMorte));
        }

        // Desativa física
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().simulated = false;

        // Troca visual
        if (visualVivo != null) visualVivo.gameObject.SetActive(false);
        if (visualMorto != null)
        {
            visualMorto.gameObject.SetActive(true);
            if (visualMorto.animation.animationNames.Count > 0)
                visualMorto.animation.Play(visualMorto.animation.animationNames[0], 1);
        }

        // --- IMPORTANTE ---
        // Destrói o objeto SÓ DEPOIS que o som acabar para não cortar o stinger.
        // O +0.1f é uma margem de segurança.
        Destroy(gameObject, duracaoStingerMorte + 0.1f);
    }

    // --- SISTEMA DE STINGER (Timbre e Suavidade) ---
    IEnumerator TocarStinger(AudioClip clip, float volumeMax, float duracaoTotal)
    {
        // 1. Prepara o áudio (Para qualquer som anterior)
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volumeMax; // Começa no volume cheio
        audioSource.Play();

        // 2. Mantém o volume cheio (o "Timbre") pela maior parte do tempo
        // Se o stinger dura 2s e o fade é 0.5s, ele toca alto por 1.5s
        float tempoDeEspera = Mathf.Max(0, duracaoTotal - tempoDeFadeOut);
        yield return new WaitForSeconds(tempoDeEspera);

        // 3. Faz o Fade Out no finalzinho para não ter corte seco
        float tempoPassado = 0;
        float volumeInicial = audioSource.volume;

        while (tempoPassado < tempoDeFadeOut)
        {
            tempoPassado += Time.deltaTime;
            // Lerp cria a descida suave
            audioSource.volume = Mathf.Lerp(volumeInicial, 0f, tempoPassado / tempoDeFadeOut);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (estaMorto) return;
        if (colisao.gameObject.CompareTag("Player"))
        {
            colisao.gameObject.SendMessage("Machucar", danoNoPlayer, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (estaMorto) return;
        if (other.CompareTag("Ataque"))
        {
            ReceberDano(1);
        }
    }
}
using UnityEngine;
using DragonBones;
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

    [Header("SONS (Com Volume)")] // --- ATUALIZADO ---
    public AudioClip somMorte;
    [Range(0f, 1f)] public float volumeMorte = 1f; // Barrinha de volume (0 a 1)

    public AudioClip somAlerta;
    [Range(0f, 1f)] public float volumeAlerta = 1f; // Barrinha de volume (0 a 1)

    public float distanciaParaSom = 60f; // Ajustado para 60 base na nossa conversa anterior

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

        // Tenta achar o player logo no começo
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        if (visualVivo != null)
        {
            visualVivo.gameObject.SetActive(true);
            if (visualVivo.animation.animationNames.Count > 0)
            {
                visualVivo.animation.Play(visualVivo.animation.animationNames[0], 0);
            }
        }

        if (visualMorto != null) visualMorto.gameObject.SetActive(false);
    }

    void Update()
    {
        if (estaMorto) return;

        VerificarProximidadePlayer();

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
        // Se não achou no Start, tenta achar agora
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
            // --- ATUALIZADO: Toca com o volume escolhido ---
            audioSource.PlayOneShot(somAlerta, volumeAlerta);
            jaAlertou = true;
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

        if (audioSource != null && somMorte != null)
        {
            // --- ATUALIZADO: Toca com o volume escolhido ---
            audioSource.PlayOneShot(somMorte, volumeMorte);
        }

        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().simulated = false;

        if (visualVivo != null) visualVivo.gameObject.SetActive(false);
        if (visualMorto != null)
        {
            visualMorto.gameObject.SetActive(true);
            if (visualMorto.animation.animationNames.Count > 0)
            {
                visualMorto.animation.Play(visualMorto.animation.animationNames[0], 1);
            }
        }

        Destroy(gameObject, 1.0f);
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
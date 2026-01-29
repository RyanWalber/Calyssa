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

    private Vector3 posicaoInicial;
    private bool indoParaDireita = true;
    private bool estaMorto = false;

    void Start()
    {
        posicaoInicial = transform.position;

        // --- INICIAR ANIMAÇÃO ---
        if (visualVivo != null)
        {
            visualVivo.gameObject.SetActive(true);

            // Toca em loop infinito (0)
            if (visualVivo.animation.animationNames.Count > 0)
            {
                string anim = visualVivo.animation.animationNames[0];
                visualVivo.animation.Play(anim, 0);
            }

            // REMOVI A LINHA QUE FORÇAVA ESCALA (1,1,1)
            // Agora o Unity vai manter o "2" que você colocou no Inspector.
        }

        if (visualMorto != null) visualMorto.gameObject.SetActive(false);
    }

    void Update()
    {
        if (estaMorto) return;

        float limiteDireita = posicaoInicial.x + distanciaPatrulha;
        float limiteEsquerda = posicaoInicial.x;

        // Movimento simples de vai e vem sem virar (Flip)
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

    public void ReceberDano(int dano = 1)
    {
        if (estaMorto) return;
        vida -= dano;
        if (vida <= 0) Morrer();
    }

    void Morrer()
    {
        estaMorto = true;
        GetComponent<Collider2D>().enabled = false;
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().simulated = false;

        if (visualVivo != null) visualVivo.gameObject.SetActive(false);
        if (visualMorto != null)
        {
            visualMorto.gameObject.SetActive(true);
            // Garante que a explosão também use a escala correta se necessário
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
}
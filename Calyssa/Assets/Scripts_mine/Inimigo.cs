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

        if (visualVivo != null)
        {
            visualVivo.gameObject.SetActive(true);

            if (visualVivo.animation.animationNames.Count > 0)
            {
                string anim = visualVivo.animation.animationNames[0];
                visualVivo.animation.Play(anim, 0);
            }

        }

        if (visualMorto != null) visualMorto.gameObject.SetActive(false);
    }

    void Update()
    {
        if (estaMorto) return;

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

    public void ReceberDano(int dano = 1)
    {
        if (estaMorto) return;

        vida -= dano;
        // Debug para você ver no console se tomou dano
        Debug.Log("Inimigo tomou dano! Vida atual: " + vida);

        if (vida <= 0) Morrer();
    }

    void Morrer()
    {
        if (estaMorto) return; // Garante que não morre duas vezes

        estaMorto = true;

        // Desativa colisão para não machucar mais o player
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().simulated = false;

        // Troca visual vivo pelo morto
        if (visualVivo != null) visualVivo.gameObject.SetActive(false);
        if (visualMorto != null)
        {
            visualMorto.gameObject.SetActive(true);
            if (visualMorto.animation.animationNames.Count > 0)
            {
                visualMorto.animation.Play(visualMorto.animation.animationNames[0], 1);
            }
        }

        // Destrói o objeto após 1 segundo (tempo da animação)
        Destroy(gameObject, 1.0f);
    }

    // Colisão Física (Player encostando no inimigo)
    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (estaMorto) return;
        if (colisao.gameObject.CompareTag("Player"))
        {
            colisao.gameObject.SendMessage("Machucar", danoNoPlayer, SendMessageOptions.DontRequireReceiver);
        }
    }

    // --- PARTE NOVA ---
    // Colisão de Gatilho (Ataque/Luz encostando no inimigo)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (estaMorto) return;

        // Verifica se foi o Ataque que encostou
        // LEMBRE-SE: A Tag do objeto da luz tem que ser "Ataque" (sem aspas)
        if (other.CompareTag("Ataque"))
        {
            ReceberDano(1);
        }
    }
}
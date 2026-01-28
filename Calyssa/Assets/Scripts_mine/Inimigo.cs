using UnityEngine;
using DragonBones; 
using Transform = UnityEngine.Transform; 

public class Inimigo : MonoBehaviour
{
    [Header("Status")]
    public int vidaMaxima = 3;
    private int vidaAtual;
    private bool estaMorto = false; 

    [Header("Configuração DragonBones")]
    public string animacaoAndar = "animtion0"; 
    public string animacaoMorte = "death"; // Verifique o nome no DragonBones!

    [Header("Movimento")]
    public float velocidade = 2f;
    public float distanciaPatrulha = 3f;
    public float raioDeteccao = 5f;
    
    private Vector3 posicaoInicial;
    private bool indoParaDireita = true;
    private Transform player;
    private UnityArmatureComponent armatureComponent;

    void Start()
    {
        vidaAtual = vidaMaxima;
        posicaoInicial = transform.position;
        armatureComponent = GetComponentInChildren<UnityArmatureComponent>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (estaMorto || player == null || armatureComponent == null) return;

        float distanciaParaPlayer = Vector2.Distance(transform.position, player.position);

        if (distanciaParaPlayer < raioDeteccao)
        {
            Perseguir();
        }
        else
        {
            Patrulhar();
        }
    }

    void Patrulhar()
    {
        float limiteDireita = posicaoInicial.x + distanciaPatrulha;
        float limiteEsquerda = posicaoInicial.x - distanciaPatrulha;

        if (indoParaDireita)
        {
            MoverPara(new Vector2(limiteDireita, transform.position.y));
            if (transform.position.x >= limiteDireita - 0.1f) indoParaDireita = false;
        }
        else
        {
            MoverPara(new Vector2(limiteEsquerda, transform.position.y));
            if (transform.position.x <= limiteEsquerda + 0.1f) indoParaDireita = true;
        }
    }

    void Perseguir()
    {
        MoverPara(new Vector2(player.position.x, transform.position.y));
    }

    void MoverPara(Vector2 destino)
    {
        transform.position = Vector2.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);

        if (armatureComponent.animation.lastAnimationName != animacaoAndar)
        {
            armatureComponent.animation.Play(animacaoAndar);
        }

        if (destino.x > transform.position.x) 
            armatureComponent.armature.flipX = false; 
        else if (destino.x < transform.position.x) 
            armatureComponent.armature.flipX = true; 
    }

    public void ReceberDano(int dano)
    {
        if (estaMorto) return;
        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        estaMorto = true;

        // Toca a animação de morte uma única vez (loop = 1)
        if (armatureComponent != null)
        {
            armatureComponent.animation.Play(animacaoMorte, 1);
        }

        // Remove o colisor para o Aster não esbarrar no corpo
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Destrói o objeto após 1.5 segundos (tempo para ver a animação)
        Destroy(gameObject, 1.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
    }
}
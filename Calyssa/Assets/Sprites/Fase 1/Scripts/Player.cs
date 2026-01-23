using UnityEngine;
// Não vamos usar o 'using DragonBones;' aqui para evitar o erro de importação por enquanto

public class Player : MonoBehaviour
{
    [Header("Configurações")]
    public float velocidade = 5f;
    public float forcaPulo = 10f;

    [Header("DragonBones - Arraste o objeto 'Andando' aqui")]
    public UnityEngine.Transform objetoDaAnimacao; // Arraste o objeto 'Andando' para cá
    public string nomeAnimacaoAndar = "animtion0"; // Vi esse nome no seu print!
    public string nomeAnimacaoParado = "idle"; // Verifique se o nome é esse mesmo

    [Header("Chão")]
    public Transform verificadorChao;
    public float raioVerificacao = 0.2f;
    public LayerMask camadaChao;

    private Rigidbody2D rb;
    private float inputH;
    private bool noChao;

    // Variável interna para guardar o componente do DragonBones
    private DragonBones.UnityArmatureComponent armature;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Tenta achar o componente dentro do objeto que você arrastou
        if (objetoDaAnimacao != null)
        {
            armature = objetoDaAnimacao.GetComponent<DragonBones.UnityArmatureComponent>();
        }
    }

    void Update()
    {
        inputH = Input.GetAxisRaw("Horizontal");

        // Pulo
        if (Input.GetButtonDown("Jump") && noChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }

        // Virar o personagem (Flip)
        if (inputH != 0)
        {
            // Vira o PAI (Aster) inteiro
            transform.localScale = new Vector3(inputH > 0 ? 1 : -1, 1, 1);
        }

        ControlarAnimacao();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(inputH * velocidade, rb.linearVelocity.y);
        noChao = Physics2D.OverlapCircle(verificadorChao.position, raioVerificacao, camadaChao);
    }

    void ControlarAnimacao()
    {
        if (armature == null) return; // Se não achou a animação, não faz nada

        if (inputH != 0 && noChao)
        {
            // Se o nome da animação atual for diferente de "animtion0", toca ela
            if (armature.animation.lastAnimationName != nomeAnimacaoAndar)
                armature.animation.Play(nomeAnimacaoAndar);
        }
        else if (noChao)
        {
            if (armature.animation.lastAnimationName != nomeAnimacaoParado)
                armature.animation.Play(nomeAnimacaoParado);
        }
    }
}
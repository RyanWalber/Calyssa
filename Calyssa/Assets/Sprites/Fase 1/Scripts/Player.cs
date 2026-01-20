using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 10f;

    [Header("Verificação de Chão")]
    public Transform verificadorChao; // Um objeto vazio nos pés do personagem
    public float raioVerificacao = 0.2f;
    public LayerMask camadaChao; // O que é considerado chão

    private Rigidbody2D rb;
    private float inputHorizontal;
    private bool estaNoChao;
    private bool olhandoDireita = true;

    void Start()
    {
        // Pega a referência do Rigidbody 2D automaticamente
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Recebe o Input (Teclas A/D ou Setas)
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        // 2. Verifica o Pulo (Tecla Espaço)
        if (Input.GetButtonDown("Jump") && estaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }

        // 3. Vira o personagem
        VirarPersonagem();
    }

    void FixedUpdate()
    {
        // 4. Aplica a física de movimento
        rb.linearVelocity = new Vector2(inputHorizontal * velocidade, rb.linearVelocity.y);

        // 5. Verifica se está tocando no chão
        estaNoChao = Physics2D.OverlapCircle(verificadorChao.position, raioVerificacao, camadaChao);
    }

    void VirarPersonagem()
    {
        if ((inputHorizontal > 0 && !olhandoDireita) || (inputHorizontal < 0 && olhandoDireita))
        {
            olhandoDireita = !olhandoDireita;
            Vector3 escala = transform.localScale;
            escala.x *= -1; // Inverte o sprite horizontalmente
            transform.localScale = escala;
        }
    }

    // Desenha o círculo de verificação no editor para ajudar a visualizar
    void OnDrawGizmosSelected()
    {
        if (verificadorChao != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(verificadorChao.position, raioVerificacao);
        }
    }
}
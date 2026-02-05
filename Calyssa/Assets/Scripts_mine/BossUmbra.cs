using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossUmbra : MonoBehaviour
{
    [Header("CONFIGURAÇÃO OBRIGATÓRIA")]
    public Transform player;
    [Tooltip("Arraste os objetos vazios (Gizmos) aqui")]
    public Transform[] pontosDeTeleporte;

    [Header("AJUSTE VISUAL")]
    [Tooltip("Marque isso se o desenho original do seu boss olha para a Esquerda")]
    public bool spriteOriginalOlhaEsquerda = true; 

    [Header("VIDA & UI")]
    public int vidaTotal = 20;
    public Slider barraVida;
    public GameObject portaSaida;
    private int vidaAtual;

    [Header("COMBATE")]
    public int danoNoPlayer = 1;
    public float forcaEmpurrao = 30f; 
    
    [Header("VELOCIDADE (MODO FÚRIA)")]
    public float velocidadeInicial = 10f; // Velocidade quando está calmo
    public float velocidadeFinal = 22f;   // Velocidade quando está quase morrendo
    
    // VARIÁVEIS INTERNAS
    private float velocidadeAtual;
    private bool podeMover = false; 
    private bool lutaComecou = false;
    
    private SpriteRenderer[] visuais;
    private Collider2D colisorBoss;
    private Rigidbody2D rbBoss;

    void Start()
    {
        vidaAtual = vidaTotal;
        velocidadeAtual = velocidadeInicial;

        visuais = GetComponentsInChildren<SpriteRenderer>();
        colisorBoss = GetComponent<Collider2D>();
        rbBoss = GetComponent<Rigidbody2D>();

        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (barraVida) 
        {
            barraVida.maxValue = vidaTotal;
            barraVida.value = vidaAtual;
            barraVida.gameObject.SetActive(false); 
        }

        if (rbBoss) rbBoss.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void AcordarBoss(float tempoEspera)
    {
        if (!lutaComecou) StartCoroutine(InicioBatalha(tempoEspera));
    }

    IEnumerator InicioBatalha(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (barraVida) barraVida.gameObject.SetActive(true);
        lutaComecou = true;
        podeMover = true;
    }

    void Update()
    {
        if (!lutaComecou || !podeMover || player == null) return;

        // 1. CALCULA VELOCIDADE BASEADA NA VIDA (QUANTO MENOS VIDA, MAIS RÁPIDO)
        float porcentagemVida = (float)vidaAtual / (float)vidaTotal;
        // Lerp faz a transição suave entre a velocidade máxima e mínima
        velocidadeAtual = Mathf.Lerp(velocidadeFinal, velocidadeInicial, porcentagemVida);

        // 2. MOVIMENTO
        transform.position = Vector3.MoveTowards(transform.position, player.position, velocidadeAtual * Time.deltaTime);

        // 3. ROTAÇÃO (CORRIGIDA)
        OlharParaPlayer();
    }

    void OlharParaPlayer()
    {
        // Se o player está à direita (x maior que boss)
        bool playerEstaNaDireita = player.position.x > transform.position.x;

        // Lógica para inverter corretamente dependendo de como o desenho foi feito
        float escalaX = 1;

        if (playerEstaNaDireita)
        {
            // Se o player ta na direita, e o sprite olha pra esquerda, temos que inverter (-1)
            escalaX = spriteOriginalOlhaEsquerda ? -1 : 1;
        }
        else
        {
            // Se o player ta na esquerda
            escalaX = spriteOriginalOlhaEsquerda ? 1 : -1;
        }

        transform.localScale = new Vector3(escalaX, 1, 1);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!podeMover) return;

        if (col.gameObject.CompareTag("Player"))
        {
            podeMover = false; 
            if(rbBoss) rbBoss.linearVelocity = Vector2.zero; // Unity 6

            AplicarEfeitoNoPlayer(col.gameObject);
            StartCoroutine(RotinaTeleporte());
        }
    }

    void AplicarEfeitoNoPlayer(GameObject p)
    {
        p.GetComponent<VidaPlayer>()?.Machucar(danoNoPlayer);

        Rigidbody2D rbPlayer = p.GetComponent<Rigidbody2D>();
        Player scriptPlayer = p.GetComponent<Player>();

        if (rbPlayer)
        {
            if (scriptPlayer) scriptPlayer.enabled = false;
            rbPlayer.linearVelocity = Vector2.zero; 
            
            Vector2 direcao = (p.transform.position - transform.position).normalized;
            direcao += Vector2.up * 0.5f; 
            
            rbPlayer.AddForce(direcao * forcaEmpurrao, ForceMode2D.Impulse);
            StartCoroutine(DestravarPlayer(scriptPlayer));
        }
    }

    IEnumerator DestravarPlayer(Player p)
    {
        yield return new WaitForSeconds(0.4f);
        if (p) p.enabled = true;
    }

    IEnumerator RotinaTeleporte()
    {
        // Some
        if(colisorBoss) colisorBoss.enabled = false;
        foreach(var r in visuais) r.enabled = false;

        // Muda de Lugar (GIZMOS)
        if (pontosDeTeleporte.Length > 0)
        {
            // Tenta achar um ponto que não seja muito perto do player pra não dar spawn kill
            int tentativa = Random.Range(0, pontosDeTeleporte.Length);
            transform.position = pontosDeTeleporte[tentativa].position;
        }

        // Tempo "Rindo" invisível (diminui conforme ele fica mais bravo)
        float tempoEspera = (vidaAtual < vidaTotal / 2) ? 0.8f : 1.5f;
        yield return new WaitForSeconds(tempoEspera);

        // Reaparece
        foreach(var r in visuais) r.enabled = true;
        if(colisorBoss) colisorBoss.enabled = true;
        podeMover = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ataque") && podeMover)
        {
            vidaAtual -= 1;
            if (barraVida) barraVida.value = vidaAtual;

            if (vidaAtual <= 0) Morrer();
            else
            {
                podeMover = false;
                StartCoroutine(RotinaTeleporte());
            }
        }
    }

    void Morrer()
    {
        if (portaSaida) portaSaida.SetActive(false);
        Destroy(gameObject);
    }
}
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
    public float velocidadeInicial = 10f;
    public float velocidadeFinal = 22f;

    // VARIÁVEIS INTERNAS
    private float velocidadeAtual;
    private bool podeMover = false;
    private bool lutaComecou = false;

    private SpriteRenderer[] visuais;
    private Collider2D colisorBoss;
    private Rigidbody2D rbBoss;

    // --- MUDANÇA 1: Variável para guardar o tamanho que você colocou no Inspector ---
    private Vector3 escalaOriginal;

    void Start()
    {
        // --- MUDANÇA 1: Salva o tamanho atual (ex: 2, 2, 2) antes de começar ---
        escalaOriginal = transform.localScale;

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

        float porcentagemVida = (float)vidaAtual / (float)vidaTotal;
        velocidadeAtual = Mathf.Lerp(velocidadeFinal, velocidadeInicial, porcentagemVida);

        transform.position = Vector3.MoveTowards(transform.position, player.position, velocidadeAtual * Time.deltaTime);

        OlharParaPlayer();
    }

    void OlharParaPlayer()
    {
        bool playerEstaNaDireita = player.position.x > transform.position.x;
        float direcaoX = 1;

        if (playerEstaNaDireita)
        {
            direcaoX = spriteOriginalOlhaEsquerda ? -1 : 1;
        }
        else
        {
            direcaoX = spriteOriginalOlhaEsquerda ? 1 : -1;
        }

        // --- MUDANÇA 1: Usa a escalaOriginal.x (absoluta) multiplicada pela direção ---
        // Isso mantêm o tamanho 2 (ou o que você definiu) e só muda o lado.
        transform.localScale = new Vector3(direcaoX * Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!podeMover) return;

        if (col.gameObject.CompareTag("Player"))
        {
            podeMover = false;
            if (rbBoss) rbBoss.linearVelocity = Vector2.zero;

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
        if (colisorBoss) colisorBoss.enabled = false;
        foreach (var r in visuais) r.enabled = false;

        // Muda de Lugar
        if (pontosDeTeleporte.Length > 0)
        {
            int tentativa = Random.Range(0, pontosDeTeleporte.Length);
            transform.position = pontosDeTeleporte[tentativa].position;
        }

        // --- MUDANÇA 2: Teleporte Super Rápido ---
        // Removi a espera de 1.5s. Agora é 0.1s só para piscar.
        yield return new WaitForSeconds(0.1f);

        // Reaparece Já
        foreach (var r in visuais) r.enabled = true;
        if (colisorBoss) colisorBoss.enabled = true;
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
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossUmbra : MonoBehaviour
{
    [Header("VIDA & UI")]
    public int vidaTotal = 20;
    public Slider barraVida; // Arraste o Slider aqui
    public GameObject portaSaida; // Ser� preenchido automaticamente pelo script da sala
    private int vidaAtual;

    [Header("COMBATE")]
    public float forcaEmpurrao = 15f;
    public float distanciaTeleporte = 12f;
    
    [Header("VELOCIDADE (F�ria)")]
    public float velocidadeBase = 12f;
    public float velocidadeMaxima = 20f;
    
    private Transform player;
    private bool estaAtacando = false;
    private bool invulneravel = false;
    private bool lutaComecou = false; // O Boss come�a dormindo
    private SpriteRenderer[] visuais;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaAtual = vidaTotal;
        
        // Tenta achar o player caso n�o tenha sido definido
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        visuais = GetComponentsInChildren<SpriteRenderer>();

        // CONFIGURA��O INICIAL: Esconde a barra de vida
        if (barraVida) 
        {
            barraVida.maxValue = vidaTotal;
            barraVida.value = vidaAtual;
            barraVida.gameObject.SetActive(false); 
        }
    }

    // --- FUN��O PARA ACORDAR O BOSS (CHAMADA PELO GATILHO) ---
    public void AcordarBoss(float tempoEspera)
    {
        if (lutaComecou) return;
        StartCoroutine(IniciarLuta(tempoEspera));
    }

    IEnumerator IniciarLuta(float delay)
    {
        // 1. Espera o tempo de suspense
        yield return new WaitForSeconds(delay);

        // 2. Mostra a barra de vida e inicia o ataque
        if (barraVida) barraVida.gameObject.SetActive(true);
        lutaComecou = true;
        
        StartCoroutine(CicloDeBatalha());
    }

    void Update()
    {
        // Se a luta n�o come�ou, ele s� olha (ou fica parado)
        if (lutaComecou)
        {
             OlharParaPlayer();
        }
    }

    void OlharParaPlayer()
    {
        if (player == null) return;
        if (player.position.x > transform.position.x) transform.rotation = Quaternion.Euler(0, 0, 0);
        else transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    IEnumerator CicloDeBatalha()
    {
        while (vidaAtual > 0)
        {
            float porcentagemVida = (float)vidaAtual / vidaTotal;
            float tempoEspera = Mathf.Lerp(0.5f, 2.0f, porcentagemVida);

            yield return new WaitForSeconds(tempoEspera);

            if (!estaAtacando && !invulneravel && lutaComecou)
            {
                TeleportarLonge();
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(AtaqueDash());
            }
        }
    }

    void TeleportarLonge()
    {
        if (player == null) return;
        
        float direcao = Random.value > 0.5f ? 1f : -1f;
        Vector3 alvo = player.position;
        alvo.x += direcao * distanciaTeleporte;
        alvo.y += 1f; 
        transform.position = alvo;
        OlharParaPlayer();
    }

    IEnumerator AtaqueDash()
    {
        if (player == null) yield break;

        estaAtacando = true;
        float velocidadeAtual = Mathf.Lerp(velocidadeMaxima, velocidadeBase, (float)vidaAtual / vidaTotal);
        Vector3 posicaoAlvo = player.position;
        float tempoDesistencia = 2.0f;

        while (Vector3.Distance(transform.position, posicaoAlvo) > 0.5f && tempoDesistencia > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidadeAtual * Time.deltaTime);
            tempoDesistencia -= Time.deltaTime;
            yield return null;
        }
        estaAtacando = false;
    }

    public void TomarDano(int dano)
    {
        // S� toma dano se a luta j� come�ou!
        if (invulneravel || vidaAtual <= 0 || !lutaComecou) return; 

        vidaAtual -= dano;
        if (barraVida) barraVida.value = vidaAtual;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ReacaoDano());
        }
    }

    IEnumerator ReacaoDano()
    {
        invulneravel = true;
        foreach(var r in visuais) r.enabled = false;
        yield return new WaitForSeconds(0.2f);
        TeleportarLonge();
        foreach(var r in visuais) r.enabled = true;
        invulneravel = false;
        StartCoroutine(CicloDeBatalha());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rbPlayer = other.GetComponent<Rigidbody2D>();
            if (rbPlayer != null)
            {
                Vector2 direcaoEmpurrao = (other.transform.position - transform.position).normalized;
                direcaoEmpurrao += Vector2.up * 0.5f; 
                rbPlayer.linearVelocity = Vector2.zero;
                rbPlayer.AddForce(direcaoEmpurrao * forcaEmpurrao, ForceMode2D.Impulse);
            }
        }
        if (other.CompareTag("Ataque"))
        {
            TomarDano(1);
        }
    }

    void Morrer()
    {
        // Abre a porta de sa�da
        if (portaSaida != null) portaSaida.SetActive(false);
        
        // Esconde a barra de vida
        if (barraVida) barraVida.gameObject.SetActive(false);
        
        Destroy(gameObject);
    }
}
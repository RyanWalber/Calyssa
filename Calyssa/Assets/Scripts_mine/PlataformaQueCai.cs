using UnityEngine;
using System.Collections;

public class PlataformaQueCai : MonoBehaviour
{
    [Header("Configura��o")]
    public float tempoParaCair = 0.5f;   // Tempo tremendo antes de cair
    public float tempoParaVoltar = 2f;   // Tempo para renascer l� em cima

    private Rigidbody2D rb;
    private Vector3 posicaoInicial;
    private bool estaCaindo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        posicaoInicial = transform.position; // Guarda onde ela nasceu

        // Garante que ela comece parada no ar
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // S� ativa se for o Player e se a plataforma j� n�o estiver caindo
        if (collision.gameObject.CompareTag("Player") && !estaCaindo)
        {
            // Verifica se o player est� pisando POR CIMA (opcional, mas evita bugs)
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(CairRoutine());
            }
        }
    }

    IEnumerator CairRoutine()
    {
        estaCaindo = true;

        // --- EFEITO DE TREMEDEIRA (SHAKE) ---
        float timer = 0;
        while (timer < tempoParaCair)
        {
            // Move a plataforma aleatoriamente um pouquinho para os lados
            float x = Random.Range(-0.05f, 0.05f);
            transform.position = new Vector3(posicaoInicial.x + x, posicaoInicial.y, posicaoInicial.z);

            timer += Time.deltaTime;
            yield return null; // Espera o pr�ximo frame
        }

        // --- A QUEDA ---
        // Liga a gravidade para ela cair de verdade
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f; // Cai pesado e r�pido

        // --- O RETORNO ---
        yield return new WaitForSeconds(tempoParaVoltar);

        ResetarPlataforma();
    }

    void ResetarPlataforma()
    {
        // Para a f�sica
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Teletransporta de volta para o in�cio
        transform.position = posicaoInicial;
        estaCaindo = false;
    }
}
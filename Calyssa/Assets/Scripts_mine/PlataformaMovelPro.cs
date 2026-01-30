using UnityEngine;

public class PlataformaMovelPro : MonoBehaviour
{
    [Header("Configuração")]
    public Transform pontoDestino;
    public float velocidade = 3f;

    private Vector3 posA;
    private Vector3 posB;
    private Vector3 alvoAtual;
    private Rigidbody2D rb; // Referência para o Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Pega o Rigidbody
        posA = transform.position;

        if (pontoDestino != null)
        {
            posB = pontoDestino.position;
            pontoDestino.parent = null;
        }
        else
        {
            posB = posA;
        }

        alvoAtual = posB;
    }

    // Mudamos para FixedUpdate para lidar com física
    void FixedUpdate()
    {
        if (posA == posB) return;

        // Move o Rigidbody em vez do Transform
        Vector3 novaPosicao = Vector3.MoveTowards(rb.position, alvoAtual, velocidade * Time.fixedDeltaTime);
        rb.MovePosition(novaPosicao);

        if (Vector3.Distance(rb.position, alvoAtual) < 0.1f)
        {
            alvoAtual = (alvoAtual == posA) ? posB : posA;
        }
    }

    private void OnDrawGizmos()
    {
        if (pontoDestino != null && Application.isPlaying == false)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, pontoDestino.position);
            Gizmos.DrawWireSphere(pontoDestino.position, 0.3f);
        }
    }

    // Grudar o Player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
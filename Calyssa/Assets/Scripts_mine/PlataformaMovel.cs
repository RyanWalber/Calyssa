using UnityEngine;

public class PlataformaMovel : MonoBehaviour
{
    public Transform pontoA, pontoB;
    public float velocidade = 2f;
    private Vector3 destino;

    void Start()
    {
        destino = pontoB.position;
    }

    void Update()
    {
        // Move a plataforma
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);

        // Troca o destino ao chegar
        if (Vector3.Distance(transform.position, destino) < 0.1f)
        {
            destino = (destino == pontoA.position) ? pontoB.position : pontoA.position;
        }
    }

    // FAZ O PLAYER GRUDAR NA PLATAFORMA
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    // FAZ O PLAYER SOLTAR AO PULAR
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
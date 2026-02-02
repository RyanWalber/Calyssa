using UnityEngine;
using System.Collections;

public class PlataformaQueCai : MonoBehaviour
{
    [Header("Configura��o")]
    public float tempoParaCair = 0.5f;   
    public float tempoParaVoltar = 2f;   
    private Rigidbody2D rb;
    private Vector3 posicaoInicial;
    private bool estaCaindo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        posicaoInicial = transform.position; 

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !estaCaindo)
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(CairRoutine());
            }
        }
    }

    IEnumerator CairRoutine()
    {
        estaCaindo = true;

        float timer = 0;
        while (timer < tempoParaCair)
        {
            float x = Random.Range(-0.05f, 0.05f);
            transform.position = new Vector3(posicaoInicial.x + x, posicaoInicial.y, posicaoInicial.z);

            timer += Time.deltaTime;
            yield return null;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f; 

        yield return new WaitForSeconds(tempoParaVoltar);

        ResetarPlataforma();
    }

    void ResetarPlataforma()
    {

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        transform.position = posicaoInicial;
        estaCaindo = false;
    }
}
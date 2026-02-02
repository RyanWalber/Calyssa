using UnityEngine;

public class ItemVida : MonoBehaviour
{
    public int valorCura = 1;
    public GameObject efeitoParticula; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            VidaPlayer scriptVida = collision.GetComponent<VidaPlayer>();

            if (scriptVida != null)
            {
                scriptVida.GanharVida(valorCura);

                if (efeitoParticula != null)
                {
                    Instantiate(efeitoParticula, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}
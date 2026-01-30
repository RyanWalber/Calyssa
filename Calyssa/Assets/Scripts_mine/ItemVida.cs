using UnityEngine;

public class ItemVida : MonoBehaviour
{
    public int valorCura = 1;
    public GameObject efeitoParticula; // Opcional: para soltar um brilho ao pegar

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem encostou foi o Player
        if (collision.CompareTag("Player"))
        {
            // Tenta pegar o script de vida que está no Player
            VidaPlayer scriptVida = collision.GetComponent<VidaPlayer>();

            if (scriptVida != null)
            {
                scriptVida.GanharVida(valorCura);

                // Se tiver efeito de brilho, ele cria o efeito antes de sumir
                if (efeitoParticula != null)
                {
                    Instantiate(efeitoParticula, transform.position, Quaternion.identity);
                }

                // Remove o item da cena
                Destroy(gameObject);
            }
        }
    }
}
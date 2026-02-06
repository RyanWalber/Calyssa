using UnityEngine;

public class ItemVida : MonoBehaviour
{
    [Header("CONFIGURAÇÃO")]
    public int valorCura = 1;
    public GameObject efeitoParticula;

    [Header("AUDIO")]
    public AudioClip somColetar; // <--- TEM QUE TER O ARQUIVO AQUI
    [Range(0f, 1f)] public float volume = 1f; // <--- CONFIRA SE ISSO NÃO ESTÁ NO ZERO

    private bool jaPegou = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (jaPegou) return;

        // Procura o script de vida
        VidaPlayer scriptVida = other.GetComponentInParent<VidaPlayer>();

        if (scriptVida == null && other.CompareTag("Player"))
        {
            scriptVida = FindObjectOfType<VidaPlayer>();
        }

        if (scriptVida != null)
        {
            jaPegou = true;
            scriptVida.GanharVida(valorCura);

            // --- A CORREÇÃO DO SOM ESTÁ AQUI ---
            if (somColetar != null)
            {
                // Truque: Toca o som EXATAMENTE onde a câmera está.
                // Isso impede que o som fique "baixo" ou "longe" no mundo 3D.
                Vector3 posicaoDoOuvido = Camera.main.transform.position;
                AudioSource.PlayClipAtPoint(somColetar, posicaoDoOuvido, volume);
            }
            else
            {
                Debug.LogError("ERRO: Você esqueceu de arrastar o som para o Inspector do ItemVida!");
            }

            if (efeitoParticula != null)
            {
                Instantiate(efeitoParticula, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
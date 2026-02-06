using UnityEngine;

public class ItemVida : MonoBehaviour
{
    [Header("CONFIGURAÇÕES")]
    public int valorCura = 1;
    public GameObject efeitoParticula;

    [Header("AUDIO")]
    public AudioClip somColetar;
    [Range(0f, 1f)] public float volume = 1f;

    private AudioSource audioSource;
    private bool jaColetou = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player") && !jaColetou)
        {
            jaColetou = true;

 
            other.SendMessage("Curar", valorCura, SendMessageOptions.DontRequireReceiver);

            
            if (efeitoParticula != null)
            {
                Instantiate(efeitoParticula, transform.position, Quaternion.identity);
            }

            if (somColetar != null)
            {
                audioSource.PlayOneShot(somColetar, volume);
            }
            else if (audioSource.clip != null)
            {

                audioSource.PlayOneShot(audioSource.clip, volume);
            }

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;


            Destroy(gameObject, 1.0f);
        }
    }
}
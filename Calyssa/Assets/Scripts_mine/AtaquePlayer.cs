using UnityEngine;

public class AtaquePlayer : MonoBehaviour
{
    public int danoDoAtaque = 1;

    [Header("Sons")]
    public AudioClip somAtaque; // Arraste seu som aqui no Inspector
    private AudioSource audioSrc;

    void Awake()
    {
        // Pega o componente de áudio do objeto
        audioSrc = GetComponent<AudioSource>();
    }

    // Toda vez que a luz do ataque for ativada (o "golpe")
    void OnEnable()
    {
        if (audioSrc != null && somAtaque != null)
        {
            audioSrc.PlayOneShot(somAtaque);
        }
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        // Tenta pegar o script Inimigo ou o BossUmbra
        Inimigo scriptInimigo = outro.GetComponent<Inimigo>();

        // Se o seu Boss não usa o script "Inimigo", o Trigger do Boss já trata o dano,
        // mas deixaremos este log para você saber que a colisão aconteceu.
        if (scriptInimigo != null)
        {
            scriptInimigo.ReceberDano(danoDoAtaque);
            Debug.Log("Acertei o inimigo!");
        }
    }
}
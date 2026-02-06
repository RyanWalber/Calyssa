using UnityEngine;
using System.Collections;

public class GatilhoSequenciaBoss : MonoBehaviour
{
    [Header("Áudios")]
    public AudioSource audioAmeaca; // Som da fala/ameaça
    public AudioSource audioMusicaBoss; // Música que começa depois
    public AudioSource audioAmbienteFase; // Música que para agora

    private bool jaAtivou = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !jaAtivou)
        {
            jaAtivou = true;
            StartCoroutine(SequenciaBoss());
        }
    }

    IEnumerator SequenciaBoss()
    {
        // 1. Para a música da fase imediatamente
        if (audioAmbienteFase != null) audioAmbienteFase.Stop();

        // 2. Toca a ameaça
        if (audioAmeaca != null)
        {
            audioAmeaca.Play();

            // ESPERA o tempo exato do áudio de ameaça acabar
            yield return new WaitForSeconds(audioAmeaca.clip.length);
        }

        // 3. Agora sim, começa a música da luta
        if (audioMusicaBoss != null)
        {
            audioMusicaBoss.Play();
        }

        // 4. Opcional: Destruir o gatilho ou desativar o objeto da ameaça
        // Destroy(audioAmeaca); 
    }
}
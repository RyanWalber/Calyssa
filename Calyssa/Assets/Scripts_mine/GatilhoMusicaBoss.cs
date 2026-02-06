using UnityEngine;
using System.Collections;

public class GatilhoSequenciaBoss : MonoBehaviour
{
    [Header("Áudios Necessários")]
    public AudioSource audioAmeaca;       // Grito (Vai sumir no final)
    public AudioSource audioMusicaBoss;   // Música (Vai surgir no final)
    public AudioSource audioAmbienteFase; // Fundo (Continua tocando)

    [Header("Configuração da Mistura")]
    [Tooltip("Quantos segundos finais do grito serão usados para a troca?")]
    public float tempoDeCrossfade = 2.0f; // Tempo que os dois tocam juntos trocando de volume

    [Tooltip("Volume do som da caverna durante a luta (0 a 1)")]
    [Range(0f, 1f)] public float volumeAmbienteNaLuta = 0.5f;

    private bool jaAtivou = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !jaAtivou)
        {
            jaAtivou = true;
            StartCoroutine(SequenciaCrossfade());
        }
    }

    IEnumerator SequenciaCrossfade()
    {
        // 1. COMEÇA O GRITO NO MÁXIMO
        float duracaoGrito = 0f;
        if (audioAmeaca != null)
        {
            audioAmeaca.volume = 1f; // Garante volume cheio
            audioAmeaca.Play();
            duracaoGrito = audioAmeaca.clip.length;
        }

        // 2. AJUSTA O AMBIENTE (Não para, só abaixa um pouco para não embolar)
        if (audioAmbienteFase != null)
        {
            StartCoroutine(MudarVolume(audioAmbienteFase, volumeAmbienteNaLuta, 2.0f));
        }

        // 3. CALCULA O TEMPO DE ESPERA
        // Se o grito tem 5s e o crossfade é 2s, ele espera 3s antes de começar a troca.
        // O Mathf.Max(0, ...) impede erro se o grito for muito curto.
        float tempoDeEspera = Mathf.Max(0, duracaoGrito - tempoDeCrossfade);

        yield return new WaitForSeconds(tempoDeEspera);

        // --- A MÁGICA DO CROSSFADE (Tudo acontece junto agora) ---

        // 4. DIMINUI O GRITO (Fade Out -> vai para 0)
        if (audioAmeaca != null)
        {
            StartCoroutine(MudarVolume(audioAmeaca, 0f, tempoDeCrossfade));
        }

        // 5. AUMENTA A MÚSICA (Fade In -> vai para 1)
        if (audioMusicaBoss != null)
        {
            audioMusicaBoss.volume = 0f; // Começa mudo
            audioMusicaBoss.Play();      // Dá o play
            StartCoroutine(MudarVolume(audioMusicaBoss, 1f, tempoDeCrossfade));
        }
    }

    // Função auxiliar que faz o volume deslizar suavemente
    IEnumerator MudarVolume(AudioSource audio, float volumeAlvo, float duracao)
    {
        float tempoPassado = 0;
        float volumeInicial = audio.volume;

        while (tempoPassado < duracao)
        {
            tempoPassado += Time.deltaTime;
            // Mathf.Lerp faz a transição suave matemática entre os valores
            audio.volume = Mathf.Lerp(volumeInicial, volumeAlvo, tempoPassado / duracao);
            yield return null;
        }
        audio.volume = volumeAlvo; // Garante o valor final exato
    }
}
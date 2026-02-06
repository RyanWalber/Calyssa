using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necessário para controlar a Imagem
using System.Collections;

public class MenuTransition : MonoBehaviour
{
    [Header("CONFIGURAÇÕES")]
    public string nomeCenaDoJogo = "NomeDaSuaCena"; // Troque pelo nome exato
    public float tempoDeFade = 1.5f;

    [Header("REFERÊNCIAS")]
    public AudioClip musicaMenu;
    public Image painelPreto; // Arraste a imagem preta aqui

    private AudioSource audioSource;
    private bool jaClicou = false;

    void Start()
    {
        // Configura áudio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicaMenu;
        audioSource.loop = true;
        audioSource.volume = 1f;
        audioSource.Play();

        // Garante que a tela preta comece transparente e invisível
        if (painelPreto != null)
        {
            painelPreto.gameObject.SetActive(false); // Desativa para não atrapalhar
            Color c = painelPreto.color;
            c.a = 0f; // Alpha 0 (Transparente)
            painelPreto.color = c;
        }
    }

    public void JogarComFade()
    {
        if (!jaClicou && painelPreto != null)
        {
            jaClicou = true;
            StartCoroutine(FadeOutTotal());
        }
    }

    IEnumerator FadeOutTotal()
    {
        // Ativa a imagem preta (ela ainda está transparente)
        painelPreto.gameObject.SetActive(true);

        float tempo = 0;
        float volInicial = audioSource.volume;

        while (tempo < tempoDeFade)
        {
            tempo += Time.deltaTime;
            float progresso = tempo / tempoDeFade;

            // 1. Abaixa o som
            audioSource.volume = Mathf.Lerp(volInicial, 0, progresso);

            // 2. Aumenta a opacidade da tela preta (de 0 a 1)
            Color c = painelPreto.color;
            c.a = Mathf.Lerp(0, 1, progresso);
            painelPreto.color = c;

            yield return null;
        }

        // Garante que ficou tudo preto e mudo
        audioSource.volume = 0;
        Color finalColor = painelPreto.color;
        finalColor.a = 1f;
        painelPreto.color = finalColor;

        // Carrega a cena
        SceneManager.LoadScene(nomeCenaDoJogo);
    }
}
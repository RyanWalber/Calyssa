using UnityEngine;
using System.Collections;

public class Limbo : MonoBehaviour
{
    [Header("Configuração")]
    public float tempoDeQuedaNoVazio = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SequenciaDeMorte(collision.gameObject));
        }
    }

    IEnumerator SequenciaDeMorte(GameObject player)
    {
        // 1. Tira o controle do Player (para ele cair reto)
        var scriptMovimento = player.GetComponent<MonoBehaviour>();
        if (scriptMovimento != null) scriptMovimento.enabled = false;

        // 2. Deixa transparente (efeito visual)
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = new Color(1, 1, 1, 0.5f);

        // --- A SOLUÇÃO "HACKER" ---
        // Procura todos os scripts ativos na cena
        MonoBehaviour[] todosScripts = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour script in todosScripts)
        {
            // Se o script tiver "Confiner" no nome (CinemachineConfiner ou CinemachineConfiner2D)
            if (script.GetType().Name.Contains("Confiner"))
            {
                script.enabled = false; // DESLIGA NA FORÇA!
                Debug.Log("Achei o limitador e desliguei: " + script.name);
            }
        }
        // -------------------------

        Debug.Log("Caindo no infinito...");

        // 3. Espera a queda dramática
        yield return new WaitForSeconds(tempoDeQuedaNoVazio);

        // 4. Mata o player e reinicia
        VidaPlayer vida = player.GetComponent<VidaPlayer>();
        if (vida != null)
        {
            vida.Machucar(9999);
        }
    }
}
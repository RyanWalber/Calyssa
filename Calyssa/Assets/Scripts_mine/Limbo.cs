using UnityEngine;
using System.Collections;

public class Limbo : MonoBehaviour
{
    [Header("Configura��o")]
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
        var scriptMovimento = player.GetComponent<MonoBehaviour>();
        if (scriptMovimento != null) scriptMovimento.enabled = false;

        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = new Color(1, 1, 1, 0.5f);

        MonoBehaviour[] todosScripts = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour script in todosScripts)
        {
            if (script.GetType().Name.Contains("Confiner"))
            {
                script.enabled = false;
                Debug.Log("Achei o limitador e desliguei: " + script.name);
            }
        }


        

        yield return new WaitForSeconds(tempoDeQuedaNoVazio);

        VidaPlayer vida = player.GetComponent<VidaPlayer>();
        if (vida != null)
        {
            vida.Machucar(9999);
        }
    }
}
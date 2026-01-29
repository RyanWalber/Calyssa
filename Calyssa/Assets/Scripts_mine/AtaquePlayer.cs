using UnityEngine;

public class AtaquePlayer : MonoBehaviour
{
    public int danoDoAtaque = 1;

    void OnTriggerEnter2D(Collider2D outro)
    {
        // Verifica se o objeto que entrou na luz tem a tag "Inimigo"
        // OU se ele tem o script Inimigo
        Inimigo scriptInimigo = outro.GetComponent<Inimigo>();

        if (scriptInimigo != null)
        {
            // Causa dano no inimigo
            scriptInimigo.ReceberDano(danoDoAtaque);
            Debug.Log("Acertei o inimigo!");
        }
    }
}
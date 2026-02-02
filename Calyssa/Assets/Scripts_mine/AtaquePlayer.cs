using UnityEngine;

public class AtaquePlayer : MonoBehaviour
{
    public int danoDoAtaque = 1;

    void OnTriggerEnter2D(Collider2D outro)
    {
        Inimigo scriptInimigo = outro.GetComponent<Inimigo>();

        if (scriptInimigo != null)
        {
            scriptInimigo.ReceberDano(danoDoAtaque);
            Debug.Log("Acertei o inimigo!");
        }
    }
}
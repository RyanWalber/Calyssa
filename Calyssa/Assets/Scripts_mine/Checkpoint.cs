using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuais")]
    public Color corAtivado = Color.green;
    
    [Header("Arraste aqui o Prefab da Luz ou Partícula")]
    public GameObject luzOuParticula; 

    private bool ativado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !ativado)
        {
            ativado = true;
            
            VidaPlayer.pontoDeRespawn = transform.position;
            VidaPlayer.temCheckpointSalvo = true;

            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = corAtivado;
            }

            if (luzOuParticula != null)
            {
                Instantiate(luzOuParticula, transform.position, Quaternion.identity);
            }
        }
    }
}
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Color corAtivado = Color.green;
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
        }
    }
}
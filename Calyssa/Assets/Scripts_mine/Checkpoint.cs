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
            
            // Salva o progresso
            VidaPlayer.pontoDeRespawn = transform.position;
            VidaPlayer.temCheckpointSalvo = true;

            // 1. Muda a cor do Checkpoint (opcional)
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = corAtivado;
            }

            // 2. Faz aparecer a Luz/Efeito
            if (luzOuParticula != null)
            {
                // Instantiate cria o objeto na cena
                // transform.position = lugar do checkpoint
                // Quaternion.identity = rotação padrão
                Instantiate(luzOuParticula, transform.position, Quaternion.identity);
            }
        }
    }
}
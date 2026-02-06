using UnityEngine;
using System.Collections;

public class SpawnerVida : MonoBehaviour
{
    [Header("O que vai nascer?")]
    public GameObject itemVidaPrefab; // Arraste seu prefab de Vida aqui

    [Header("Onde vai nascer?")]
    public Transform[] pontosDeSpawn; // Vamos colocar os pontos vazios aqui

    [Header("Configuração")]
    public float tempoEntreSpawns = 15f; // A cada quantos segundos?
    public bool podeSpawnar = true;

    void Start()
    {
        // Começa a contagem assim que o jogo inicia
        StartCoroutine(RotinaDeSpawn());
    }

    IEnumerator RotinaDeSpawn()
    {
        // Espera um pouco antes de criar o primeiro (para não ser imediato)
        yield return new WaitForSeconds(5f);

        while (podeSpawnar)
        {
            SpawnarItem();
            // Espera o tempo configurado antes de rodar de novo
            yield return new WaitForSeconds(tempoEntreSpawns);
        }
    }

    void SpawnarItem()
    {
        if (itemVidaPrefab == null || pontosDeSpawn.Length == 0) return;

        // 1. Escolhe um número aleatório (ex: entre 0 e 3)
        int indexAleatorio = Random.Range(0, pontosDeSpawn.Length);

        // 2. Cria o item na posição daquele ponto escolhido
        Instantiate(itemVidaPrefab, pontosDeSpawn[indexAleatorio].position, Quaternion.identity);
    }

    // Função para parar de spawnar (ex: quando o Boss morrer)
    public void PararSpawner()
    {
        podeSpawnar = false;
    }
}
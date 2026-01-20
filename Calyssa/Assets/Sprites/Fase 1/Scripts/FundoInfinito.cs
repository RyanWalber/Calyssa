using UnityEngine;

public class FundoInfinito : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect; // 1 = gruda na camera, 0.5 = move metade, 0 = parado

    void Start()
    {
        startpos = transform.position.x;
        // Pega a largura exata da imagem (Sprite) para saber quando repetir
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        // Calcula a distância que a câmera percorreu em relação ao efeito parallax
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        // Move o fundo
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // A Mágica do Loop: Se a câmera andou mais que o tamanho da imagem, reposiciona o fundo
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
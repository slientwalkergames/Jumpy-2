using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Zemin Ayarları")]
    // Sahnede kaydırılacak zemin parçalarını bu dizinin içine atacağız
    [SerializeField] private Transform[] groundPieces;

    // Zeminin kayma hızı
    [SerializeField] private float moveSpeed = 5f;

    // Bir zemin parçasının X eksenindeki tam uzunluğu (Scale X değerimiz olan 50)
    [SerializeField] private float groundLength = 50f;

    void Update()
    {
        // Dizideki her bir zemin parçasını tek tek kontrol etmek için bir döngü başlatıyoruz
        for (int i = 0; i < groundPieces.Length; i++)
        {
            // 1. Zemin parçasını sola doğru hareket ettir
            groundPieces[i].Translate(Vector3.left * moveSpeed * Time.deltaTime);

            // 2. Kusursuz Döngü Kontrolü: 
            // Eğer zemin, kendi uzunluğu kadar sola gittiyse (yani kameradan tamamen çıktıysa)
            if (groundPieces[i].position.x <= -groundLength)
            {
                // Onu alıp, dizideki tüm zeminlerin toplam uzunluğu kadar sağa (ileri) atıyoruz.
                // Örneğin 2 zemin varsa (2 * 50 = 100). Zemin -50'ye gelince 100 birim ileri atarsak
                // tam +50 noktasına, yani diğer zeminin bittiği yere milimetrik olarak oturur.
                Vector3 newPos = groundPieces[i].position;
                newPos.x += groundPieces.Length * groundLength;

                groundPieces[i].position = newPos;
            }
        }
    }
}
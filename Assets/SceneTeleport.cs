using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : MonoBehaviour
{
    public string sceneName;        // Gidilecek sahne
    public string spawnPointName;   // Gidilecek spawn noktası

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Spawn bilgisini kaydet
            PlayerPrefs.SetString("SpawnPoint", spawnPointName);
            PlayerPrefs.Save();

            // Sahneyi yükle
            SceneManager.LoadScene(sceneName);
        }
    }
}


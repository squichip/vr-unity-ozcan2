using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceEnableOnlyControllers : MonoBehaviour
{
    public GameObject leftController;
    public GameObject rightController;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (leftController != null)
            leftController.SetActive(true);

        if (rightController != null)
            rightController.SetActive(true);
    }
}

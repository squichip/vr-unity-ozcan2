using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableXRControllersOnSceneLoad : MonoBehaviour
{
    [Header("Controller Objects")]
    public GameObject leftController;
    public GameObject rightController;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        EnableControllers();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnableControllers();
    }

    void EnableControllers()
    {
        if (leftController != null)
            leftController.SetActive(true);

        if (rightController != null)
            rightController.SetActive(true);
    }
}

using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [Header("Areas (3D world objects)")]
    public GameObject sportArea1;   // spor alanın (3D objelerin parent'ı)
    // şimdilik tek sahne dediğin için sadece 1 yeter
    // ileride sportArea2, sportArea3 eklersin

    [Header("UI Panels (under camera canvas)")]
    public GameObject menuPanel;    // MenuPanel
    public GameObject sportPanel;   // SportPanel

    void Start()
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        // UI
        if (menuPanel) menuPanel.SetActive(true);
        if (sportPanel) sportPanel.SetActive(false);

        // World
        if (sportArea1) sportArea1.SetActive(false);
    }

    public void ShowSport1()
    {
        // UI
        if (menuPanel) menuPanel.SetActive(false);
        if (sportPanel) sportPanel.SetActive(true);

        // World
        if (sportArea1) sportArea1.SetActive(true);
    }
}

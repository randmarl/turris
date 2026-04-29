using UnityEngine;
using UnityEngine.SceneManagement;

public class PeamenüüHaldur : MonoBehaviour
{
    [Header("UI paneelid")]
    [SerializeField] private GameObject peamenüüPaneel;
    [SerializeField] private GameObject tasemetePaneel;

    [Header("Stseenid")]
    [SerializeField] private string tase1Stseen = "Level1";
    [SerializeField] private string õpetuseStseen = "Õpetus";

    private void Start()
    {
        if (!AvaekraaniNavigatsioon.AvaTasemedKohe)
            return;

        AvaekraaniNavigatsioon.AvaTasemedKohe = false;
        AvaTasemed();
    }

    public void AvaTasemed()
    {
        if (peamenüüPaneel != null)
            peamenüüPaneel.SetActive(false);

        if (tasemetePaneel != null)
            tasemetePaneel.SetActive(true);
    }

    public void TagasiPeamenüüsse()
    {
        if (tasemetePaneel != null)
            tasemetePaneel.SetActive(false);

        if (peamenüüPaneel != null)
            peamenüüPaneel.SetActive(true);
    }

    public void AvaTase1()
    {
        SceneManager.LoadScene(tase1Stseen);
    }

    public void AvaÕpetus()
    {
        SceneManager.LoadScene(õpetuseStseen);
    }

    public void Välju()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class PeamenüüHaldur : MonoBehaviour
{
    [Header("UI paneelid")]
    [SerializeField] private GameObject peamenüüPaneel;
    [SerializeField] private GameObject tasemetePaneel;

    [Header("Stseenid")]
    [SerializeField] private string tase1Stseen = "Level1";
    [SerializeField] private string tase2Stseen = "Level2";
    [SerializeField] private string tase3Stseen = "Level3";
    [SerializeField] private string tase4Stseen = "Level4";
    [SerializeField] private string tase5Stseen = "Level5";
    [SerializeField] private string tase6Stseen = "Level6";
    [SerializeField] private string tase7Stseen = "Level7";
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

    public void AvaTase2()
    {
        SceneManager.LoadScene(tase2Stseen);
    }

    public void AvaTase3()
    {
        SceneManager.LoadScene(tase3Stseen);
    }
    public void AvaTase4()
    {
        SceneManager.LoadScene(tase4Stseen);
    }

    public void AvaTase5()
    {
        SceneManager.LoadScene(tase5Stseen);
    }

    public void AvaTase6()
    {
        SceneManager.LoadScene(tase6Stseen);
    }

    public void AvaTase7()
    {
        SceneManager.LoadScene(tase7Stseen);
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
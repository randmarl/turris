using UnityEngine;
using UnityEngine.SceneManagement;

public class Peamenüühaldur : MonoBehaviour
{
    [Header("UI paneelid")]
    [SerializeField] private GameObject peamenüüPaneel;
    [SerializeField] private GameObject tasemetePaneel;

    [Header("Stseenid")]
    [SerializeField] private string tase1Stseen = "Level1";

    private void Start()
    {
        if (AvaekraaniNavigatsioon.AvaTasemedKohe)
        {
            AvaekraaniNavigatsioon.AvaTasemedKohe = false;
            AvaTasemed();
        }
    }

    public void AvaTasemed()
    {
        peamenüüPaneel.SetActive(false);
        tasemetePaneel.SetActive(true);
    }

    public void TagasiPeamenüüsse()
    {
        tasemetePaneel.SetActive(false);
        peamenüüPaneel.SetActive(true);
    }

    public void AvaTase1()
    {
        SceneManager.LoadScene(tase1Stseen);
    }

    public void AvaOpetus()
    {
        Debug.Log("Õpetus avaneb.");
    }

    public void Välju()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
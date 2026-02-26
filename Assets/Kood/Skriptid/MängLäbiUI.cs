using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MängLäbiUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject mängLäbiObjekt;
    [SerializeField] private TMP_Text mängLäbiTekst;

    [Header("Seaded")]
    [SerializeField] private float ooteAeg = 5f;
    [SerializeField] private string avaekraaniStseen = "avaekraan";
    [SerializeField] private bool peataMäng = true;

    private bool käivitatud = false;

    private void Awake()
    {
        if (mängLäbiObjekt != null)
            mängLäbiObjekt.SetActive(false);
    }

    private void OnEnable()
    {
        if (MängijaElud.Instance != null)
            MängijaElud.Instance.MängLäbi.AddListener(OnMängLäbi);
    }

    private void OnDisable()
    {
        if (MängijaElud.Instance != null)
            MängijaElud.Instance.MängLäbi.RemoveListener(OnMängLäbi);
    }

    private void OnMängLäbi()
    {
        if (käivitatud) return;
        käivitatud = true;

        if (mängLäbiTekst != null)
            mängLäbiTekst.text = "MÄNG LÄBI!";

        if (mängLäbiObjekt != null)
            mängLäbiObjekt.SetActive(true);

        StartCoroutine(TagasiAvaekraanile());
    }

    public void Kaivita()
    {
        OnMängLäbi();
    }

    private IEnumerator TagasiAvaekraanile()
    {
        if (peataMäng) Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(ooteAeg);

        Time.timeScale = 1f;
        SceneManager.LoadScene(avaekraaniStseen);
    }
}
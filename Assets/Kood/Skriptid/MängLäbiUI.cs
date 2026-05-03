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

    private bool käivitatud;

    private void Awake()
    {
        if (mängLäbiObjekt != null)
            mängLäbiObjekt.SetActive(false);
    }

    private void OnEnable()
    {
        ÜhendaMängijaEludega();
    }

    private void Start()
    {
        ÜhendaMängijaEludega();
    }

    private void OnDisable()
    {
        if (MängijaElud.Instance != null)
            MängijaElud.Instance.MängLäbi.RemoveListener(OnMängLäbi);
    }

    private void ÜhendaMängijaEludega()
    {
        if (MängijaElud.Instance == null)
            return;

        MängijaElud.Instance.MängLäbi.RemoveListener(OnMängLäbi);
        MängijaElud.Instance.MängLäbi.AddListener(OnMängLäbi);
    }

    private void OnMängLäbi()
    {
        if (käivitatud)
            return;

        käivitatud = true;

        if (mängLäbiTekst != null)
            mängLäbiTekst.text = "MÄNG LÄBI!";

        if (mängLäbiObjekt != null)
            mängLäbiObjekt.SetActive(true);

        StartCoroutine(TagasiAvaekraanile());
    }

    public void Käivita()
    {
        OnMängLäbi();
    }

    private IEnumerator TagasiAvaekraanile()
    {
        if (peataMäng)
            Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(ooteAeg);

        Time.timeScale = 1f;

        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.LähtestaAlgrahale();

        SceneManager.LoadScene(avaekraaniStseen);
    }
}
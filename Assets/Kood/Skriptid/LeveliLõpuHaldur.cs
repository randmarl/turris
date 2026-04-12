using UnityEngine;
using UnityEngine.SceneManagement;

public class LeveliLõpuHaldur : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private VaenlaseTekitaja vaenlaseTekitaja;
    [SerializeField] private JutustuseHaldur jutustuseHaldur;

    [Header("Lõpujutustus")]
    [SerializeField] private string võiduPealkiri = "Kodutee on vaba";
    [SerializeField] private TextAsset võiduJutustuseFail;

    [Header("Stseen")]
    [SerializeField] private string avaekraaniStseen = "avaekraan";

    private bool võitKäivitunud = false;

    private void OnEnable()
    {
        if (vaenlaseTekitaja != null)
            vaenlaseTekitaja.TaseLäbitud.AddListener(KäivitaVõiduJutustus);
    }

    private void OnDisable()
    {
        if (vaenlaseTekitaja != null)
            vaenlaseTekitaja.TaseLäbitud.RemoveListener(KäivitaVõiduJutustus);

        if (jutustuseHaldur != null)
            jutustuseHaldur.JutustusLõppes.RemoveListener(TagasiAvaekraanile);
    }

    private void KäivitaVõiduJutustus()
    {
        if (võitKäivitunud) return;
        võitKäivitunud = true;

        if (jutustuseHaldur == null)
        {
            Debug.LogError("LevelLõpuHaldur: JutustuseHaldur viide puudub.");
            return;
        }

        if (võiduJutustuseFail == null)
        {
            Debug.LogError("LevelLõpuHaldur: võiduJutustuseFail puudub.");
            return;
        }

        jutustuseHaldur.JutustusLõppes.RemoveListener(TagasiAvaekraanile);
        jutustuseHaldur.JutustusLõppes.AddListener(TagasiAvaekraanile);

        jutustuseHaldur.SeaPealkiri(võiduPealkiri);
        jutustuseHaldur.SeaJutustuseFail(võiduJutustuseFail);

        jutustuseHaldur.enabled = true;
        jutustuseHaldur.gameObject.SetActive(true);
    }

    private void TagasiAvaekraanile()
    {
        if (jutustuseHaldur != null)
            jutustuseHaldur.JutustusLõppes.RemoveListener(TagasiAvaekraanile);

        Time.timeScale = 1f;
        SceneManager.LoadScene(avaekraaniStseen);
    }
}
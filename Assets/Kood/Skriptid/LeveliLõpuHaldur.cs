using UnityEngine;
using UnityEngine.SceneManagement;

public class LeveliLõpuHaldur : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private VaenlaseTekitaja vaenlaseTekitaja;
    [SerializeField] private JutustuseHaldur jutustuseHaldur;

    [Header("Lõpujutustus")]
    [SerializeField] private string võiduPealkiri = "VÕIT!";
    [SerializeField] private TextAsset võiduJutustuseFail;

    [Header("Stseen")]
    [SerializeField] private string avaekraaniStseen = "avaekraan";

    private bool võitKäivitunud;

    private void OnEnable()
    {
        if (vaenlaseTekitaja != null)
            // kui tase läbi, siis võidujutustus
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
        if (võitKäivitunud)
            return;

        // väldib võidu topelt käivitamist
        võitKäivitunud = true;

        if (jutustuseHaldur == null)
        {
            Debug.LogError("LeveliLõpuHaldur: JutustuseHaldur viide puudub.");
            return;
        }

        if (võiduJutustuseFail == null)
        {
            Debug.LogError("LeveliLõpuHaldur: võiduJutustuseFail puudub.");
            return;
        }

        // pärast lõpujutustust tagasi avaekraanile
        jutustuseHaldur.JutustusLõppes.RemoveListener(TagasiAvaekraanile);
        jutustuseHaldur.JutustusLõppes.AddListener(TagasiAvaekraanile);

        jutustuseHaldur.SeaPealkiri(võiduPealkiri);
        jutustuseHaldur.SeaJutustuseFail(võiduJutustuseFail);

        // käivitab jutustuse uuesti
        jutustuseHaldur.enabled = true;
        jutustuseHaldur.gameObject.SetActive(true);
    }

    private void TagasiAvaekraanile()
    {
        if (jutustuseHaldur != null)
            jutustuseHaldur.JutustusLõppes.RemoveListener(TagasiAvaekraanile);

        Time.timeScale = 1f;

        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.LähtestaAlgrahale();

        SceneManager.LoadScene(avaekraaniStseen);
    }
}
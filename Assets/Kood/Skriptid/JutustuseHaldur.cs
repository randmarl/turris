using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

public class JutustuseHaldur : MonoBehaviour
{
    [Header("UI viited")]
    [SerializeField] private GameObject jutustusPaneel;
    [SerializeField] private TMP_Text pealkiriTekst;
    [SerializeField] private TMP_Text jutumulliTekst;
    [SerializeField] private GameObject jutumullObjekt;
    [SerializeField] private GameObject tegelaneObjekt;

    [Header("Sätted")]
    [SerializeField] private string pealkiri = "Manivaldi matused";
    [SerializeField] private float pealkiriKestusSek = 2.0f;
    [SerializeField] private TextAsset jutustuseFail;

    [Header("Tekstiefekt")]
    [SerializeField] private bool kasutaTähthaavalEfekti = true;
    [SerializeField] private float täheIlmumiseViivitus = 0.04f;

    [Header("Sündmused")]
    public UnityEvent JutustusLõppes = new UnityEvent();

    private string[] jutustuseRead;
    private int praeguneRidaIndeks = 0;

    private bool onPealkiriFaasis = true;
    private bool kirjutabTeksti = false;

    private Coroutine kirjutamiseCoroutine;
    private float pealkiriOotamiseTaimer = 0f;
    private bool pealkiriOnTäielikultNäidatud = false;

    private string praeguneTäisTekst = "";
    private TMP_Text aktiivneTekstiväli;

    private void Awake()
    {
        LaeJutustuseReadFailist();
    }

    private void OnEnable()
    {
        KäivitaJutustus();
    }

    private void LaeJutustuseReadFailist()
    {
        if (jutustuseFail != null && !string.IsNullOrWhiteSpace(jutustuseFail.text))
        {
            jutustuseRead = jutustuseFail.text
                .Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(rida => rida.Trim())
                .Where(rida => !string.IsNullOrEmpty(rida))
                .ToArray();
        }
        else
        {
            jutustuseRead = new string[0];
        }
    }

    public void SeaPealkiri(string uusPealkiri)
    {
        pealkiri = uusPealkiri;
    }

    public void SeaJutustuseFail(TextAsset uusFail)
    {
        jutustuseFail = uusFail;
        LaeJutustuseReadFailist();
    }

    private void KäivitaJutustus()
    {
        Menüü menüü = FindFirstObjectByType<Menüü>();
        if (menüü != null)
        {
            menüü.SulgeMenüü();
        }

        Time.timeScale = 0f;
        
        if (jutustusPaneel != null)
            jutustusPaneel.SetActive(true);

        onPealkiriFaasis = true;
        pealkiriOotamiseTaimer = 0f;
        pealkiriOnTäielikultNäidatud = false;
        kirjutabTeksti = false;

        if (pealkiriTekst != null)
            pealkiriTekst.gameObject.SetActive(true);

        if (jutumullObjekt != null)
            jutumullObjekt.SetActive(false);

        if (tegelaneObjekt != null)
            tegelaneObjekt.SetActive(false);

        praeguneRidaIndeks = 0;

        if (jutustuseRead == null || jutustuseRead.Length == 0)
        {
            LõpetaJutustus();
            return;
        }

        NäitaPealkiri();
    }

    private void Update()
    {
        if (onPealkiriFaasis && pealkiriOnTäielikultNäidatud)
        {
            pealkiriOotamiseTaimer += Time.unscaledDeltaTime;
            if (pealkiriOotamiseTaimer >= pealkiriKestusSek)
            {
                NäitaJutumulli();
            }
        }
    }

    public void TöötleKlikki()
    {
        if (kirjutabTeksti)
        {
            LõpetaKirjutamineKohe();
            return;
        }

        if (onPealkiriFaasis)
        {
            NäitaJutumulli();
            return;
        }

        JärgmineRidaVõiLõpeta();
    }

    private void NäitaPealkiri()
    {
        aktiivneTekstiväli = pealkiriTekst;
        praeguneTäisTekst = pealkiri;

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(aktiivneTekstiväli, praeguneTäisTekst, onValmis: () =>
            {
                pealkiriOnTäielikultNäidatud = true;
                pealkiriOotamiseTaimer = 0f;
            });
        }
        else
        {
            pealkiriTekst.text = pealkiri;
            pealkiriOnTäielikultNäidatud = true;
            pealkiriOotamiseTaimer = 0f;
        }
    }

    private void NäitaJutumulli()
    {
        onPealkiriFaasis = false;

        pealkiriTekst.gameObject.SetActive(false);
        jutumullObjekt.SetActive(true);
        tegelaneObjekt.SetActive(true);

        praeguneRidaIndeks = 0;
        NäitaRida(praeguneRidaIndeks);
    }

    private void NäitaRida(int indeks)
    {
        aktiivneTekstiväli = jutumulliTekst;
        praeguneTäisTekst = jutustuseRead[indeks];

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(aktiivneTekstiväli, praeguneTäisTekst, onValmis: null);
        }
        else
        {
            jutumulliTekst.text = praeguneTäisTekst;
        }
    }

    private void JärgmineRidaVõiLõpeta()
    {
        praeguneRidaIndeks++;

        if (praeguneRidaIndeks >= jutustuseRead.Length)
        {
            LõpetaJutustus();
            return;
        }

        NäitaRida(praeguneRidaIndeks);
    }

    private void LõpetaJutustus()
    {
        if (jutustusPaneel != null)
            jutustusPaneel.SetActive(false);

        Time.timeScale = 1f;
        JutustusLõppes.Invoke();
        enabled = false;
    }

    private void AlustaKirjutamist(TMP_Text tekstiväli, string koguTekst, System.Action onValmis)
    {
        if (kirjutamiseCoroutine != null)
            StopCoroutine(kirjutamiseCoroutine);

        kirjutamiseCoroutine = StartCoroutine(KirjutaTekstTähthaaval(tekstiväli, koguTekst, onValmis));
    }

    private IEnumerator KirjutaTekstTähthaaval(TMP_Text tekstiväli, string koguTekst, System.Action onValmis)
    {
        kirjutabTeksti = true;
        tekstiväli.text = "";

        foreach (char täht in koguTekst)
        {
            tekstiväli.text += täht;
            yield return new WaitForSecondsRealtime(täheIlmumiseViivitus);
        }

        kirjutabTeksti = false;
        kirjutamiseCoroutine = null;
        onValmis?.Invoke();
    }

    private void LõpetaKirjutamineKohe()
    {
        if (kirjutamiseCoroutine != null)
            StopCoroutine(kirjutamiseCoroutine);

        kirjutabTeksti = false;
        kirjutamiseCoroutine = null;

        if (aktiivneTekstiväli != null)
            aktiivneTekstiväli.text = praeguneTäisTekst;

        if (onPealkiriFaasis)
        {
            pealkiriOnTäielikultNäidatud = true;
            pealkiriOotamiseTaimer = 0f;
        }
    }
}
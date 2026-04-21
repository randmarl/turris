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
    [SerializeField] private TMP_Text jatkamiseTekst;

    [Header("Sätted")]
    [SerializeField] private string pealkiri = "Manivaldi matused";
    [SerializeField] private float pealkiriKestusSek = 2.0f;
    [SerializeField] private TextAsset jutustuseFail;

    [Header("Tekstiefekt")]
    [SerializeField] private bool kasutaTähthaavalEfekti = true;
    [SerializeField] private float täheIlmumiseViivitus = 0.04f;

    [Header("Jätkamise vihje")]
    [SerializeField] private float jatkamiseVihjeViivitus = 3f;
    [SerializeField] private float jatkamiseVihjeFadeKestus = 1f;
    [SerializeField] private float jatkamiseVihjeNähtavKestus = 1.2f;
    [SerializeField] private float jatkamiseVihjePeidusKestus = 0.8f;

    [Header("Sündmused")]
    public UnityEvent JutustusLõppes = new UnityEvent();

    private string[] jutustuseRead;
    private int praeguneRidaIndeks = 0;

    private bool onPealkiriFaasis = true;
    private bool kirjutabTeksti = false;

    private Coroutine kirjutamiseCoroutine;
    private Coroutine vihjeCoroutine;

    private float pealkiriOotamiseTaimer = 0f;
    private bool pealkiriOnTäielikultNäidatud = false;

    private string praeguneTäisTekst = "";
    private TMP_Text aktiivneTekstiväli;

    private float viimaseKlikiAeg = 0f;
    private bool jatkamiseVihjeNahtav = false;

    private void Awake()
    {
        LaeJutustuseReadFailist();
        PeidaJatkamiseVihje();
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

        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

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

        if (!onPealkiriFaasis && !kirjutabTeksti)
        {
            viimaseKlikiAeg += Time.unscaledDeltaTime;

            if (!jatkamiseVihjeNahtav && viimaseKlikiAeg >= jatkamiseVihjeViivitus)
            {
                NaitaJatkamiseVihje();
            }
        }
    }

    public void TöötleKlikki()
    {
        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

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

        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

        pealkiriTekst.gameObject.SetActive(false);
        jutumullObjekt.SetActive(true);
        tegelaneObjekt.SetActive(true);

        praeguneRidaIndeks = 0;
        NäitaRida(praeguneRidaIndeks);
    }

    private void NäitaRida(int indeks)
    {
        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

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
        PeidaJatkamiseVihje();

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

    private void PeidaJatkamiseVihje()
    {
        jatkamiseVihjeNahtav = false;

        if (vihjeCoroutine != null)
        {
            StopCoroutine(vihjeCoroutine);
            vihjeCoroutine = null;
        }

        if (jatkamiseTekst != null)
        {
            Color c = jatkamiseTekst.color;
            c.a = 0f;
            jatkamiseTekst.color = c;
        }
    }

    private void NaitaJatkamiseVihje()
    {
        jatkamiseVihjeNahtav = true;

        if (vihjeCoroutine != null)
            StopCoroutine(vihjeCoroutine);

        vihjeCoroutine = StartCoroutine(JatkamiseVihjeAnimatsioon());
    }

    private IEnumerator JatkamiseVihjeAnimatsioon()
    {
        if (jatkamiseTekst == null)
            yield break;

        Color c = jatkamiseTekst.color;
        c.a = 0f;
        jatkamiseTekst.color = c;

        while (jatkamiseVihjeNahtav)
        {
            float aeg = 0f;
            while (aeg < jatkamiseVihjeFadeKestus)
            {
                aeg += Time.unscaledDeltaTime;
                c.a = Mathf.Clamp01(aeg / jatkamiseVihjeFadeKestus);
                jatkamiseTekst.color = c;
                yield return null;
            }

            c.a = 1f;
            jatkamiseTekst.color = c;

            yield return new WaitForSecondsRealtime(jatkamiseVihjeNähtavKestus);

            aeg = 0f;
            while (aeg < jatkamiseVihjeFadeKestus)
            {
                aeg += Time.unscaledDeltaTime;
                c.a = 1f - Mathf.Clamp01(aeg / jatkamiseVihjeFadeKestus);
                jatkamiseTekst.color = c;
                yield return null;
            }

            c.a = 0f;
            jatkamiseTekst.color = c;

            yield return new WaitForSecondsRealtime(jatkamiseVihjePeidusKestus);
        }

        vihjeCoroutine = null;
    }
}
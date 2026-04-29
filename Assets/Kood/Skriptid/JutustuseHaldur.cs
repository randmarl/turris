using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
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
    [SerializeField] private float pealkiriKestusSekundites = 2f;
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

    private string[] jutustuseRead = Array.Empty<string>();
    private int praeguseReaIndeks;

    private bool onPealkiriFaasis = true;
    private bool kirjutabTeksti;
    private bool pealkiriOnTäielikultNäidatud;
    private bool jatkamiseVihjeNähtav;

    private Coroutine kirjutamiseCoroutine;
    private Coroutine vihjeCoroutine;

    private float pealkiriOotamiseTaimer;
    private float viimaseKlikiAeg;

    private string praeguneTäisTekst = "";
    private TMP_Text aktiivneTekstiväli;

    private void Awake()
    {
        LaeJutustuseReadFailist();
        PeidaJatkamiseVihje();
    }

    private void OnEnable()
    {
        KäivitaJutustus();
    }

    private void Update()
    {
        UuendaPealkiriFaasi();
        UuendaJatkamiseVihjet();
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
            menüü.SulgeMenüü();

        Time.timeScale = 0f;

        if (jutustusPaneel != null)
            jutustusPaneel.SetActive(true);

        LähtestaJutustuseOlek();

        if (jutustuseRead.Length == 0)
        {
            LõpetaJutustus();
            return;
        }

        NäitaPealkiri();
    }

    private void LähtestaJutustuseOlek()
    {
        viimaseKlikiAeg = 0f;
        pealkiriOotamiseTaimer = 0f;
        praeguseReaIndeks = 0;

        onPealkiriFaasis = true;
        pealkiriOnTäielikultNäidatud = false;
        kirjutabTeksti = false;

        PeidaJatkamiseVihje();

        if (pealkiriTekst != null)
            pealkiriTekst.gameObject.SetActive(true);

        if (jutumullObjekt != null)
            jutumullObjekt.SetActive(false);

        if (tegelaneObjekt != null)
            tegelaneObjekt.SetActive(false);
    }

    private void LaeJutustuseReadFailist()
    {
        if (jutustuseFail == null || string.IsNullOrWhiteSpace(jutustuseFail.text))
        {
            jutustuseRead = Array.Empty<string>();
            return;
        }

        jutustuseRead = jutustuseFail.text
            .Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(rida => rida.Trim())
            .Where(rida => !string.IsNullOrEmpty(rida))
            .ToArray();
    }

    private void UuendaPealkiriFaasi()
    {
        if (!onPealkiriFaasis || !pealkiriOnTäielikultNäidatud)
            return;

        pealkiriOotamiseTaimer += Time.unscaledDeltaTime;

        if (pealkiriOotamiseTaimer >= pealkiriKestusSekundites)
            NäitaJutumulli();
    }

    private void UuendaJatkamiseVihjet()
    {
        if (onPealkiriFaasis || kirjutabTeksti)
            return;

        viimaseKlikiAeg += Time.unscaledDeltaTime;

        if (!jatkamiseVihjeNähtav && viimaseKlikiAeg >= jatkamiseVihjeViivitus)
            NaitaJatkamiseVihje();
    }

    private void NäitaPealkiri()
    {
        aktiivneTekstiväli = pealkiriTekst;
        praeguneTäisTekst = pealkiri;

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(aktiivneTekstiväli, praeguneTäisTekst, () =>
            {
                pealkiriOnTäielikultNäidatud = true;
                pealkiriOotamiseTaimer = 0f;
            });

            return;
        }

        if (pealkiriTekst != null)
            pealkiriTekst.text = pealkiri;

        pealkiriOnTäielikultNäidatud = true;
        pealkiriOotamiseTaimer = 0f;
    }

    private void NäitaJutumulli()
    {
        onPealkiriFaasis = false;

        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

        if (pealkiriTekst != null)
            pealkiriTekst.gameObject.SetActive(false);

        if (jutumullObjekt != null)
            jutumullObjekt.SetActive(true);

        if (tegelaneObjekt != null)
            tegelaneObjekt.SetActive(true);

        praeguseReaIndeks = 0;
        NäitaRida(praeguseReaIndeks);
    }

    private void NäitaRida(int indeks)
    {
        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

        aktiivneTekstiväli = jutumulliTekst;
        praeguneTäisTekst = jutustuseRead[indeks];

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(aktiivneTekstiväli, praeguneTäisTekst, null);
            return;
        }

        if (jutumulliTekst != null)
            jutumulliTekst.text = praeguneTäisTekst;
    }

    private void JärgmineRidaVõiLõpeta()
    {
        praeguseReaIndeks++;

        if (praeguseReaIndeks >= jutustuseRead.Length)
        {
            LõpetaJutustus();
            return;
        }

        NäitaRida(praeguseReaIndeks);
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

    private void AlustaKirjutamist(TMP_Text tekstiväli, string koguTekst, Action onValmis)
    {
        if (tekstiväli == null)
            return;

        if (kirjutamiseCoroutine != null)
            StopCoroutine(kirjutamiseCoroutine);

        kirjutamiseCoroutine = StartCoroutine(KirjutaTekstTähthaaval(tekstiväli, koguTekst, onValmis));
    }

    private IEnumerator KirjutaTekstTähthaaval(TMP_Text tekstiväli, string koguTekst, Action onValmis)
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

        if (!onPealkiriFaasis)
            return;

        pealkiriOnTäielikultNäidatud = true;
        pealkiriOotamiseTaimer = 0f;
    }

    private void PeidaJatkamiseVihje()
    {
        jatkamiseVihjeNähtav = false;

        if (vihjeCoroutine != null)
        {
            StopCoroutine(vihjeCoroutine);
            vihjeCoroutine = null;
        }

        SeaJatkamiseTekstiLäbipaistvus(0f);
    }

    private void NaitaJatkamiseVihje()
    {
        jatkamiseVihjeNähtav = true;

        if (vihjeCoroutine != null)
            StopCoroutine(vihjeCoroutine);

        vihjeCoroutine = StartCoroutine(JatkamiseVihjeAnimatsioon());
    }

    private IEnumerator JatkamiseVihjeAnimatsioon()
    {
        if (jatkamiseTekst == null)
            yield break;

        SeaJatkamiseTekstiLäbipaistvus(0f);

        while (jatkamiseVihjeNähtav)
        {
            yield return MuudaJatkamiseVihjeLäbipaistvust(0f, 1f);
            yield return new WaitForSecondsRealtime(jatkamiseVihjeNähtavKestus);

            yield return MuudaJatkamiseVihjeLäbipaistvust(1f, 0f);
            yield return new WaitForSecondsRealtime(jatkamiseVihjePeidusKestus);
        }

        vihjeCoroutine = null;
    }

    private IEnumerator MuudaJatkamiseVihjeLäbipaistvust(float algus, float lõpp)
    {
        float aeg = 0f;

        while (aeg < jatkamiseVihjeFadeKestus)
        {
            aeg += Time.unscaledDeltaTime;
            float väärtus = Mathf.Lerp(algus, lõpp, aeg / jatkamiseVihjeFadeKestus);
            SeaJatkamiseTekstiLäbipaistvus(väärtus);

            yield return null;
        }

        SeaJatkamiseTekstiLäbipaistvus(lõpp);
    }

    private void SeaJatkamiseTekstiLäbipaistvus(float läbipaistvus)
    {
        if (jatkamiseTekst == null)
            return;

        Color värv = jatkamiseTekst.color;
        värv.a = läbipaistvus;
        jatkamiseTekst.color = värv;
    }
}
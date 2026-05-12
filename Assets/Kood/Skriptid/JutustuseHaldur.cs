using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class JutustuseHaldur : MonoBehaviour
{
    [Header("UI viited")]
    [SerializeField] private GameObject jutustuspaneel;
    [SerializeField] private TMP_Text pealkirjatekst;
    [SerializeField] private TMP_Text jutumullitekst;
    [SerializeField] private GameObject jutumullObjekt;
    [SerializeField] private GameObject tegelaneObjekt;
    [SerializeField] private TMP_Text jätkamiseTekst;

    [Header("Sätted")]
    [SerializeField] private string pealkiri = "Manivaldi matused";
    [SerializeField] private float pealkirjaKestusSekundites = 2f;
    [SerializeField] private TextAsset jutustusefail;

    [Header("Tekstiefekt")]
    [SerializeField] private bool kasutaTähthaavalEfekti = true;
    [SerializeField] private float täheIlmumiseViivitus = 0.04f;

    [Header("Jätkamise vihje")]
    [SerializeField] private float jätkamisevihjeViivitus = 3f;
    [SerializeField] private float jätkamisevihjeHaihtumiseKestus = 1f;
    [SerializeField] private float jätkamisevihjeNähtavKestus = 1.2f;
    [SerializeField] private float jätkamisevihjePeidusKestus = 0.8f;

    [Header("Sündmused")]
    public UnityEvent JutustusLõppes = new UnityEvent();

    private string[] jutustuseRead = Array.Empty<string>();
    private int praeguseReaIndeks;

    private bool onPealkiriFaasis = true;
    private bool kirjutabTeksti;
    private bool pealkiriOnTäielikultNäidatud;
    private bool jätkamisevihjeNähtav;

    private Coroutine kirjutamiseCoroutine;
    private Coroutine vihjeCoroutine;

    private float pealkirjaOotamiseTaimer;
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
            // klikk näitab terve teksti kohe
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
        jutustusefail = uusFail;
        LaeJutustuseReadFailist();
    }

    private void KäivitaJutustus()
    {
        Menüü menüü = FindFirstObjectByType<Menüü>();

        if (menüü != null)
            menüü.SulgeMenüü();

        // mäng pausile jutustuse ajaks
        Time.timeScale = 0f;

        if (jutustuspaneel != null)
            jutustuspaneel.SetActive(true);

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
        pealkirjaOotamiseTaimer = 0f;
        praeguseReaIndeks = 0;

        onPealkiriFaasis = true;
        pealkiriOnTäielikultNäidatud = false;
        kirjutabTeksti = false;

        PeidaJatkamiseVihje();

        if (pealkirjatekst != null)
            pealkirjatekst.gameObject.SetActive(true);

        if (jutumullObjekt != null)
            jutumullObjekt.SetActive(false);

        if (tegelaneObjekt != null)
            tegelaneObjekt.SetActive(false);
    }

    private void LaeJutustuseReadFailist()
    {
        if (jutustusefail == null || string.IsNullOrWhiteSpace(jutustusefail.text))
        {
            jutustuseRead = Array.Empty<string>();
            return;
        }

        // tekstifaili jagamine jutustuse osadeks
        jutustuseRead = jutustusefail.text
            .Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(rida => rida.Trim())
            .Where(rida => !string.IsNullOrEmpty(rida))
            .ToArray();
    }

    private void UuendaPealkiriFaasi()
    {
        if (!onPealkiriFaasis || !pealkiriOnTäielikultNäidatud)
            return;

        // paus töötab ka peatatud mängus
        pealkirjaOotamiseTaimer += Time.unscaledDeltaTime;

        if (pealkirjaOotamiseTaimer >= pealkirjaKestusSekundites)
            NäitaJutumulli();
    }

    private void UuendaJatkamiseVihjet()
    {
        if (onPealkiriFaasis || kirjutabTeksti)
            return;

        viimaseKlikiAeg += Time.unscaledDeltaTime;

        if (!jätkamisevihjeNähtav && viimaseKlikiAeg >= jätkamisevihjeViivitus)
            NaitaJatkamiseVihje();
    }

    private void NäitaPealkiri()
    {
        aktiivneTekstiväli = pealkirjatekst;
        praeguneTäisTekst = pealkiri;

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(aktiivneTekstiväli, praeguneTäisTekst, () =>
            {
                pealkiriOnTäielikultNäidatud = true;
                pealkirjaOotamiseTaimer = 0f;
            });

            return;
        }

        if (pealkirjatekst != null)
            pealkirjatekst.text = pealkiri;

        pealkiriOnTäielikultNäidatud = true;
        pealkirjaOotamiseTaimer = 0f;
    }

    private void NäitaJutumulli()
    {
        onPealkiriFaasis = false;

        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

        if (pealkirjatekst != null)
            pealkirjatekst.gameObject.SetActive(false);

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

        aktiivneTekstiväli = jutumullitekst;
        praeguneTäisTekst = jutustuseRead[indeks];

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(aktiivneTekstiväli, praeguneTäisTekst, null);
            return;
        }

        if (jutumullitekst != null)
            jutumullitekst.text = praeguneTäisTekst;
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

        if (jutustuspaneel != null)
            jutustuspaneel.SetActive(false);

        Time.timeScale = 1f;

        // annab teistele skriptidele teada
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

        // tähed ilmuvad ükshaaval
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
        pealkirjaOotamiseTaimer = 0f;
    }

    private void PeidaJatkamiseVihje()
    {
        jätkamisevihjeNähtav = false;

        if (vihjeCoroutine != null)
        {
            StopCoroutine(vihjeCoroutine);
            vihjeCoroutine = null;
        }

        SeaJatkamiseTekstiLäbipaistvus(0f);
    }

    private void NaitaJatkamiseVihje()
    {
        jätkamisevihjeNähtav = true;

        if (vihjeCoroutine != null)
            StopCoroutine(vihjeCoroutine);

        vihjeCoroutine = StartCoroutine(JatkamiseVihjeAnimatsioon());
    }

    private IEnumerator JatkamiseVihjeAnimatsioon()
    {
        if (jätkamiseTekst == null)
            yield break;

        SeaJatkamiseTekstiLäbipaistvus(0f);

        while (jätkamisevihjeNähtav)
        {
            yield return MuudaJatkamiseVihjeLäbipaistvust(0f, 1f);
            yield return new WaitForSecondsRealtime(jätkamisevihjeNähtavKestus);

            yield return MuudaJatkamiseVihjeLäbipaistvust(1f, 0f);
            yield return new WaitForSecondsRealtime(jätkamisevihjePeidusKestus);
        }

        vihjeCoroutine = null;
    }

    private IEnumerator MuudaJatkamiseVihjeLäbipaistvust(float algus, float lõpp)
    {
        float aeg = 0f;

        while (aeg < jätkamisevihjeHaihtumiseKestus)
        {
            aeg += Time.unscaledDeltaTime;

            // läbipaistvuse sujuv muutmine
            float väärtus = Mathf.Lerp(algus, lõpp, aeg / jätkamisevihjeHaihtumiseKestus);
            SeaJatkamiseTekstiLäbipaistvus(väärtus);

            yield return null;
        }

        SeaJatkamiseTekstiLäbipaistvus(lõpp);
    }

    private void SeaJatkamiseTekstiLäbipaistvus(float läbipaistvus)
    {
        if (jätkamiseTekst == null)
            return;

        Color värv = jätkamiseTekst.color;
        värv.a = läbipaistvus;
        jätkamiseTekst.color = värv;
    }
}
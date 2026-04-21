using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class ÕpetuseHaldur : MonoBehaviour
{
    [System.Serializable]
    public class ÕpetuseSamm
    {
        [TextArea(3, 8)]
        public string tekst;

        public GameObject visuaal;
    }

    [Header("UI viited")]
    [SerializeField] private GameObject õpetusPaneel;
    [SerializeField] private TMP_Text õpetusTekst;
    [SerializeField] private TMP_Text jatkamiseTekst;

    [Header("Õpetuse sammud")]
    [SerializeField] private ÕpetuseSamm[] sammud;

    [Header("Tekstiefekt")]
    [SerializeField] private bool kasutaTähthaavalEfekti = true;
    [SerializeField] private float täheIlmumiseViivitus = 0.04f;

    [Header("Jätkamise vihje")]
    [SerializeField] private float jatkamiseVihjeViivitus = 3f;
    [SerializeField] private float jatkamiseVihjeFadeKestus = 1f;
    [SerializeField] private float jatkamiseVihjeNähtavKestus = 1.2f;
    [SerializeField] private float jatkamiseVihjePeidusKestus = 0.8f;

    [Header("Sündmused")]
    public UnityEvent ÕpetusLõppes = new UnityEvent();

    private int praeguneSamm = 0;
    private bool kirjutabTeksti = false;

    private Coroutine kirjutamiseCoroutine;
    private Coroutine vihjeCoroutine;

    private string praeguneTäisTekst = "";
    private float viimaseKlikiAeg = 0f;
    private bool jatkamiseVihjeNahtav = false;

    private void Awake()
    {
        if (õpetusPaneel != null)
            õpetusPaneel.SetActive(false);

        PeidaJatkamiseVihje();
        PeidaKõikVisuaalid();
    }

    private void Update()
    {
        if (õpetusPaneel == null || !õpetusPaneel.activeSelf)
            return;

        if (!kirjutabTeksti)
        {
            viimaseKlikiAeg += Time.unscaledDeltaTime;

            if (!jatkamiseVihjeNahtav && viimaseKlikiAeg >= jatkamiseVihjeViivitus)
            {
                NäitaJatkamiseVihje();
            }
        }
    }

    public void KäivitaÕpetus()
    {
        if (õpetusPaneel == null)
            return;

        Time.timeScale = 0f;
        õpetusPaneel.SetActive(true);

        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

        praeguneSamm = 0;

        if (sammud == null || sammud.Length == 0)
        {
            LõpetaÕpetus();
            return;
        }

        NäitaSamm(praeguneSamm);
    }

    public void TöötleKlikki()
    {
        if (õpetusPaneel == null || !õpetusPaneel.activeSelf)
            return;

        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

        if (kirjutabTeksti)
        {
            LõpetaKirjutamineKohe();
            return;
        }

        praeguneSamm++;

        if (praeguneSamm >= sammud.Length)
        {
            LõpetaÕpetus();
            return;
        }

        NäitaSamm(praeguneSamm);
    }

    private void NäitaSamm(int indeks)
    {
        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();
        PeidaKõikVisuaalid();

        ÕpetuseSamm samm = sammud[indeks];
        praeguneTäisTekst = samm.tekst;

        if (samm.visuaal != null)
            samm.visuaal.SetActive(true);

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(praeguneTäisTekst);
        }
        else
        {
            õpetusTekst.text = praeguneTäisTekst;
        }
    }

    private void AlustaKirjutamist(string tekst)
    {
        if (kirjutamiseCoroutine != null)
            StopCoroutine(kirjutamiseCoroutine);

        kirjutamiseCoroutine = StartCoroutine(KirjutaTekstTähthaaval(tekst));
    }

    private IEnumerator KirjutaTekstTähthaaval(string tekst)
    {
        kirjutabTeksti = true;
        õpetusTekst.text = "";

        foreach (char täht in tekst)
        {
            õpetusTekst.text += täht;
            yield return new WaitForSecondsRealtime(täheIlmumiseViivitus);
        }

        kirjutabTeksti = false;
        kirjutamiseCoroutine = null;
    }

    private void LõpetaKirjutamineKohe()
    {
        if (kirjutamiseCoroutine != null)
            StopCoroutine(kirjutamiseCoroutine);

        kirjutamiseCoroutine = null;
        kirjutabTeksti = false;
        õpetusTekst.text = praeguneTäisTekst;
    }

    private void LõpetaÕpetus()
    {
        PeidaJatkamiseVihje();
        PeidaKõikVisuaalid();

        if (õpetusPaneel != null)
            õpetusPaneel.SetActive(false);

        Time.timeScale = 1f;
        ÕpetusLõppes.Invoke();
    }

    private void PeidaKõikVisuaalid()
    {
        if (sammud == null) return;

        foreach (var samm in sammud)
        {
            if (samm != null && samm.visuaal != null)
                samm.visuaal.SetActive(false);
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

    private void NäitaJatkamiseVihje()
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
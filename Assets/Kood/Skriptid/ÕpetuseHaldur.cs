using System.Collections;
using TMPro;
using UnityEngine;
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

    [Header("Käivitamine")]
    [SerializeField] private bool käivitaAutomaatselt = false;

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

    private int praeguneSamm;
    private bool kirjutabTeksti;
    private bool õpetusKäib;

    private Coroutine kirjutamiseCoroutine;
    private Coroutine vihjeCoroutine;

    private string praeguneTäisTekst = "";
    private float viimaseKlikiAeg;
    private bool jatkamiseVihjeNähtav;

    private void Awake()
    {
        // algolek enne mängu
        if (õpetusPaneel != null && !käivitaAutomaatselt)
            õpetusPaneel.SetActive(false);

        PeidaJatkamiseVihje();
        PeidaKõikVisuaalid();
    }

    private void Start()
    {
        if (käivitaAutomaatselt)
            KäivitaÕpetus();
    }

    private void Update()
    {
        if (!õpetusKäib || õpetusPaneel == null || !õpetusPaneel.activeSelf)
            return;

        if (kirjutabTeksti)
            return;

        // kasutaja passimise aeg
        viimaseKlikiAeg += Time.unscaledDeltaTime;

        if (!jatkamiseVihjeNähtav && viimaseKlikiAeg >= jatkamiseVihjeViivitus)
            NäitaJatkamiseVihje();
    }

    public void KäivitaÕpetus()
    {
        if (õpetusPaneel == null)
        {
            Debug.LogError("ÕpetuseHaldur: õpetusPaneel puudub.");
            return;
        }

        if (õpetusTekst == null)
        {
            Debug.LogError("ÕpetuseHaldur: õpetusTekst puudub.");
            return;
        }

        õpetusKäib = true;

        // mäng pausile, ui jääb tööle
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
        if (!õpetusKäib || õpetusPaneel == null || !õpetusPaneel.activeSelf)
            return;

        viimaseKlikiAeg = 0f;
        PeidaJatkamiseVihje();

        // pooleli tekst kohe lõpuni
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

        // ainult ühe sammu pilt nähtav
        PeidaKõikVisuaalid();

        ÕpetuseSamm samm = sammud[indeks];

        if (samm == null)
            return;

        praeguneTäisTekst = samm.tekst;

        if (samm.visuaal != null)
            samm.visuaal.SetActive(true);

        if (kasutaTähthaavalEfekti)
        {
            AlustaKirjutamist(praeguneTäisTekst);
            return;
        }

        õpetusTekst.text = praeguneTäisTekst;
    }

    private void AlustaKirjutamist(string tekst)
    {
        // vana kirjutamine kinni
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

            // pärisaja ootamine, sest mäng on pausil
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

        // kogu tekst korraga nähtavaks
        õpetusTekst.text = praeguneTäisTekst;
    }

    private void LõpetaÕpetus()
    {
        õpetusKäib = false;

        PeidaJatkamiseVihje();
        PeidaKõikVisuaalid();

        if (õpetusPaneel != null)
            õpetusPaneel.SetActive(false);

        // mäng käima tagasi
        Time.timeScale = 1f;

        ÕpetusLõppes.Invoke();
    }

    private void PeidaKõikVisuaalid()
    {
        if (sammud == null)
            return;

        foreach (ÕpetuseSamm samm in sammud)
        {
            if (samm != null && samm.visuaal != null)
                samm.visuaal.SetActive(false);
        }
    }

    private void PeidaJatkamiseVihje()
    {
        jatkamiseVihjeNähtav = false;

        // vilkumise coroutine kinni
        if (vihjeCoroutine != null)
        {
            StopCoroutine(vihjeCoroutine);
            vihjeCoroutine = null;
        }

        if (jatkamiseTekst != null)
        {
            // tekst läbipaistvaks
            Color värv = jatkamiseTekst.color;
            värv.a = 0f;
            jatkamiseTekst.color = värv;
        }
    }

    private void NäitaJatkamiseVihje()
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

        Color värv = jatkamiseTekst.color;
        värv.a = 0f;
        jatkamiseTekst.color = värv;

        // korduv haihtumine sisse ja välja
        while (jatkamiseVihjeNähtav)
        {
            float aeg = 0f;

            // sisse haihtumine
            while (aeg < jatkamiseVihjeFadeKestus)
            {
                aeg += Time.unscaledDeltaTime;
                värv.a = Mathf.Clamp01(aeg / jatkamiseVihjeFadeKestus);
                jatkamiseTekst.color = värv;
                yield return null;
            }

            värv.a = 1f;
            jatkamiseTekst.color = värv;

            yield return new WaitForSecondsRealtime(jatkamiseVihjeNähtavKestus);

            aeg = 0f;

            // välja haihtumine
            while (aeg < jatkamiseVihjeFadeKestus)
            {
                aeg += Time.unscaledDeltaTime;
                värv.a = 1f - Mathf.Clamp01(aeg / jatkamiseVihjeFadeKestus);
                jatkamiseTekst.color = värv;
                yield return null;
            }

            värv.a = 0f;
            jatkamiseTekst.color = värv;

            yield return new WaitForSecondsRealtime(jatkamiseVihjePeidusKestus);
        }

        vihjeCoroutine = null;
    }
}
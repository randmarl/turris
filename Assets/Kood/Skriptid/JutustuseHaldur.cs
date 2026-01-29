using UnityEngine;
using TMPro;
using System.Collections;

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
    [SerializeField] private string[] jutustuseRead;

    [Header("Tekstiefekt")]
    [SerializeField] private bool kasutaTähthaavalEfekti = true;
    [SerializeField] private float täheIlmumiseViivitus = 0.04f;

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
        if (jutustuseRead == null || jutustuseRead.Length == 0)
        {
            jutustuseRead = new string[]
            {
                "Tere! Mina olen Leemet. Ma tulin just Manivaldi matustelt. Oma silmaga pole seda Manivaldi küll kunagi näinud, sest ta ei elanud metsas.",
                "Onu Vootele rääkis, et Manivald on näinud ka Põhja Konna... Mis too veel on? Ei tea.", 
                "Lisaks andis Meeme mulle miskisuguse sõrmuse. Väga ilus on.",
                "Kurb küll, et suri mees, kellesarnaseid enam ei sünni, aga mets ei maga kunagi. Mul on vaja turvalist teed koju.",
                "Kaitse teed, enne kui vaenlased matuselisteni jõuavad!"
            };
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        jutustusPaneel.SetActive(true);

        onPealkiriFaasis = true;
        pealkiriOotamiseTaimer = 0f;
        pealkiriOnTäielikultNäidatud = false;

        pealkiriTekst.gameObject.SetActive(true);
        jutumullObjekt.SetActive(false);
        tegelaneObjekt.SetActive(false);

        praeguneRidaIndeks = 0;

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
        jutustusPaneel.SetActive(false);
        Time.timeScale = 1f;
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

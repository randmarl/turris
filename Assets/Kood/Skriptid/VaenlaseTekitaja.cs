using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VaenlaseTekitaja : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private VaenlasteTõenäosused[] vaenlased;

    [Header("Boss")]
    [SerializeField] private GameObject bossVaenlaseMall;
    [SerializeField] private int keskmiseLaineBoss = 5;
    [SerializeField] private int keskmiseLaineBossid = 1;
    [SerializeField] private int viimaseLaineBossid = 2;

    [Header("Atribuudid")]
    [SerializeField] private int algseltVaenlasi = 10;
    [SerializeField] private float vaenlasiSekundis = 1f;
    [SerializeField] private float aegLaineteVahel = 5f;
    [SerializeField] private float raskuseTegur = 1.5f;
    [SerializeField] private float vaenlasiSekundisPiir = 15f;

    [Header("Leveli lõpp")]
    [SerializeField] private int maksimaalneLaineteArv = 10;

    [Header("Sündmused")]
    public static UnityEvent vaenlaseHävitamiseSündmus = new UnityEvent();
    [HideInInspector] public UnityEvent TaseLäbitud = new UnityEvent();

    public event Action<int, int> LaineMuutus;

    public int PraeguneLaine => praeguneLaine;
    public int MaksimaalneLaineteArv => maksimaalneLaineteArv;
    public bool KasLaineKäib => kasLaineKäib;
    public float LaineKestus => laineKestus;
    public bool KasOnViimaneLaine => praeguneLaine >= maksimaalneLaineteArv;

    private int praeguneLaine = 1;
    private int vaenlasiElus;
    private int vaenlasiTekitada;

    private float laineKestus;

    private bool kasLaineKäib;
    private bool taseOnLäbitud;

    private Coroutine järgmiseLaineCoroutine;

    private void Awake()
    {
        vaenlaseHävitamiseSündmus.AddListener(VaenlaneHävitatud);
    }

    private void Start()
    {
        TeavitaLaineUuendusest();
        järgmiseLaineCoroutine = StartCoroutine(AlustaLainet(aegLaineteVahel, praeguneLaine));
    }

    private void OnDestroy()
    {
        vaenlaseHävitamiseSündmus.RemoveListener(VaenlaneHävitatud);
    }

    private void Update()
    {
        if (taseOnLäbitud || !kasLaineKäib)
            return;

        UuendaLaineAega();

        if (KasOnViimaneLaine && vaenlasiTekitada == 0 && vaenlasiElus == 0)
        {
            taseOnLäbitud = true;
            kasLaineKäib = false;
            TaseLäbitud.Invoke();
        }
    }

    public void KäivitaJärgmineLaineKohe()
    {
        if (taseOnLäbitud || !kasLaineKäib || KasOnViimaneLaine)
            return;

        // praeguse laine ülejäänud vaenlased tulevad lõpuni
        if (järgmiseLaineCoroutine != null)
        {
            StopCoroutine(järgmiseLaineCoroutine);
            järgmiseLaineCoroutine = null;
        }

        AlustaJärgmiseLaineOotamist(0f);
    }

    private void UuendaLaineAega()
    {
        laineKestus += Time.deltaTime;
    }

    private IEnumerator AlustaLainet(float ooteaeg, int laineNumber)
    {
        yield return new WaitForSeconds(ooteaeg);

        if (taseOnLäbitud)
            yield break;

        järgmiseLaineCoroutine = null;
        kasLaineKäib = true;

        laineKestus = 0f;

        yield return StartCoroutine(TekitaLaineVaenlased(laineNumber));
    }

    private IEnumerator TekitaLaineVaenlased(int laineNumber)
    {
        this.vaenlasiTekitada++;

        // määrab laine vaenlaste arvu ja tekkimiskiiruse
        int vaenlasiTekitada = VaenlasiLaines(laineNumber);
        float vaenlasiSekundisHetkel = VaenlasiSekundiga(laineNumber);

        while (vaenlasiTekitada > 0)
        {
            bool vaenlaneTekkis = TekitaVaenlane();

            if (vaenlaneTekkis)
            {
                vaenlasiTekitada--;
                vaenlasiElus++;
            }

            yield return new WaitForSeconds(1f / vaenlasiSekundisHetkel);
        }

        ProoviTekitadaBossSellesLaines(laineNumber);

        this.vaenlasiTekitada--;

        if (!KasOnViimaneLaine && laineNumber == praeguneLaine)
            AlustaJärgmiseLaineOotamist(aegLaineteVahel);
    }

    private void ProoviTekitadaBossSellesLaines(int laineNumber)
    {
        if (bossVaenlaseMall == null)
            return;

        if (laineNumber == maksimaalneLaineteArv)
        {
            TekitaBossidKohe(viimaseLaineBossid);
            return;
        }

        if (laineNumber == keskmiseLaineBoss)
            TekitaBossidKohe(keskmiseLaineBossid);
    }

    private void TekitaBossidKohe(int bossideArv)
    {
        for (int i = 0; i < bossideArv; i++)
        {
            bool bossTekkis = TekitaKindelVaenlane(bossVaenlaseMall);

            if (bossTekkis)
                vaenlasiElus++;
        }
    }

    private void VaenlaneHävitatud()
    {
        // vaenlaste arv ei lähe miinusesse
        vaenlasiElus = Mathf.Max(0, vaenlasiElus - 1);
    }

    private void AlustaJärgmiseLaineOotamist(float ooteaeg)
    {
        praeguneLaine++;
        TeavitaLaineUuendusest();

        if (järgmiseLaineCoroutine != null)
            StopCoroutine(järgmiseLaineCoroutine);

        järgmiseLaineCoroutine = StartCoroutine(AlustaLainet(ooteaeg, praeguneLaine));
    }

    private void TeavitaLaineUuendusest()
    {
        LaineMuutus?.Invoke(praeguneLaine, maksimaalneLaineteArv);
    }

    private bool TekitaVaenlane()
    {
        GameObject mall = VõtaJärgmineVaenlane();

        if (mall == null || Haldur.Peamine == null || Haldur.Peamine.AlgusPunktid.Length == 0)
            return false;

        Instantiate(mall, Haldur.Peamine.AlgusPunktid[0].position, Quaternion.identity);
        return true;
    }

    private bool TekitaKindelVaenlane(GameObject mall)
    {
        if (mall == null || Haldur.Peamine == null || Haldur.Peamine.AlgusPunktid.Length == 0)
            return false;

        Instantiate(mall, Haldur.Peamine.AlgusPunktid[0].position, Quaternion.identity);
        return true;
    }

    private GameObject VõtaJärgmineVaenlane()
    {
        return VõtaJuhuslikVaenlaneKaaluJärgi();
    }

    private GameObject VõtaJuhuslikVaenlaneKaaluJärgi()
    {
        float kogukaal = 0f;

        foreach (var v in vaenlased)
        {
            if (v != null && v.Prefab != null && v.Kaal > 0f)
                kogukaal += v.Kaal;
        }

        if (kogukaal <= 0f)
            return null;

        // valib juhusliku numbri
        float juhuslik = UnityEngine.Random.value * kogukaal;
        float jooksev = 0f;

        foreach (var v in vaenlased)
        {
            if (v == null || v.Prefab == null || v.Kaal <= 0f)
                continue;

            // liidab vaenlaste kaale kokku
            jooksev += v.Kaal;

            if (juhuslik <= jooksev)
                return v.Prefab;
        }

        return vaenlased[vaenlased.Length - 1].Prefab;
    }

    private int VaenlasiLaines(int laineNumber)
    {
        // vaenlaste arv kasvab laine numbriga
        return Mathf.RoundToInt(algseltVaenlasi * Mathf.Pow(laineNumber, raskuseTegur));
    }

    private float VaenlasiSekundiga(int laineNumber)
    {
        // tekitamise kiirus kasvab, aga piirini
        return Mathf.Clamp(
            vaenlasiSekundis * Mathf.Pow(laineNumber, raskuseTegur),
            0.01f,
            vaenlasiSekundisPiir
        );
    }
}
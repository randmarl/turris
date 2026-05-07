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

    [Header("Atribuudid")]
    [SerializeField] private int algseltVaenlasi = 10;
    [SerializeField] private float vaenlasiSekundis = 1f;
    [SerializeField] private float aegLaineteVahel = 5f;
    [SerializeField] private float raskuseTegur = 1.5f;
    [SerializeField] private float vaenlasiSekundisPiir = 15f;

    [Header("Leveli lõpp")]
    [SerializeField] private int maksimaalneLaineteArv = 5;

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

    private float aegViimasestTekitamisest;
    private float vaenlasiSekundisHetkel;
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
        järgmiseLaineCoroutine = StartCoroutine(AlustaLainet());
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
        ProoviVaenlaneTekitada();

        if (vaenlasiElus == 0 && vaenlasiTekitada == 0)
            LõpetaLaine();
    }

    public void KäivitaJärgmineLaineKohe()
    {
        if (taseOnLäbitud || !kasLaineKäib || KasOnViimaneLaine)
            return;

        LõpetaPraeguneLaine();
        AlustaJärgmiseLaineOotamist();
    }

    private void UuendaLaineAega()
    {
        laineKestus += Time.deltaTime;
        aegViimasestTekitamisest += Time.deltaTime;
    }

    private void ProoviVaenlaneTekitada()
    {
        if (vaenlasiTekitada <= 0)
            return;

        if (aegViimasestTekitamisest < 1f / vaenlasiSekundisHetkel)
            return;

        bool vaenlaneTekkis = TekitaVaenlane();

        if (!vaenlaneTekkis)
            return;

        vaenlasiTekitada--;
        vaenlasiElus++;
        aegViimasestTekitamisest = 0f;
    }

    private void VaenlaneHävitatud()
    {
        vaenlasiElus = Mathf.Max(0, vaenlasiElus - 1);
    }

    private IEnumerator AlustaLainet()
    {
        yield return new WaitForSeconds(aegLaineteVahel);

        if (taseOnLäbitud)
            yield break;

        järgmiseLaineCoroutine = null;
        kasLaineKäib = true;
        laineKestus = 0f;
        aegViimasestTekitamisest = 0f;
        vaenlasiTekitada = VaenlasiLaines();
        vaenlasiSekundisHetkel = VaenlasiSekundiga();
    }

    private void LõpetaLaine()
    {
        LõpetaPraeguneLaine();

        if (KasOnViimaneLaine)
        {
            taseOnLäbitud = true;
            TaseLäbitud.Invoke();
            return;
        }

        AlustaJärgmiseLaineOotamist();
    }

    private void LõpetaPraeguneLaine()
    {
        kasLaineKäib = false;
        laineKestus = 0f;
        aegViimasestTekitamisest = 0f;
    }

    private void AlustaJärgmiseLaineOotamist()
    {
        praeguneLaine++;
        TeavitaLaineUuendusest();

        if (järgmiseLaineCoroutine != null)
            StopCoroutine(järgmiseLaineCoroutine);

        järgmiseLaineCoroutine = StartCoroutine(AlustaLainet());
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

    private GameObject VõtaJärgmineVaenlane()
    {
        bool onViimaseLaineViimaneVaenlane = KasOnViimaneLaine && vaenlasiTekitada == 1;

        if (onViimaseLaineViimaneVaenlane && bossVaenlaseMall != null)
            return bossVaenlaseMall;

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

        float juhuslik = UnityEngine.Random.value * kogukaal;
        float jooksev = 0f;

        foreach (var v in vaenlased)
        {
            if (v == null || v.Prefab == null || v.Kaal <= 0f)
                continue;

            jooksev += v.Kaal;

            if (juhuslik <= jooksev)
                return v.Prefab;
        }

        return vaenlased[vaenlased.Length - 1].Prefab;
    }

    private int VaenlasiLaines()
    {
        return Mathf.RoundToInt(algseltVaenlasi * Mathf.Pow(praeguneLaine, raskuseTegur));
    }

    private float VaenlasiSekundiga()
    {
        return Mathf.Clamp(
            vaenlasiSekundis * Mathf.Pow(praeguneLaine, raskuseTegur),
            0f,
            vaenlasiSekundisPiir
        );
    }
}
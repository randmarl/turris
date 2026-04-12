using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class VaenlaseTekitaja : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private VaenlasteTõenäosused[] vaenlased;

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

    private int praeguneLaine = 1;
    private float aegViimasestTekitamisest;
    private int vaenlasiElus;
    private int vaenlasiTekitada;
    private float vaenlasiSekundisHetkel;
    private bool kasTekib = false;
    private bool taseOnLäbitud = false;

    private void Awake()
    {
        vaenlaseHävitamiseSündmus.AddListener(VaenlaneHävitatud);
    }

    private void OnDestroy()
    {
        vaenlaseHävitamiseSündmus.RemoveListener(VaenlaneHävitatud);
    }

    private void Start()
    {
        TeavitaLaineUuendusest();
        StartCoroutine(AlustaLainet());
    }

    private void Update()
    {
        if (taseOnLäbitud) return;
        if (!kasTekib) return;

        aegViimasestTekitamisest += Time.deltaTime;

        if (aegViimasestTekitamisest >= (1f / vaenlasiSekundisHetkel) && vaenlasiTekitada > 0)
        {
            TekitaVaenlane();
            vaenlasiTekitada--;
            vaenlasiElus++;
            aegViimasestTekitamisest = 0f;
        }

        if (vaenlasiElus == 0 && vaenlasiTekitada == 0)
        {
            LõpetaLaine();
        }
    }

    private void VaenlaneHävitatud()
    {
        vaenlasiElus--;

        if (vaenlasiElus < 0)
            vaenlasiElus = 0;
    }

    private IEnumerator AlustaLainet()
    {
        yield return new WaitForSeconds(aegLaineteVahel);

        if (taseOnLäbitud)
            yield break;

        kasTekib = true;
        vaenlasiTekitada = VaenlasiLaines();
        vaenlasiSekundisHetkel = VaenlasiSekundiga();
    }

    private void LõpetaLaine()
    {
        kasTekib = false;
        aegViimasestTekitamisest = 0f;

        if (praeguneLaine >= maksimaalneLaineteArv)
        {
            taseOnLäbitud = true;
            TaseLäbitud.Invoke();
            return;
        }

        praeguneLaine++;
        TeavitaLaineUuendusest();
        StartCoroutine(AlustaLainet());
    }

    private void TeavitaLaineUuendusest()
    {
        LaineMuutus?.Invoke(praeguneLaine, maksimaalneLaineteArv);
    }

    private void TekitaVaenlane()
    {
        GameObject mall = VõtaJuhuslikVaenlaneKaaluJärgi();
        if (mall == null)
            return;

        Instantiate(mall, haldur.peamine.algusPunkt[0].position, Quaternion.identity);
    }

    private GameObject VõtaJuhuslikVaenlaneKaaluJärgi()
    {
        float kogukaal = 0f;

        foreach (var v in vaenlased)
        {
            if (v != null && v.prefab != null && v.kaal > 0f)
                kogukaal += v.kaal;
        }

        if (kogukaal <= 0f)
            return null;

        float juhuslik = UnityEngine.Random.value * kogukaal;
        float jooksev = 0f;

        foreach (var v in vaenlased)
        {
            if (v == null || v.prefab == null || v.kaal <= 0f)
                continue;

            jooksev += v.kaal;

            if (juhuslik <= jooksev)
                return v.prefab;
        }

        return vaenlased[vaenlased.Length - 1].prefab;
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
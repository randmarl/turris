using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class VaenlaseTekitaja : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private GameObject[] vaenlaseMallid;

    [Header("Atribuudid")]
    [SerializeField] private int algseltVaenlasi = 10;
    [SerializeField] private float vaenlasiSekundis = 1f;
    [SerializeField] private float aegLaineteVahel = 5f;
    [SerializeField] private float raskuseTegur = 1.5f;
    [SerializeField] private float vaenlasiSekundisPiir = 15f;

    [Header("Sündmused")]
    public static UnityEvent vaenlaseHävitamiseSündmus = new UnityEvent();

    private int praeguneLaine = 1;
    private float aegViimasestTekitamisest;
    private int vaenlasiElus;
    private int vaenlasiTekitada;
    private float vaenlasiSekundisHetkel;
    private bool kasTekib = false;

    private void Awake()
    {
        vaenlaseHävitamiseSündmus.AddListener(VaenlaneHävitatud);
    }

    private void Start()
    {
        StartCoroutine(AlustaLainet());
    }

    private void Update()
    {
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
    }

    private IEnumerator AlustaLainet()
    {
        yield return new WaitForSeconds(aegLaineteVahel);
        kasTekib = true;
        vaenlasiTekitada = VaenlasiLaines();
        vaenlasiSekundisHetkel = VaenlasiSekundiga();
    }

    private void LõpetaLaine()
    {
        kasTekib = false;
        aegViimasestTekitamisest = 0f;
        praeguneLaine++;
        StartCoroutine(AlustaLainet());
    }

    private void TekitaVaenlane()
    {
        int indeks = Random.Range(0, vaenlaseMallid.Length);
        GameObject mall = vaenlaseMallid[indeks];
        Instantiate(mall, haldur.peamine.algusPunkt[0].position, Quaternion.identity);
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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoeHaldur : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button turrisNupp;
    [SerializeField] private TMP_Text turriseHinnaTekst;
    [SerializeField] private Transform[] kohad;

    [Header("Prefabs")]
    [SerializeField] private GameObject tornikaardiPrefab;
    [SerializeField] private TornideTõenäosused[] tornid;

    [Header("Seaded")]
    [SerializeField] private int algHind = 50;
    [SerializeField] private int hinnaTõus = 5;

    private int torniHind;
    private bool esimeneOstTehtud;

    private void Awake()
    {
        torniHind = algHind;
        UuendaHinnaTeksti();
    }

    private void Update()
    {
        UuendaNupuOleku();
    }

    public void VajutatiTurrist()
    {
        if (OravanahaHaldur.Instance == null || tornikaardiPrefab == null || tornid == null || tornid.Length == 0)
            return;

        Transform tühiSlot = LeiaTühiSlot();
        if (tühiSlot == null)
            return;

        GameObject valitudTorn = VõtaValitudTorn();
        if (valitudTorn == null)
            return;

        if (!OravanahaHaldur.Instance.KulutaOravanahku(torniHind))
            return;

        GameObject kaartObjekt = Instantiate(tornikaardiPrefab, tühiSlot);
        Tornikaart tornikaart = kaartObjekt.GetComponent<Tornikaart>();

        if (tornikaart != null)
            tornikaart.SeaTorn(valitudTorn);

        torniHind += hinnaTõus;
        UuendaHinnaTeksti();
        UuendaNupuOleku();
    }

    private void UuendaNupuOleku()
    {
        if (turrisNupp == null)
            return;

        if (OravanahaHaldur.Instance == null)
        {
            turrisNupp.interactable = false;
            return;
        }

        bool onRaha = OravanahaHaldur.Instance.Oravanahad >= torniHind;
        bool onVabaKoht = LeiaTühiSlot() != null;

        turrisNupp.interactable = onRaha && onVabaKoht;
    }

    private void UuendaHinnaTeksti()
    {
        if (turriseHinnaTekst != null)
            turriseHinnaTekst.text = torniHind.ToString();
    }

    private GameObject VõtaValitudTorn()
    {
        if (!esimeneOstTehtud)
        {
            esimeneOstTehtud = true;
            return tornid[0] != null ? tornid[0].Prefab : null;
        }

        return VõtaJuhuslikTornKaaluJärgi();
    }

    private GameObject VõtaJuhuslikTornKaaluJärgi()
    {
        float kogukaal = 0f;

        foreach (TornideTõenäosused torn in tornid)
        {
            if (torn != null && torn.Prefab != null && torn.Kaal > 0f)
                kogukaal += torn.Kaal;
        }

        if (kogukaal <= 0f)
            return null;

        float juhuslik = Random.value * kogukaal;
        float jooksevKaal = 0f;

        foreach (TornideTõenäosused torn in tornid)
        {
            if (torn == null || torn.Prefab == null || torn.Kaal <= 0f)
                continue;

            jooksevKaal += torn.Kaal;

            if (juhuslik <= jooksevKaal)
                return torn.Prefab;
        }

        return null;
    }

    private Transform LeiaTühiSlot()
    {
        if (kohad == null)
            return null;

        foreach (Transform koht in kohad)
        {
            if (koht == null)
                continue;

            if (koht.GetComponentInChildren<Tornikaart>() == null)
                return koht;
        }

        return null;
    }
}
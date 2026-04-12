using UnityEngine;
using UnityEngine.UI;

public class Poehaldur : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button turrisNupp;
    [SerializeField] private Transform[] kohad;

    [Header("Prefabs")]
    [SerializeField] private GameObject tornikaardiPrefab;
    [SerializeField] private TornideTõenäosused[] tornid;

    [Header("Seaded")]
    [SerializeField] private int torniHind = 100;
    private bool esimeneOstTehtud = false;

    private void Update()
    {
        if (OravanahaHaldur.Instance == null)
        {
            if (turrisNupp != null)
                turrisNupp.interactable = false;
            return;
        }

        bool onRaha = OravanahaHaldur.Instance.Oravanahad >= torniHind;
        bool onVabaSlot = LeiaTühiSlot() != null;

        if (turrisNupp != null)
            turrisNupp.interactable = onRaha && onVabaSlot;
    }

    public void VajutatiTurrist()
    {
        if (OravanahaHaldur.Instance == null)
            return;

        Transform tühiSlot = LeiaTühiSlot();
        if (tühiSlot == null)
            return;

        bool õnnestus = OravanahaHaldur.Instance.KulutaOravanahku(torniHind);
        if (!õnnestus)
            return;

        if (tornid == null || tornid.Length == 0)
            return;

        if (tornikaardiPrefab == null)
            return;

        GameObject valitudTorn = null;

        if (!esimeneOstTehtud)
        {
            if (tornid[0] != null)
                valitudTorn = tornid[0].prefab;

            esimeneOstTehtud = true;
        }
        else
        {
            valitudTorn = VõtaJuhuslikTornKaaluJärgi();
        }

        if (valitudTorn == null)
            return;

        GameObject kaartObjekt = Instantiate(tornikaardiPrefab, tühiSlot);
        Tornikaart tornikaart = kaartObjekt.GetComponent<Tornikaart>();

        if (tornikaart != null)
            tornikaart.SeaTorn(valitudTorn);
    }

    private GameObject VõtaJuhuslikTornKaaluJärgi()
    {
        float kogukaal = 0f;

        foreach (var torn in tornid)
        {
            if (torn != null && torn.prefab != null && torn.kaal > 0f)
                kogukaal += torn.kaal;
        }

        if (kogukaal <= 0f)
            return null;

        float juhuslik = Random.value * kogukaal;
        float jooksev = 0f;

        foreach (var torn in tornid)
        {
            if (torn == null || torn.prefab == null || torn.kaal <= 0f)
                continue;

            jooksev += torn.kaal;

            if (juhuslik <= jooksev)
                return torn.prefab;
        }

        return tornid[tornid.Length - 1].prefab;
    }

    private Transform LeiaTühiSlot()
    {
        if (kohad == null)
            return null;

        foreach (Transform slot in kohad)
        {
            if (slot == null)
                continue;

            Tornikaart olemasolevKaart = slot.GetComponentInChildren<Tornikaart>();
            if (olemasolevKaart == null)
                return slot;
        }

        return null;
    }
}
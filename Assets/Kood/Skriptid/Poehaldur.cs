using UnityEngine;
using UnityEngine.UI;

public class Poehaldur : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button turrisNupp;
    [SerializeField] private Transform[] kohad;

    [Header("Prefabs")]
    [SerializeField] private GameObject tornikaardiPrefab;
    [SerializeField] private GameObject[] torniPrefabid;

    [Header("Seaded")]
    [SerializeField] private int torniHind = 100;

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

        if (torniPrefabid == null || torniPrefabid.Length == 0)
            return;

        if (tornikaardiPrefab == null)
            return;

        GameObject valitudTorn = torniPrefabid[Random.Range(0, torniPrefabid.Length)];

        GameObject kaartObjekt = Instantiate(tornikaardiPrefab, tühiSlot);
        Tornikaart tornikaart = kaartObjekt.GetComponent<Tornikaart>();

        if (tornikaart != null)
            tornikaart.SeaTorn(valitudTorn);
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
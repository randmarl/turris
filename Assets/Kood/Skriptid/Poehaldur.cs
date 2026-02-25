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
            if (turrisNupp != null) turrisNupp.interactable = false;
            return;
        }

        bool onRaha = OravanahaHaldur.Instance.Oravanahad >= torniHind;
        bool onVabaSlot = LeiaTühiSlot() != null;

        if (turrisNupp != null)
            turrisNupp.interactable = onRaha && onVabaSlot;
    }

    public void VajutatiTurrist()
    {
        //Debug.Log("TURRIS vajutati");

        if (OravanahaHaldur.Instance == null)
        {
            //Debug.LogError("OravanahaHaldur puudub!");
            return;
        }

        Transform tühiSlot = LeiaTühiSlot();
        if (tühiSlot == null)
        {
            //Debug.Log("Pole vaba sloti");
            return;
        }

        bool õnnestus = OravanahaHaldur.Instance.KulutaOravanahku(torniHind);
        if (!õnnestus)
        {
            //Debug.Log("Pole piisavalt oravanahku");
            return;
        }

        if (torniPrefabid == null || torniPrefabid.Length == 0)
        {
            //Debug.LogError("Torni prefabid pole määratud!");
            return;
        }

        if (tornikaardiPrefab == null)
        {
            //Debug.LogError("Tornikaardi prefab pole määratud!");
            return;
        }

        GameObject valitudTorn = torniPrefabid[Random.Range(0, torniPrefabid.Length)];

        GameObject kaartObjekt = Instantiate(tornikaardiPrefab, tühiSlot);
        Tornikaart tornikaart = kaartObjekt.GetComponent<Tornikaart>();

        if (tornikaart != null)
        {
            tornikaart.SeaTorn(valitudTorn);
        }

        //Debug.Log("Tornikaart loodud sloti: " + tühiSlot.name);
    }

    private Transform LeiaTühiSlot()
    {
        if (kohad == null) return null;

        foreach (Transform slot in kohad)
        {
            if (slot == null) continue;
            Tornikaart olemasolevKaart = slot.GetComponentInChildren<Tornikaart>();
            if (olemasolevKaart == null)
                return slot;
        }

        return null;
    }
}

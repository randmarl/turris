using UnityEngine;
using UnityEngine.UI;

public class Poehaldur : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button turrisNupp;
    [SerializeField] private Transform[] kohad;

    [Header("Tornikaardi prefab")]
    [SerializeField] private Tornikaart tornikaardiPrefab;

    [Header("Tornid")]
    [SerializeField] private GameObject[] torniPrefabid;
    [SerializeField] private Sprite[] torniIkoonid;

    [Header("Hind")]
    [SerializeField] private int turriseHind = 100;

    private void OnEnable()
    {
        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.OravanahadMuutusid += UuendaNupuOlekut;

        UuendaNupuOlekut(OravanahaHaldur.Instance != null ? OravanahaHaldur.Instance.Oravanahad : 0);
    }

    private void OnDisable()
    {
        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.OravanahadMuutusid -= UuendaNupuOlekut;
    }

    public void VajutatiTurrist()
    {
        if (OravanahaHaldur.Instance == null) return;

        if (!OravanahaHaldur.Instance.KulutaOravanahku(turriseHind))
            return;

        Transform tühiSlot = LeiaTühiSlot();
        if (tühiSlot == null)
        {
            OravanahaHaldur.Instance.LisaOravanahku(turriseHind);
            return;
        }

        int indeks = Random.Range(0, torniPrefabid.Length);
        Tornikaart uusKaart = Instantiate(tornikaardiPrefab, tühiSlot);
        Sprite ikoon = (torniIkoonid != null && torniIkoonid.Length > indeks) ? torniIkoonid[indeks] : null;
        uusKaart.SeaSisu(torniPrefabid[indeks], ikoon);
    }

    private void UuendaNupuOlekut(int oravanahad)
    {
        if (turrisNupp == null) return;
        turrisNupp.interactable = oravanahad >= turriseHind && LeiaTühiSlot() != null;
    }

    private Transform LeiaTühiSlot()
    {
        foreach (Transform slot in kohad)
        {
            if (slot == null) continue;
            if (slot.childCount == 0) return slot;
        }
        return null;
    }
}

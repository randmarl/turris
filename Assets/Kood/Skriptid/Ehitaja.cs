using UnityEngine;

public class Ehitaja : MonoBehaviour
{
    public static Ehitaja peamine;

    [Header("Seaded")]
    [SerializeField] private int ehituseHind = 100;

    [Header("Tornid (random)")]
    [SerializeField] private GameObject[] torniPrefabid;

    private void Awake()
    {
        peamine = this;
    }

    public int VõtaEhitusHind()
    {
        return ehituseHind;
    }

    public GameObject VõtaSuvalineTornPrefab()
    {
        if (torniPrefabid == null || torniPrefabid.Length == 0) return null;
        return torniPrefabid[Random.Range(0, torniPrefabid.Length)];
    }
}
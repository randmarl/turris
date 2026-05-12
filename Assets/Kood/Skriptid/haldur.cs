using UnityEngine;

public class Haldur : MonoBehaviour
{
    public static Haldur Peamine { get; private set; }

    [Header("Rada")]
    [SerializeField] private Transform[] alguspunktid;
    [SerializeField] private Transform[] teekond;

    public Transform[] AlgusPunktid => alguspunktid;
    public Transform[] Teekond => teekond;

    private void Awake()
    {
        if (Peamine != null && Peamine != this)
        {
            // eemaldab üleliigse halduri
            Destroy(gameObject);
            return;
        }

        // salvestab selle peamiseks halduriks
        Peamine = this;
    }
}
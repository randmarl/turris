using UnityEngine;

public class Haldur : MonoBehaviour
{
    public static Haldur Peamine { get; private set; }

    [Header("Rada")]
    [SerializeField] private Transform[] algusPunktid;
    [SerializeField] private Transform[] teekond;

    public Transform[] AlgusPunktid => algusPunktid;
    public Transform[] Teekond => teekond;

    private void Awake()
    {
        if (Peamine != null && Peamine != this)
        {
            Destroy(gameObject);
            return;
        }

        Peamine = this;
    }
}
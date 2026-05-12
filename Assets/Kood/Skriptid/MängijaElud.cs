using UnityEngine;
using UnityEngine.Events;

public class MängijaElud : MonoBehaviour
{
    public static MängijaElud Instance { get; private set; }

    [Header("Seaded")]
    [SerializeField] private int algsedElud = 10;

    public int Elud { get; private set; }

    [HideInInspector] public UnityEvent<int> EludMuutusid = new UnityEvent<int>();
    [HideInInspector] public UnityEvent MängLäbi = new UnityEvent();

    private bool mängLäbiKäivitunud;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // eemaldab üleliigse elude süsteemi
            Destroy(gameObject);
            return;
        }

        // salvestab selle peamiseks elude süsteemiks
        Instance = this;
        Elud = algsedElud;
    }

    private void Start()
    {
        EludMuutusid.Invoke(Elud);
    }

    public void VõtaElu(int kogus = 1)
    {
        if (Elud <= 0)
            return;

        Elud -= kogus;
        Elud = Mathf.Max(Elud, 0);

        // annab ui-le uue elude arvu
        EludMuutusid.Invoke(Elud);

        if (Elud == 0 && !mängLäbiKäivitunud)
        {
            // mäng läbi ainult ühe korra
            mängLäbiKäivitunud = true;
            MängLäbi.Invoke();
        }
    }
}
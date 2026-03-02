using UnityEngine;

public class vaenlaseLiikumine : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private Rigidbody2D keha2D;

    [Header("Atribuudid")]
    [SerializeField] private float liikumiskiirus = 3f;

    private Transform sihtpunkt;
    private int teeIndeks = 0;
    private float algneKiirus;
    private SpriteRenderer sr;

    private void Start()
    {
        algneKiirus = liikumiskiirus;
        sihtpunkt = haldur.peamine.teekond[teeIndeks];

        if (keha2D == null)
            keha2D = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Vector2.Distance(sihtpunkt.position, transform.position) <= 0.1f)
        {
            teeIndeks++;
            
            if (teeIndeks == haldur.peamine.teekond.Length)
            {
                if (MängijaElud.Instance != null)
                    MängijaElud.Instance.VõtaElu(1);

                VaenlaseTekitaja.vaenlaseHävitamiseSündmus?.Invoke();

                Destroy(gameObject);
                return;
            }
            else
            {
                sihtpunkt = haldur.peamine.teekond[teeIndeks];
            }
        }

        Liigu();
        UuendaSuunda();
    }

    private void Liigu()
    {
        Vector2 suund = (sihtpunkt.position - transform.position).normalized;
        keha2D.linearVelocity = suund * liikumiskiirus;
    }
    private void UuendaSuunda()
    {
        if (sr == null || keha2D == null) return;

        if (keha2D.linearVelocity.x > 0.01f)
        {
            sr.flipX = false;
        }
        else if (keha2D.linearVelocity.x < -0.01f)
        {
            sr.flipX = true;
        }
    }

    public void UuendaKiirus(float uusKiirus)
    {
        liikumiskiirus = uusKiirus;
    }

    public void TaastaKiirus()
    {
        liikumiskiirus = algneKiirus;
    }
}
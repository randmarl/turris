using UnityEngine;

public class VaenlaseLiikumine : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private Rigidbody2D keha2D;

    [Header("Atribuudid")]
    [SerializeField] private float liikumiskiirus = 3f;
    [SerializeField] private int lõpusVõetavadElud = 1;

    private Transform sihtpunkt;
    private int teeIndeks;
    private float algneKiirus;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        algneKiirus = liikumiskiirus;

        if (keha2D == null)
            keha2D = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (Haldur.Peamine != null && Haldur.Peamine.Teekond.Length > 0)
            sihtpunkt = Haldur.Peamine.Teekond[teeIndeks];
    }

    private void Update()
    {
        if (sihtpunkt == null)
            return;

        if (Vector2.Distance(transform.position, sihtpunkt.position) <= 0.1f)
        {
            // liigub järgmise teepunkti juurde
            teeIndeks++;

            if (Haldur.Peamine == null || teeIndeks >= Haldur.Peamine.Teekond.Length)
            {
                // raja lõppu jõudnud vaenlane võtab elu
                if (MängijaElud.Instance != null)
                    MängijaElud.Instance.VõtaElu(lõpusVõetavadElud);

                VaenlaseTekitaja.vaenlaseHävitamiseSündmus?.Invoke();
                Destroy(gameObject);
                return;
            }

            sihtpunkt = Haldur.Peamine.Teekond[teeIndeks];
        }

        Liigu();
        UuendaSuunda();
    }

    private void Liigu()
    {
        // suund praeguse sihtpunkti poole
        Vector2 suund = (sihtpunkt.position - transform.position).normalized;
        keha2D.linearVelocity = suund * liikumiskiirus;
    }

    private void UuendaSuunda()
    {
        if (spriteRenderer == null || keha2D == null)
            return;

        // pöörab pildi liikumise järgi
        if (keha2D.linearVelocity.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (keha2D.linearVelocity.x < -0.01f)
            spriteRenderer.flipX = true;
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
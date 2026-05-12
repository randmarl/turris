using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Kahur : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private Transform kahuriPöördepunkt;
    [SerializeField] private LayerMask vaenlaskiht;
    [SerializeField] private GameObject kuuliMall;
    [SerializeField] private Transform laskepunkt;

    [Header("Atribuudid")]
    [SerializeField] private float sihtimisraadius = 5f;
    [SerializeField] private float pööramiskiirus = 6f;
    [SerializeField] private float kuuleSekundis = 1f;

    private Transform sihtmärk;
    private float aegJärgmiseLasuni;

    private float tulekiiruseKordaja = 1f;
    private float tulekiiruseParenduseLõpuAeg;

    private void Update()
    {
        UuendaTulekiiruseParendust();

        if (sihtmärk == null)
        {
            LeiaSihtmärk();
            return;
        }

        PööraSihtmärgiSuunas();

        if (!KasSihtmärkOnRaadiuses())
        {
            sihtmärk = null;
            return;
        }

        ProoviTulistada();
    }

    private void UuendaTulekiiruseParendust()
    {
        if (tulekiiruseKordaja <= 1f)
            return;

        if (Time.time < tulekiiruseParenduseLõpuAeg)
            return;

        // tulekiirus tagasi tavaliseks
        tulekiiruseKordaja = 1f;
    }

    public void ParendaTulekiirust(float kordaja, float kestus)
    {
        // jätab tugevama või pikema parenduse alles
        tulekiiruseKordaja = Mathf.Max(tulekiiruseKordaja, kordaja);
        tulekiiruseParenduseLõpuAeg = Mathf.Max(tulekiiruseParenduseLõpuAeg, Time.time + kestus);
    }

    private void LeiaSihtmärk()
    {
        // vaenlased kahuri raadiuses
        RaycastHit2D[] tabamused = Physics2D.CircleCastAll(
            transform.position,
            sihtimisraadius,
            Vector2.zero,
            0f,
            vaenlaskiht
        );

        if (tabamused.Length > 0)
            sihtmärk = tabamused[0].transform;
    }

    private void ProoviTulistada()
    {
        aegJärgmiseLasuni += Time.deltaTime;

        // tulekiirus koos võimaliku parendusega
        float tegelikKuuleSekundis = kuuleSekundis * tulekiiruseKordaja;

        if (aegJärgmiseLasuni < 1f / tegelikKuuleSekundis)
            return;

        Tulista();
        aegJärgmiseLasuni = 0f;
    }

    private void Tulista()
    {
        if (kuuliMall == null || laskepunkt == null)
            return;

        GameObject kuuliObjekt = Instantiate(kuuliMall, laskepunkt.position, Quaternion.identity);

        Collider2D kuuliCollider = kuuliObjekt.GetComponent<Collider2D>();

        if (kuuliCollider != null)
        {
            // kuul ei põrkaks oma kahuriga
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(kuuliCollider, collider);
        }

        Kuul kuul = kuuliObjekt.GetComponent<Kuul>();

        if (kuul != null)
            kuul.MääraSihtmärk(sihtmärk);
    }

    private bool KasSihtmärkOnRaadiuses()
    {
        return Vector2.Distance(transform.position, sihtmärk.position) <= sihtimisraadius;
    }

    private void PööraSihtmärgiSuunas()
    {
        if (kahuriPöördepunkt == null)
            return;

        // kahuri pööramine sihtmärgi suunas
        // allikas: Medium artikkel kahuri pööramisest Unitys
        // link: https://medium.com/@rohan5210work/from-wobbly-angles-to-perfect-aim-how-i-fixed-cannon-rotation-in-unity-2d-0c1dc81f16ff
        Vector2 suund = sihtmärk.position - transform.position;
        float nurk = Mathf.Atan2(suund.y, suund.x) * Mathf.Rad2Deg - 90f;
        Quaternion sihtimispööre = Quaternion.Euler(0f, 0f, nurk);

        // sujuv pööramine
        kahuriPöördepunkt.rotation = Quaternion.RotateTowards(
            kahuriPöördepunkt.rotation,
            sihtimispööre,
            pööramiskiirus * Time.deltaTime
        );
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // raadius editoris nähtavaks
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, sihtimisraadius);
    }
    #endif

    public float VõtaSihtimisRaadius()
    {
        return sihtimisraadius;
    }
}
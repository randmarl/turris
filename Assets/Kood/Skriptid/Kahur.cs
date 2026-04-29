using UnityEngine;
using UnityEditor;

public class Kahur : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private Transform kahuriPöördePunkt;
    [SerializeField] private LayerMask vaenlaseKiht;
    [SerializeField] private GameObject kuuliMall;
    [SerializeField] private Transform laskepunkt;

    [Header("Atribuudid")]
    [SerializeField] private float sihtimisRaadius = 5f;
    [SerializeField] private float pööramisKiirus = 6f;
    [SerializeField] private float kuuleSekundis = 1f;

    private Transform sihtmärk;
    private float aegJärgmiseLasuni;

    private void Update()
    {
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

    private void LeiaSihtmärk()
    {
        RaycastHit2D[] tabamused = Physics2D.CircleCastAll(
            transform.position,
            sihtimisRaadius,
            Vector2.zero,
            0f,
            vaenlaseKiht
        );

        if (tabamused.Length > 0)
            sihtmärk = tabamused[0].transform;
    }

    private void ProoviTulistada()
    {
        aegJärgmiseLasuni += Time.deltaTime;

        if (aegJärgmiseLasuni < 1f / kuuleSekundis)
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
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(kuuliCollider, collider);
        }

        Kuul kuul = kuuliObjekt.GetComponent<Kuul>();

        if (kuul != null)
            kuul.MääraSihtmärk(sihtmärk);
    }

    private bool KasSihtmärkOnRaadiuses()
    {
        return Vector2.Distance(transform.position, sihtmärk.position) <= sihtimisRaadius;
    }

    private void PööraSihtmärgiSuunas()
    {
        if (kahuriPöördePunkt == null)
            return;

        Vector2 suund = sihtmärk.position - transform.position;
        float nurk = Mathf.Atan2(suund.y, suund.x) * Mathf.Rad2Deg - 90f;
        Quaternion sihtimispööre = Quaternion.Euler(0f, 0f, nurk);

        kahuriPöördePunkt.rotation = Quaternion.RotateTowards(
            kahuriPöördePunkt.rotation,
            sihtimispööre,
            pööramisKiirus * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, sihtimisRaadius);
    }

    public float VõtaSihtimisRaadius()
    {
        return sihtimisRaadius;
    }
}
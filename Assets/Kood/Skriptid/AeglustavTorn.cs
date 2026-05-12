using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AeglustavTorn : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private LayerMask vaenlaseKiht;
    [SerializeField] private GameObject külmaEfekt;

    [Header("Atribuudid")]
    [SerializeField] private float sihtimisraadius = 5f;
    [SerializeField] private float rünnakuidSekundis = 4f;
    [SerializeField] private float aeglustuseKestus = 1f;

    private const float AeglustuseKordaja = 0.5f;

    private float aegJärgmiseMõjuni;
    private Coroutine efektiCoroutine;

    private void Start()
    {
        if (külmaEfekt != null)
            külmaEfekt.SetActive(false);
    }

    private void Update()
    {
        // aeg järgmise aeglustuseni
        aegJärgmiseMõjuni += Time.deltaTime;

        if (aegJärgmiseMõjuni < 1f / rünnakuidSekundis)
            return;

        AeglustaVaenlasi();
        aegJärgmiseMõjuni = 0f;
    }

    private void AeglustaVaenlasi()
    {
        // vaenlased torni raadiuses
        RaycastHit2D[] tabamused = Physics2D.CircleCastAll(
            transform.position,
            sihtimisraadius,
            Vector2.zero,
            0f,
            vaenlaseKiht
        );

        bool aeglustusRakendus = false;

        foreach (RaycastHit2D tabamus in tabamused)
        {
            VaenlaseLiikumine liikumine = tabamus.transform.GetComponent<VaenlaseLiikumine>();

            if (liikumine == null)
                continue;

            aeglustusRakendus = true;

            // vaenlase kiirus väiksemaks
            liikumine.UuendaKiirus(AeglustuseKordaja);

            // kiirus hiljem tagasi
            StartCoroutine(TaastaVaenlaseKiirus(liikumine));
        }

        if (!aeglustusRakendus)
            return;

        if (efektiCoroutine != null)
            StopCoroutine(efektiCoroutine);

        // külmaefekti taaskäivitamine
        efektiCoroutine = StartCoroutine(NäitaKülmaEfekti());
    }

    private IEnumerator NäitaKülmaEfekti()
    {
        if (külmaEfekt != null)
            külmaEfekt.SetActive(true);

        yield return new WaitForSeconds(aeglustuseKestus);

        if (külmaEfekt != null)
            külmaEfekt.SetActive(false);

        efektiCoroutine = null;
    }

    private IEnumerator TaastaVaenlaseKiirus(VaenlaseLiikumine liikumine)
    {
        yield return new WaitForSeconds(aeglustuseKestus);

        if (liikumine != null)
            liikumine.TaastaKiirus();
    }

    private void OnDrawGizmosSelected()
    {
        // raadius editoris nähtavaks
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, sihtimisraadius);
    }

    public float VõtaSihtimisRaadius()
    {
        return sihtimisraadius;
    }
}
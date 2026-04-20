using System.Collections;
using UnityEngine;
using UnityEditor;

public class AeglustavTorn : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private LayerMask vaenlaseKiht;
    [SerializeField] private GameObject kylmaEfekt;

    [Header("Atribuudid")]
    [SerializeField] private float sihtimisRaadius = 5f;
    [SerializeField] private float rünnakuidSekundis = 4f;
    [SerializeField] private float aeglustuseKestus = 1f;

    private float aegJärgmiseMõjuni;
    private Coroutine efektiCoroutine;

    private void Start()
    {
        if (kylmaEfekt != null)
            kylmaEfekt.SetActive(false);
    }

    private void Update()
    {
        aegJärgmiseMõjuni += Time.deltaTime;

        if (aegJärgmiseMõjuni >= 1f / rünnakuidSekundis)
        {
            AeglustaVaenlasi();
            aegJärgmiseMõjuni = 0f;
        }
    }

    private void AeglustaVaenlasi()
    {
        RaycastHit2D[] tabamused = Physics2D.CircleCastAll(
            transform.position,
            sihtimisRaadius,
            Vector2.zero,
            0f,
            vaenlaseKiht
        );

        bool aeglustusRakendus = false;

        if (tabamused.Length > 0)
        {
            for (int i = 0; i < tabamused.Length; i++)
            {
                RaycastHit2D tabamus = tabamused[i];
                vaenlaseLiikumine liikumine = tabamus.transform.GetComponent<vaenlaseLiikumine>();

                if (liikumine != null)
                {
                    aeglustusRakendus = true;
                    liikumine.UuendaKiirus(0.5f);
                    StartCoroutine(TaastaVaenlaseKiirus(liikumine));
                }
            }
        }

        if (aeglustusRakendus)
        {
            if (efektiCoroutine != null)
                StopCoroutine(efektiCoroutine);

            efektiCoroutine = StartCoroutine(NaitaKylmaEfekti());
        }
    }

    private IEnumerator NaitaKylmaEfekti()
    {
        if (kylmaEfekt != null)
            kylmaEfekt.SetActive(true);

        yield return new WaitForSeconds(aeglustuseKestus);

        if (kylmaEfekt != null)
            kylmaEfekt.SetActive(false);

        efektiCoroutine = null;
    }

    private IEnumerator TaastaVaenlaseKiirus(vaenlaseLiikumine liikumine)
    {
        yield return new WaitForSeconds(aeglustuseKestus);

        if (liikumine != null)
            liikumine.TaastaKiirus();
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
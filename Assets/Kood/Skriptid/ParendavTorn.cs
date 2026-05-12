using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParendavTorn : MonoBehaviour
{
    [Header("Parenduse seaded")]
    [SerializeField] private float parenduseRaadius = 4f;
    [SerializeField] private float parendusiSekundis = 0.5f;
    [SerializeField] private float parenduseKestus = 3f;
    [SerializeField] private float tulekiiruseKordaja = 2f;

    [Header("Efekt")]
    [SerializeField] private GameObject parenduseEfekt;

    private float aegJärgmiseParenduseni;
    private float efektiLõpuAeg;

    private void Start()
    {
        if (parenduseEfekt != null)
            parenduseEfekt.SetActive(false);
    }

    private void Update()
    {
        aegJärgmiseParenduseni += Time.deltaTime;

        // parendab kindla aja tagant
        if (aegJärgmiseParenduseni >= 1f / parendusiSekundis)
        {
            ParendaLähedalOlevaidTorne();
            aegJärgmiseParenduseni = 0f;
        }

        UuendaEfekti();
    }

    private void ParendaLähedalOlevaidTorne()
    {
        // otsib kõik kahuritüüpi tornid
        Kahur[] kõikKahurid = FindObjectsByType<Kahur>(FindObjectsSortMode.None);
        bool midagiParendati = false;

        foreach (Kahur kahur in kõikKahurid)
        {
            if (kahur == null)
                continue;

            if (kahur.gameObject == gameObject)
                continue;

            float kaugus = Vector2.Distance(transform.position, kahur.transform.position);

            // ainult raadiuses olevad tornid
            if (kaugus > parenduseRaadius)
                continue;

            kahur.ParendaTulekiirust(tulekiiruseKordaja, parenduseKestus);
            midagiParendati = true;
        }

        if (!midagiParendati)
            return;

        // efekt kestab sama kaua kui parendus
        efektiLõpuAeg = Time.time + parenduseKestus;

        if (parenduseEfekt != null)
            parenduseEfekt.SetActive(true);
    }

    private void UuendaEfekti()
    {
        if (parenduseEfekt == null || !parenduseEfekt.activeSelf)
            return;

        if (Time.time >= efektiLõpuAeg)
            parenduseEfekt.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        // raadius editoris nähtavaks
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.forward, parenduseRaadius);
    }

    public float VõtaSihtimisRaadius()
    {
        return parenduseRaadius;
    }
}
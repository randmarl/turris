using System.Collections;
using UnityEngine;
using UnityEditor;

public class AeglustavTorn : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private LayerMask vaenlaseKiht;

    [Header("Atribuudid")]
    [SerializeField] private float sihtimisRaadius = 5f;
    [SerializeField] private float rünnakuidSekundis = 4f;
    [SerializeField] private float aeglustuseKestus = 1f;

    private float aegJärgmiseMõjuni;

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
            (Vector2)transform.position,
            0f,
            vaenlaseKiht
        );

        if (tabamused.Length > 0)
        {
            for (int i = 0; i < tabamused.Length; i++)
            {
                RaycastHit2D tabamus = tabamused[i];

                vaenlaseLiikumine liikumine = tabamus.transform.GetComponent<vaenlaseLiikumine>();
                if (liikumine != null)
                {
                    liikumine.UuendaKiirus(0.5f);
                    StartCoroutine(TaastaVaenlaseKiirus(liikumine));
                }
            }
        }
    }

    private IEnumerator TaastaVaenlaseKiirus(vaenlaseLiikumine liikumine)
    {
        yield return new WaitForSeconds(aeglustuseKestus);
        liikumine.TaastaKiirus();
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, sihtimisRaadius);
    }
}

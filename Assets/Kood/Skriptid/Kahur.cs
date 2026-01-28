using System.Collections;
using System.Collections.Generic;
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

        if (!KontrolliKasSihtmärkRaadiuses())
        {
            sihtmärk = null;
        } else
        {
            aegJärgmiseLasuni += Time.deltaTime;
            if (aegJärgmiseLasuni >= 1f / kuuleSekundis)
            {
                Tulista();
                aegJärgmiseLasuni = 0f;
            }
        }
    }

    private void Tulista()
    {
        GameObject kuuliObjekt = Instantiate(kuuliMall, laskepunkt.position, Quaternion.identity);
        Kuul kuuliSkript = kuuliObjekt.GetComponent<Kuul>();
        kuuliSkript.MääraSihtmärk(sihtmärk);
    }

    private void LeiaSihtmärk()
    {
        RaycastHit2D[] tabamused = Physics2D.CircleCastAll(transform.position, sihtimisRaadius, (Vector2)transform.position, 0f, vaenlaseKiht);
        if (tabamused.Length > 0)
        {
            sihtmärk = tabamused[0].transform;
        }
    }

    private bool KontrolliKasSihtmärkRaadiuses()
    {
        return Vector2.Distance(sihtmärk.position, transform.position) <= sihtimisRaadius;
    }

    private void PööraSihtmärgiSuunas()
    {
        float nurk = Mathf.Atan2(sihtmärk.position.y - transform.position.y, sihtmärk.position.x -
        transform.position.x) * Mathf.Rad2Deg - 90f;
        Quaternion sihtimispööre = Quaternion.Euler(new Vector3(0f, 0f, nurk));
        kahuriPöördePunkt.rotation = Quaternion.RotateTowards(kahuriPöördePunkt.rotation, sihtimispööre, pööramisKiirus * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, sihtimisRaadius);
    }
}

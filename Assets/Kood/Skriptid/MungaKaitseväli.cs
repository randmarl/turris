using System.Collections.Generic;
using UnityEngine;

public class MungaKaitseväli : MonoBehaviour
{
    private static readonly List<MungaKaitseväli> aktiivsedKaitseväljad = new List<MungaKaitseväli>();

    [Header("Kaitse seaded")]
    [SerializeField] private float raadius = 3f;

    [Range(0f, 1f)]
    [SerializeField] private float kahjuKordaja = 0.5f;

    [SerializeField] private bool kaitsebIseennast = false;

    private void OnEnable()
    {
        if (!aktiivsedKaitseväljad.Contains(this))
            aktiivsedKaitseväljad.Add(this);
    }

    private void OnDisable()
    {
        aktiivsedKaitseväljad.Remove(this);
    }

    public static float VõtaKahjuKordaja(Vector2 asukoht, GameObject kaitstavObjekt)
    {
        float parimKordaja = 1f;

        foreach (MungaKaitseväli kaitseväli in aktiivsedKaitseväljad)
        {
            if (kaitseväli == null)
                continue;

            if (!kaitseväli.kaitsebIseennast && kaitseväli.gameObject == kaitstavObjekt)
                continue;

            float kaugus = Vector2.Distance(asukoht, kaitseväli.transform.position);

            if (kaugus > kaitseväli.raadius)
                continue;

            parimKordaja = Mathf.Min(parimKordaja, kaitseväli.kahjuKordaja);
        }

        return parimKordaja;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raadius);
    }
}
using UnityEngine;
using TMPro;

public class OravanahaTekstiUuendaja : MonoBehaviour
{
    [SerializeField] private TMP_Text tekst;
    [SerializeField] private string eesliide = "Oravanahad: ";

    private void Reset()
    {
        tekst = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (tekst == null) tekst = GetComponent<TMP_Text>();

        if (OravanahaHaldur.Instance != null)
        {
            OravanahaHaldur.Instance.OravanahadMuutusid += Uuenda;
            Uuenda(OravanahaHaldur.Instance.Oravanahad);
        }
    }

    private void OnDisable()
    {
        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.OravanahadMuutusid -= Uuenda;
    }

    private void Uuenda(int summa)
    {
        if (tekst != null)
            tekst.text = eesliide + summa;
    }
}
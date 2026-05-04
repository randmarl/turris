using UnityEngine;

public class Tervis : MonoBehaviour
{
    [Header("Atribuudid")]
    [SerializeField] private float elupunktid = 2f;
    [SerializeField] private int väärtus = 50;

    private bool onHävitatud;

    public void Kahjusta(int kahju)
    {
        if (onHävitatud)
            return;

        float kahjuKordaja = MungaKaitseväli.VõtaKahjuKordaja(transform.position, gameObject);
        float tegelikKahju = kahju * kahjuKordaja;

        elupunktid -= tegelikKahju;

        if (elupunktid > 0f)
            return;

        elupunktid = 0f;
        onHävitatud = true;

        VaenlaseTekitaja.vaenlaseHävitamiseSündmus?.Invoke();

        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.LisaOravanahku(väärtus);

        Destroy(gameObject);
    }
}
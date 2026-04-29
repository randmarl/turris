using UnityEngine;

public class Tervis : MonoBehaviour
{
    [Header("Atribuudid")]
    [SerializeField] private int elupunktid = 2;
    [SerializeField] private int väärtus = 50;

    private bool onHävitatud;

    public void Kahjusta(int kahju)
    {
        if (onHävitatud)
            return;

        elupunktid -= kahju;

        if (elupunktid > 0)
            return;

        elupunktid = 0;
        onHävitatud = true;

        VaenlaseTekitaja.vaenlaseHävitamiseSündmus?.Invoke();

        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.LisaOravanahku(väärtus);

        Destroy(gameObject);
    }
}
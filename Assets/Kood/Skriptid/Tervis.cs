using UnityEngine;

public class Tervis : MonoBehaviour
{
    [Header("Atribuudid")]
    [SerializeField] private int elupunktid = 2;
    [SerializeField] private int väärtus = 50;

    private bool onHävitatud = false;

    public void Kahjusta(int kahju)
    {
        elupunktid -= kahju;

        if (elupunktid <= 0 && !onHävitatud)
        {
            VaenlaseTekitaja.vaenlaseHävitamiseSündmus?.Invoke();
            if (haldur.peamine != null) haldur.peamine.LisaRaha(väärtus);
            onHävitatud = true;
            Destroy(gameObject);
        }
    }
}

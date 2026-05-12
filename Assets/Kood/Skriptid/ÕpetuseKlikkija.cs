using UnityEngine;
using UnityEngine.EventSystems;

public class ÕpetuseKlikkija : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ÕpetuseHaldur õpetuseHaldur;

    public void OnPointerClick(PointerEventData eventData)
    {
        // kaitse puuduva viite vastu
        if (õpetuseHaldur == null)
            return;

        õpetuseHaldur.TöötleKlikki();
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

public class ÕpetuseKlikkija : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ÕpetuseHaldur õpetuseHaldur;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (õpetuseHaldur != null)
            õpetuseHaldur.TöötleKlikki();
    }
}
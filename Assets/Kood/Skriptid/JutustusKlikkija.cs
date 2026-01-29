using UnityEngine;
using UnityEngine.EventSystems;

public class JutustusKlikkija : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private JutustuseHaldur jutustuseHaldur;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (jutustuseHaldur != null)
            jutustuseHaldur.TöötleKlikki();
    }
}
using UnityEngine;
using TMPro;

public class Menüü : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private TextMeshProUGUI rahaTekst;
    [SerializeField] private Animator animaator;

    private bool kasMenüüOnAvatud = true;

    public void LülitaMenüü()
    {
        kasMenüüOnAvatud = !kasMenüüOnAvatud;
        animaator.SetBool("MenüüAvatud", kasMenüüOnAvatud);
    }

    private void OnGUI()
    {
        rahaTekst.text = haldur.peamine.raha.ToString();
    }

//    public void MääraValitudTorn()
//    {      
//    }
}

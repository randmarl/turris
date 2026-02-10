using UnityEngine;

public class Ehitaja : MonoBehaviour
{
    public static Ehitaja peamine;

    [Header("Viited")]
    [SerializeField] private Torn[] tornid;

    private int valitudTorn = 0;

    private void Awake()
    {
        peamine = this;
    }

   public Torn VõtaValitudTorn()
   {
       return tornid[valitudTorn];
   }

    public void MääraValitudTorn(int uusValik)
    {
        valitudTorn = uusValik;
    }
}

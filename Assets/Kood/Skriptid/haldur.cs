using UnityEngine;

public class haldur : MonoBehaviour
{
    public static haldur peamine;
    public Transform[] algusPunkt;
    public Transform[] teekond;

    public int raha;

    private void Awake()
    {
        peamine = this;
    }
    private void Start()
    {
        raha = 100;
    }
    public void LisaRaha(int kogus)
    {
        raha += kogus;
    }
    public bool KulutaRaha(int kogus)
    {
        if (kogus <= raha)
        {
            raha -= kogus;
            return true;
        }else
        {
            Debug.Log("Sul pole piisavalt raha selle jaoks.");
            return false;
        }
    }
}

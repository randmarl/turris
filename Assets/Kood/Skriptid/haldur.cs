using UnityEngine;

public class haldur : MonoBehaviour
{
    public static haldur peamine;

    [Header("Rada")]
    public Transform[] algusPunkt;
    public Transform[] teekond;

    private void Awake()
    {
        peamine = this;
    }
}
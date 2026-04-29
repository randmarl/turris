using System;
using UnityEngine;

[Serializable]
public class Torn
{
    public string nimi;
    public int hind;
    public GameObject prefab;

    public Torn(string nimi, int hind, GameObject prefab)
    {
        this.nimi = nimi;
        this.hind = hind;
        this.prefab = prefab;
    }
}
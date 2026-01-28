using System;
using UnityEngine;

[Serializable]
public class Torn
{
    public string nimi;
    public int hind;
    public GameObject prefab;

    public Torn(string _nimi, int _hind, GameObject _prefab)
    {
        nimi = _nimi;
        hind = _hind;
        prefab = _prefab;
    }
}
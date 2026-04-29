using System;
using UnityEngine;

[Serializable]
public class TornideTõenäosused
{
    [SerializeField] private GameObject prefab;
    [SerializeField, Min(0f)] private float kaal = 1f;

    public GameObject Prefab => prefab;
    public float Kaal => kaal;
}
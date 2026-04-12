using UnityEngine;
using System;

[Serializable]
public class TornideTõenäosused
{
    public GameObject prefab;
    [Min(0f)] public float kaal = 1f;
}

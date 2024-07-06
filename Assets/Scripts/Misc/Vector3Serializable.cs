using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vector3Serializable
{
    public float x, y, z;
    public Vector3Serializable(float _x,float _y,float _z)
    {
        x = _x; y = _y; z = _z;
    }

    public Vector3Serializable()
    {

    }

}

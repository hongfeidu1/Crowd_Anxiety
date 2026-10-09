using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorCircle : MonoBehaviour
{
    [Header("Ô²»·²ÎÊý")]
    public float radius = 5.0f;
    public int pointCount = 20;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        GenerateCircle();
    }

    private void GenerateCircle()
    {
        line.positionCount = pointCount;

        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            float rad = 2 * Mathf.PI * i / pointCount;
            float x = radius * Mathf.Cos(rad);
            float y = radius * Mathf.Sin(rad);

            points[i] = new Vector3(x, y, 0);
        }

        line.SetPositions(points);
    }
}

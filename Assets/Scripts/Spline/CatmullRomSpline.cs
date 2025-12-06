using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CatmullRomSpline
{
    public static Vector3[] CreateCatmullRomSpline(GameObject[] controlPoints, float tIncrement, float infulence)
    { 
        
        Vector3 P0 = controlPoints[2].transform.position;
        Vector3 P1 = controlPoints[0].transform.position;
        Vector3 P2 = controlPoints[1].transform.position;
        Vector3 P3 = controlPoints[3].transform.position;

        float t;

        int segments = controlPoints.Length - 3;


        //Vector3[] interpolatedPositions = new Vector3[Mathf.FloorToInt((1 / tIncrement) * segments) ];
        int numPoints = Mathf.CeilToInt((1 / tIncrement) * segments) + 1;
        Vector3[] interpolatedPositions = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {

            t = i * tIncrement; 

            interpolatedPositions[i] = GetPoint(t, P0, P1, P2, P3, infulence);
            //Debug.Log($"i={i}, t={t}, P0={P0}, P1={P1}, P2={P2}, P3={P3}, Position={interpolatedPositions[i]}");
        }



        return interpolatedPositions;

    }

   

    public static Vector3 GetPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float influence)
    {


        
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 T1 = (p2 - p0) * influence; // Tangent at P1
        Vector3 T2 = (p3 - p1) * influence; // Tangent at P2


        // CatmullRom spline equation from wiki

        //Vector3 point = 0.5f * ((2f * p1) +(-p0 + p2) * t +(2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +(-p0 + 3f * p1 - 3f * p2 + p3) * t3);

        Vector3 point = (2 * t3 - 3 * t2 + 1) * p1 +
           (t3 - 2 * t2 + t) * T1 +
           (-2 * t3 + 3 * t2) * p2 +
           (t3 - t2) * T2;
        
        return point;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLineRenderer : MonoBehaviour {

    public int segments;
    public float xradius;
    private float yradius;
    LineRenderer line;

    public void ConstructCircle()
    {
        line = gameObject.GetComponent<LineRenderer>();

        yradius = xradius;
        line.positionCount = segments + 1;
        line.useWorldSpace = false;

        float x;
        float y;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }
}

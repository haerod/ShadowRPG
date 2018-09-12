using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFrontCamera : MonoBehaviour
{
    void Update()
    {
        Vector3 v = Camera.main.transform.position - transform.position;
        v.y = v.z = 0.0f; // Trifouiller entre v.y, v.x et v.z pour changer de résulta si besoin
        transform.LookAt(Camera.main.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}

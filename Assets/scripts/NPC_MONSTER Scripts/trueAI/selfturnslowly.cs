using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfturnslowly : MonoBehaviour {

    // Use this for initialization
    public Space m_RotateSpace;
    public float m_RotateSpeed = 30f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * m_RotateSpeed * Time.deltaTime, m_RotateSpace);
    }
}

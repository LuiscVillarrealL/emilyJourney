using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSignal : MonoBehaviour
{

    public float signalRotationSpeed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * signalRotationSpeed * Time.deltaTime * -1);
        //transform.Rotate(signalRotationSpeed * Time.deltaTime, 0, 0);
    }
}

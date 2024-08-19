using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{

    public float thrust = 40.0f;
    public Rigidbody CarRb;

    // Start is called before the first frame update
    void Start()
    {
        CarRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       CarRb.AddForce(transform.forward * thrust * Time.deltaTime, ForceMode.Impulse);
    }
}

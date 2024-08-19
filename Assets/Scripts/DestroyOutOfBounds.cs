using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    public float topBound = 333;
    public float BottomBound = -333;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > topBound)
        {
            Destroy(gameObject);

        }
        else if (transform.position.x < BottomBound)
        {
            Destroy(gameObject);
        }
        
    }
}

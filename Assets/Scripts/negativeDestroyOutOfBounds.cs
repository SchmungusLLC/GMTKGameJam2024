using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class negativeDestroyOutOfBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -333)
        {
            Destroy(gameObject);
        }
    }
}

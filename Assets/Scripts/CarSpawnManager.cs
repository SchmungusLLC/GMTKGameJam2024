using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnManager : MonoBehaviour
{
    public GameObject[] CarsPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnCars", 2, 5.5f);
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.S))
        {
            Instantiate(CarsPrefabs[0], new Vector3(-36,3,66), CarsPrefabs[0].transform.rotation);
            Instantiate(CarsPrefabs[1], new Vector3(30,3,85), CarsPrefabs[1].transform.rotation);
        }
        */
    }

    void SpawnCars()
    {
         Instantiate(CarsPrefabs[0], new Vector3(-36,1,66), CarsPrefabs[0].transform.rotation);
            Instantiate(CarsPrefabs[1], new Vector3(50,1,85), CarsPrefabs[1].transform.rotation);
    }
}

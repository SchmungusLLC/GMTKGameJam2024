using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnManager : MonoBehaviour
{
    public GameObject[] CarsPrefabs;

    public float spawnTime;
    public float spawnLowThreshold;
    public float spawnHighThreshold;

    public float nextSpawnTime;
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = 0;
        SpawnCars();
    }

    // Update is called once per frame
    void Update()
    {
       spawnTime += Time.deltaTime;

        if (spawnTime > nextSpawnTime )
        {
            SpawnCars();
        }
    }

    void SpawnCars()
    {
        switch (Random.Range(0,3))
        {
            case 0:
                Instantiate(CarsPrefabs[0], new Vector3(-36, 0, 66), CarsPrefabs[0].transform.rotation);
                break;
            case 1:
                Instantiate(CarsPrefabs[1], new Vector3(50, 0, 85), CarsPrefabs[1].transform.rotation);
                break;
            case 2:
                Instantiate(CarsPrefabs[1], new Vector3(50, 0, 85), CarsPrefabs[1].transform.rotation);
                Instantiate(CarsPrefabs[0], new Vector3(-36, 0, 66), CarsPrefabs[0].transform.rotation);
                break;
        }

        spawnTime = 0;
        nextSpawnTime = Random.Range(spawnLowThreshold, spawnHighThreshold);
    }
}

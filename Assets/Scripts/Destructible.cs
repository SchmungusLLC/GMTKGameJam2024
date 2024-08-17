using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [Tooltip("Number of hits required to destroy this object")]
    [SerializeField] int hitMax;
    // the number of hits sustained
    int hits;
    private void Awake()
    {
        hits = 0;
    }

    public void Struck(Rigidbody attacker, char attackDir)
    {
        hits++;
        if (hits == hitMax)
        {
            gameObject.SetActive(false);
        }
    }
}

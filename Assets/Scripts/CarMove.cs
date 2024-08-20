using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{

    public float thrust = 40.0f;
    public float ImpactForce = 300f;
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

     private void OnCollisionEnter(Collision collision)
    {
        float randomAngle = Random.Range(-45, 45);

       if (collision.gameObject.TryGetComponent(out Player player))
       {
            player.rb3D.AddForce(transform.forward * ImpactForce, ForceMode.Impulse);
       }
       else if (collision.gameObject.TryGetComponent(out Enemy enemy))
       {
            enemy.rb3D.AddForce(transform.forward * ImpactForce, ForceMode.Impulse);
       }
       else if (collision.gameObject.TryGetComponent(out Destructible destructible))
       {
            destructible.rb3D.AddForce(transform.forward * ImpactForce, ForceMode.Impulse);
       }
    }
}

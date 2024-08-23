using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{

    public float moveSpeed; // Speed of the car
    public float pauseDuration; // Time to wait after collision
    public bool isPaused;

    public Animator carAnimator;
    public float ImpactForce;
    public Rigidbody CarRb;
    public BoxCollider boxCollider;

    public LayerMask crashColliders;

    // Start is called before the first frame update
    void Start()
    {
        CarRb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            MoveForward();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (crashColliders.ContainsLayer(collision.gameObject.layer))
        {
            StartCoroutine(PauseMovement());
            carAnimator.SetTrigger("Honk");
            AudioManager._AudioManger.Play("honk");
        }
    }

    void MoveForward()
    {
        CarRb.AddForce(transform.forward * moveSpeed);
    }

    IEnumerator PauseMovement()
    {
        isPaused = true;
        boxCollider.enabled = false;
        CarRb.velocity = Vector3.zero; // Stop the car immediately
        yield return new WaitForSeconds(pauseDuration); // Wait for the specified duration
        isPaused = false;
    }
}

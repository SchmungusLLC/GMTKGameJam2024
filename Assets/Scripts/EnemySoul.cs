using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class EnemySoul : MonoBehaviour
{
    public Animator animator;
    public Enemy attachedEnemy;

    public bool isSoulMoving;

    public AttackState attackState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attachedEnemy = GetComponentInParent<Enemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSoulMoving)
        {
            MoveSoulToTarget();
        }
    }

    public void Appear(AttackState lastAttackState)
    {
        attackState = lastAttackState;

        gameObject.SetActive(true);
        gameObject.transform.SetParent(null);
        gameObject.transform.eulerAngles = player.cameraFaceDir;
        //Debug.Log("Enemy attack state = " + attackState);
        if (attackState == AttackState.SmallAttack)
        {
            animator.Play("LightSoulAppear", 0);
            animator.Play("LightSoulFlicker", 1);
        }
        else
        {
            animator.Play("HeavySoulAppear", 0);
            animator.Play("HeavySoulFlicker", 1);
        }
    }

    public void StartSoulMoving()
    {
        isSoulMoving = true;
    }

    public void MoveSoulToTarget()
    {
        // Get the bottom-right corner of the screen in world space
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(player.lightSlider.transform.position);
        //targetPosition.z = transform.position.z;

        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, 200 * Time.deltaTime);

        // Check if the sprite has reached the target position
        if (Vector3.Distance(gameObject.transform.position, targetPosition) < 0.1f)
        {
            isSoulMoving = false;
            //animator.StopPlayback();
            if (attackState == AttackState.SmallAttack)
            {
                ChargePlayerLight();
            }
            else
            {
                ChargePlayerHeavy();
            }

            gameObject.SetActive(false);
        }
    }

    public void ChargePlayerLight()
    {
        player.AddLightUltimateCharge(0.1f);
        player.AddScalesValue(-1);
    }

    public void ChargePlayerHeavy()
    {
        player.AddHeavyUltimateCharge(0.1f);
        player.AddScalesValue(1);
    }
}

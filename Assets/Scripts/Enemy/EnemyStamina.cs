using UnityEngine;

public partial class Enemy
// ============================================================
//                      ENEMY STAMINA
// ============================================================
{
    [Header("Stamina Stats")]
    [Tooltip("Total Number of Stamina Points (SP)")]
    [SerializeField] float maxSP;
    [Tooltip("Number of SP regenerated per second")]
    [SerializeField] float SPRegenRate;
    [Tooltip("Time in seconds after last expending SP before regen starts")]
    [SerializeField] float SPRegenDelay;

    // current SP value
    private float currentSP;
    // float for current delay time 
    private float SPDelayTime;
    // bool to check if player is currently regaining SP
    private bool isRegenSP;

    bool SpendSP(float SPSpent)
    {
        if (currentSP < SPSpent)
        {
            Debug.Log("Not enough SP!");
            return false;
        }
        currentSP -= SPSpent;
        SPBar.value = currentSP;
        isRegenSP = false;
        SPDelayTime = 0f;
        return true;
    }

    void RegenSP()
    {
        currentSP += SPRegenRate * Time.deltaTime;
        if (currentSP >= maxSP)
        {
            currentSP = maxSP;
            isRegenSP = false;
        }
        SPBar.value = currentSP;
    }

    void SPTimer()
    {
        SPDelayTime += Time.deltaTime;
        if (SPDelayTime > SPRegenDelay)
        {
            isRegenSP = true;
        }
    }
}
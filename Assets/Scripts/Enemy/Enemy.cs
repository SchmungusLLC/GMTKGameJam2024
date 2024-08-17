using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Enemy : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Canvas GameObject holding player's in-world UI elements")]
    [SerializeField] GameObject playerWorldCanvas;
    [Tooltip("UI Slider for SP Points")]
    [SerializeField] Slider SPBar;
    [Tooltip("Transform of the player's SP bar (used to lock rotation)")]
    [SerializeField] Transform SPBarTransform;

    // Start is called before the first frame update
    void Start()
    {
        SPBar.maxValue = currentSP = maxSP;
        SPBar.value = currentSP;
        isRegenSP = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRegenSP) { RegenSP(); }
        else if (currentSP != maxSP) { SPTimer(); }
    }
}

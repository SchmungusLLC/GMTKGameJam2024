using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MatOverride : MonoBehaviour
{
     public Texture newTexture;

    void Start()
    {
        ApplyTexture();
    }

    // Called whenever a value is changed in the inspector
    void OnValidate()
    {
        ApplyTexture();
    }

    void ApplyTexture()
    {
        if (newTexture == null) return;

        Renderer renderer = GetComponent<Renderer>();

        // Ensure we don't modify the shared material directly in the editor
        if (Application.isPlaying)
        {
            renderer.material.SetTexture("_MainTex", newTexture);
        }
        else
        {
            // Use the sharedMaterial property in the editor to prevent instance changes
            renderer.sharedMaterial.SetTexture("_MainTex", newTexture);
            // string materialPath = "/C:/Users/Michael/Documents/Github/GMTKGameJam2024/Assets/MatOverride.mat";
            // Material newMaterial = new Material(Shader.Find("Standard"));
            // newMaterial.SetTexture("_MainTex", newTexture);
            // AssetDatabase.CreateAsset(newMaterial, materialPath);
            // AssetDatabase.SaveAssets();
            // renderer.sharedMaterial = newMaterial;
        }
    }
}

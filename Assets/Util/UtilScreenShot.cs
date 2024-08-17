using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;

public class UtilScreenShot : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            string screenshotName = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            string screenshotPath = Application.dataPath + "/Debug/ScreenShots/" + screenshotName;
            ScreenCapture.CaptureScreenshot(screenshotPath);
            Debug.Log("Screenshot captured: " + screenshotPath);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            TakeLevelSizeScreenShot();
        }
#endif
    }

    public static void TakeLevelSizeScreenShot()
    {
        Camera activeCamera = Camera.main;
        Cinemachine.CinemachineBrain cinemachineBrain = activeCamera.GetComponent<Cinemachine.CinemachineBrain>();
        cinemachineBrain.enabled = false;

        PixelPerfectCamera pixelPerfectCamera = activeCamera.GetComponent<PixelPerfectCamera>();
        Vector4 worldBounds = GetWorldBounds();
        float width = Mathf.Abs(worldBounds.y - worldBounds.x);
        float height = Mathf.Abs(worldBounds.z - worldBounds.w);

        pixelPerfectCamera.refResolutionX = Mathf.RoundToInt(width * 32);
        pixelPerfectCamera.refResolutionY = Mathf.RoundToInt(height * 32);

        // Set the render resolution to 800x800
        activeCamera.targetTexture = new RenderTexture(pixelPerfectCamera.refResolutionX, pixelPerfectCamera.refResolutionY, 32);

        //set camera to the center of the bounds
        activeCamera.transform.position = new Vector3(worldBounds.x + width / 2, worldBounds.w + height / 2, activeCamera.transform.position.z);

        //make the background transparent
        CameraClearFlags originalClearFlags = activeCamera.clearFlags;
        activeCamera.clearFlags = CameraClearFlags.SolidColor;
        activeCamera.backgroundColor = new Color(0, 0, 0, 0);


        TilemapRenderer[] tilemapRenderers = FindObjectsOfType<TilemapRenderer>();
        foreach (TilemapRenderer tilemapRenderer in tilemapRenderers)
        {
            tilemapRenderer.enabled = false;
        }
        string timeString = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        System.IO.Directory.CreateDirectory(Application.dataPath + "/Debug/ScreenShots/" + "full-screenshot-" + timeString);
        for (int i = 0; i < tilemapRenderers.Length; i++)
        {
            TilemapRenderer tilemapRenderer = tilemapRenderers[i];
            tilemapRenderer.enabled = true;
            //save to file
            string screenshotName = "Layer_" + i + '_' + timeString + ".png";
            //create folder
            
            string screenshotPath = Application.dataPath + "/Debug/ScreenShots/" + "full-screenshot-" + timeString + '/' + i + ".png";
            RenderToFile(activeCamera, screenshotPath);
            Debug.Log("Layer " + i + "captured: " + screenshotPath);
            tilemapRenderer.enabled = false;
        }
        foreach (TilemapRenderer tilemapRenderer in tilemapRenderers)
        {
            tilemapRenderer.enabled = true;
        }
            
        string screenshotPath2 = Application.dataPath + "/Debug/ScreenShots/" + "full-screenshot-" + timeString + '/' + "_complete" + ".png";
        RenderToFile(activeCamera, screenshotPath2);

        activeCamera.clearFlags = originalClearFlags;




    }

    private static void RenderToFile(Camera activeCamera, string screenshotPath)
    {

        // Render the camera
        activeCamera.Render();



        // output the render texture to a file
        RenderTexture.active = activeCamera.targetTexture;
        Texture2D tex = new Texture2D(activeCamera.targetTexture.width, activeCamera.targetTexture.height);
        tex.ReadPixels(new Rect(0, 0, activeCamera.targetTexture.width, activeCamera.targetTexture.height), 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(screenshotPath, bytes);
    }

    private static Vector4 GetWorldBounds()
    {
        SpriteRenderer[] spriteRenderers = FindObjectsOfType<SpriteRenderer>();

        if (spriteRenderers.Length == 0)
        {
            Debug.LogWarning("No SpriteRenderers found in the scene.");
            return Vector4.zero;
        }

        float farthestLeft = float.MaxValue;
        float farthestRight = float.MinValue;
        float highest = float.MinValue;
        float lowest = float.MaxValue;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Bounds bounds = spriteRenderer.bounds;

            if (bounds.min.x < farthestLeft)
            {
                farthestLeft = bounds.min.x;
            }

            if (bounds.max.x > farthestRight)
            {
                farthestRight = bounds.max.x;
            }

            if (bounds.max.y > highest)
            {
                highest = bounds.max.y;
            }

            if (bounds.min.y < lowest)
            {
                lowest = bounds.min.y;
            }
        }

        TilemapRenderer[] tilemapRenderers = FindObjectsOfType<TilemapRenderer>();

        foreach (TilemapRenderer tilemapRenderer in tilemapRenderers)
        {
            Bounds bounds = tilemapRenderer.bounds;

            if (bounds.min.x < farthestLeft)
            {
                farthestLeft = bounds.min.x;
            }

            if (bounds.max.x > farthestRight)
            {
                farthestRight = bounds.max.x;
            }

            if (bounds.max.y > highest)
            {
                highest = bounds.max.y;
            }

            if (bounds.min.y < lowest)
            {
                lowest = bounds.min.y;
            }
        }

        return new Vector4(farthestLeft, farthestRight, highest, lowest);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarklessAR : MonoBehaviour
{
    //Gyro
    private Gyroscope gyro;
    private GameObject cameraContainer;
    private Quaternion rotation;

    // Cam
    private WebCamTexture cam;
    public RawImage background;
    public AspectRatioFitter fit;


    private bool arReady = false;

    private void Start()
    {
        // Check if we support both services
        // Gyroscope
        if(!SystemInfo.supportsGyroscope)
        {
            Debug.Log("This device does not have a gyroscope");
            return;
        }

        // Back Camera
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            if(!WebCamTexture.devices[i].isFrontFacing)
            {
                cam = new WebCamTexture(WebCamTexture.devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        // If we cannot find a back camera, exit
        if (cam == null)
        {
            Debug.Log("this device dows not have a gyroscope");
            return;
        }

        // Both services are supported, let's enable them!
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        gyro = Input.gyro;
        gyro.enabled = true;
        cameraContainer.transform.rotation = Quaternion.Euler(90f, 0, 0);
        rotation = new Quaternion(0, 0, 1, 0);


        cam.Play();
        background.texture = cam;

        arReady = true;
    }

    private void Update()
    {
        if (arReady)
        {
            // Update Camera
            float ratio = (float)cam.width / (float)cam.height;
            fit.aspectRatio = ratio;

            float scaleY = cam.videoVerticallyMirrored ? -1.0f : 1.0f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -cam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

            //Update Gyro
            transform.localRotation = gyro.attitude * rotation;

        }
    }
}

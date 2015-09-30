using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class StereoscopyController_Base : MonoBehaviour
{
    public enum Mode { Interlaced, SBS };
    [Tooltip("Currently selected sterioscopy mode. There are two modes implemented: Horizontal interlaced and Side by side.")]
    public Mode stereoMode = Mode.SBS;
    [Tooltip("This value is the distance between cameras. This value symbolises the distance between the user eyes in the virtual world.")]
    public float camDistance = 0.04f; //6.5cm for a different pair (via their site) / 8...
    [Tooltip("The focus distance is the distance of the focus points for the cameras. The focus point is determined by multiplying the focusDistance to the forward vercor of the camera")]
    public float focusDistance = 10f; //used for camera lookat point...(from center forward vector)
    [Tooltip("This is a reference to the material used to combine the multiscopic views.")]
    public Material mat;
    [HideInInspector]
    public GameObject leftCam;     // the camera for the left eye
    [HideInInspector]
    public GameObject rightCam;    // the camera for the right eye
    [HideInInspector]
    public RenderTexture leftCamRendTex;    // the render texture for the left camera
    [HideInInspector]
    public RenderTexture rightCamRendTex;   // the render texture for the right camera

    void Start()
    {
        CustomStart();      // since this is a based class reused for the multiscopic rendering optimisation the implementation of custom virtual Start, Update and OnRenderImage functions was required. 
    }

    void Update()
    {
        CustomUpdate();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CustomOnRenderImage(source, destination);
    }

    public virtual void CustomStart()
    {
        //generate the two cameras and adjust their position, rotation and hierarchy
        //generate left cam:
        leftCam = new GameObject("leftCam");
        leftCam.transform.parent = gameObject.transform;
        leftCam.transform.localPosition = Vector3.zero;
        leftCam.transform.localRotation = Quaternion.identity;
        leftCam.AddComponent<Camera>();

        switch (stereoMode)      // set appropriate aspect ratio based on the mode. 
        {
            case Mode.Interlaced:
                leftCam.GetComponent<Camera>().aspect = (float)Screen.width / (float)Screen.height;
                break;
            case Mode.SBS:
                leftCam.GetComponent<Camera>().aspect = (float)Screen.width / 2 / (float)Screen.height;
                break;
        }

        leftCamRendTex = new RenderTexture((int)((Screen.width) / 2), (int)(Screen.height), 24);     // created the render texture for the camera
        leftCam.GetComponent<Camera>().targetTexture = leftCamRendTex;      // assign the render texture to the camera
        leftCam.GetComponent<Camera>().depth = 1;       // set the camera depth, but it's better to set it.
        leftCam.GetComponent<Camera>().cullingMask = GetComponent<Camera>().cullingMask;    // the camera culling mask is set to be the same as the one attached to this game object.
        leftCam.GetComponent<Camera>().nearClipPlane = GetComponent<Camera>().nearClipPlane;    // the new camera is set to have the same culling frustum as the camera attached to this game object
        leftCam.GetComponent<Camera>().farClipPlane = GetComponent<Camera>().farClipPlane;

        //generate right cam:
        rightCam = new GameObject("rightCam");
        rightCam.transform.parent = gameObject.transform;
        rightCam.transform.localPosition = Vector3.zero;
        rightCam.transform.localRotation = Quaternion.identity;
        rightCam.AddComponent<Camera>();

        switch (stereoMode)
        {
            case Mode.Interlaced:
                rightCam.GetComponent<Camera>().aspect = (float)Screen.width / (float)Screen.height;
                break;
            case Mode.SBS:
                rightCam.GetComponent<Camera>().aspect = (float)Screen.width / 2 / (float)Screen.height;
                break;
        }

        rightCamRendTex = new RenderTexture((int)((Screen.width) / 2), (int)(Screen.height), 24);
        rightCam.GetComponent<Camera>().targetTexture = rightCamRendTex;
        rightCam.GetComponent<Camera>().depth = 2;
        rightCam.GetComponent<Camera>().cullingMask = GetComponent<Camera>().cullingMask;
        rightCam.GetComponent<Camera>().nearClipPlane = GetComponent<Camera>().nearClipPlane;
        rightCam.GetComponent<Camera>().farClipPlane = GetComponent<Camera>().farClipPlane;

        GetComponent<Camera>().cullingMask = 0; // after the new cameras are created this camera is set to render nothing.
        GetComponent<Camera>().backgroundColor = new Color(0, 0, 0, 0);
        GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
        GetComponent<Camera>().depth = 0;

        mat.SetTexture("_LeftTex", leftCamRendTex);      // supply the scene render textures to the shader
        mat.SetTexture("_RightTex", rightCamRendTex);
        mat.SetFloat("_WidthRes", Screen.width);         // only the width of the application is needed in the shader. It is required to determine the pixel location of each texel for the interlacing.

        RefreshCameras();       // it sets the cameras appropriately
        mat.SetPass(0);
    }

    public virtual void CustomUpdate()
    {
        //RefreshCameras(); //This is slow and should only be used when tweaking cam values 
    }

    public virtual void ChangeCamDistance(float _camDistance)
    {
        leftCam.transform.localPosition = new Vector3(-(_camDistance / 2f), 0, 0);    // set the positions of the two cameras
        rightCam.transform.localPosition = new Vector3((_camDistance / 2f), 0, 0);
    }

    public virtual void ChangeFocus(float _focusDistance)
    {
        Vector3 tempFocusVector = transform.forward * _focusDistance;       // set the rotation of the two cameras. They 'look' at a point in from of this game object.
        leftCam.transform.LookAt(tempFocusVector);
        rightCam.transform.LookAt(tempFocusVector);
    }

    void ChangeFOV(float _fieldOfView)      // set the field of view of the cameras
    {
        leftCam.GetComponent<Camera>().fieldOfView = _fieldOfView;
        rightCam.GetComponent<Camera>().fieldOfView = _fieldOfView;
    }

    public void RefreshCameras()
    {
        ChangeCamDistance(camDistance);
        ChangeFocus(focusDistance);
        ChangeFOV(GetComponent<Camera>().fieldOfView);
    }

    public virtual void CustomOnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}


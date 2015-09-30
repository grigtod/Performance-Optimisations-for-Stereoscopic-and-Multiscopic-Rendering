using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class MultiscopyController_Base : MonoBehaviour
{
    [Tooltip("This value is the distance between cameras. This value symbolises the distance between the user eyes in the virtual world.")]
    public float camDistance = 0.04f;
    [Tooltip("The focus distance is the distance of the focus points for the cameras. The focus point is determined by multiplying the focusDistance to the forward vercor of the camera")]
    public float focusDistance = 10f;
    [Tooltip("This is a reference to the material used to combine the multiscopic views.")]
    public Material mat;
    [HideInInspector]
    public List<GameObject> camerasGO = new List<GameObject>();     // a list of all the multiscopic cameras.
    [HideInInspector]
    public List<RenderTexture> camRendTexture = new List<RenderTexture>();      // a list of all the render textures from multiscopic cameras.

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
        camDistance = Mathf.Clamp(camDistance, -100f, 100f);    // The distance between the cameras can be smaller than zero. It flips the cameras horizontally. It is not realistic to have a high value. 

        for (int i = 0; i < 8; i++)     // sets up all of the camera appropriately. 
        {
            camerasGO.Add(new GameObject("CamGO " + i));    // CamGo stands for cameraGameObject
            camerasGO[i].transform.parent = gameObject.transform;   // the created game object becomes a child (in the hierarchy) of this game object (the game object this script is attached to).
            camerasGO[i].transform.localPosition = Vector3.zero;    // the rotation and position are set to zero. The child camera game object will have the same position and rotation in the scene as the parent.
            camerasGO[i].transform.localRotation = Quaternion.identity;
            camerasGO[i].AddComponent<Camera>();    // a camera component is added to the camera game object that was just created
            camRendTexture.Add(new RenderTexture((int)((Screen.width * 3) / 8), (int)(Screen.height / 3), 24));      // a render texture for the multiscopic camera is created.
            camerasGO[i].GetComponent<Camera>().targetTexture = camRendTexture[i];      // the target texture of the camera.
            camerasGO[i].GetComponent<Camera>().depth = -((1 * i) + 1);     // each camera has a different rendering depth. This is not required but it's set up just in case.
            camerasGO[i].GetComponent<Camera>().cullingMask = GetComponent<Camera>().cullingMask;   // the camera culling mask is set to be the same as the one attached to this game object.
            camerasGO[i].GetComponent<Camera>().nearClipPlane = GetComponent<Camera>().nearClipPlane;   // the new camera is set to have the same culling frustum as the camera attached to this game object
            camerasGO[i].GetComponent<Camera>().farClipPlane = GetComponent<Camera>().farClipPlane;
        }

        GetComponent<Camera>().cullingMask = 0; // after the new cameras are created this camera is set to render nothing.
        GetComponent<Camera>().backgroundColor = new Color(0, 0, 0, 0);
        GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
        GetComponent<Camera>().depth = 0;

        for (int i = 0; i < 8; i++)     // supply all the render texture to the shader
        {
            mat.SetTexture("_ViewTex" + (i + 1), camRendTexture[i]);
        }
        mat.SetInt("_WidthRes", Screen.width);      // supply the scene resolution to the shader
        mat.SetInt("_HeightRes", Screen.height);
        mat.SetFloat("_multiplX", 1.0f / (float)((Screen.width * 3) / 8));    // this is determine in the shader. It is effectively pixel to texel ratio
        mat.SetFloat("_multiplY", 1.0f / (float)(Screen.height));
        RefreshCameras();   // it sets the cameras appropriately
    }

    public virtual void CustomUpdate()
    {
        //RefreshCameras(); //This is slow and should only be used when tweaking cam values. It is very useful for testing.  
    }

    public virtual void ChangeCamDistance(float _camDistance)
    {
        for (int i = 0; i < 8; i++)
        {
            camerasGO[i].transform.localPosition = new Vector3(-(_camDistance * 3) + (_camDistance * i), 0, 0);     // properly distribute the 8 cameras so there are 4 for earch side of the center
        }
    }

    public virtual void ChangeFocus(float _focusDistance)
    {
        Vector3 tempFocusVector = transform.forward * _focusDistance;       // the camera focus point is determined
        for (int i = 0; i < 8; i++)
        {
            camerasGO[i].transform.LookAt(tempFocusVector);     // all the cameras are set to look at the focus point
        }
    }

    void ChangeFOV(float _fieldOfView)
    {
        for (int i = 0; i < 8; i++)
        {
            camerasGO[i].GetComponent<Camera>().fieldOfView = _fieldOfView; // the new field of view is set for all of the cameras
        }
    }

    public void RefreshCameras()    // this function refreshes all the cameras. Calling this function every frame will reduce performance, it should only be called when the settings of the cameras are changed (via gui etc).
    {
        ChangeCamDistance(camDistance);
        ChangeFocus(focusDistance);
        ChangeFOV(GetComponent<Camera>().fieldOfView);
    }

    public virtual void CustomOnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);   // draw the shader pass to the screen
    }


}


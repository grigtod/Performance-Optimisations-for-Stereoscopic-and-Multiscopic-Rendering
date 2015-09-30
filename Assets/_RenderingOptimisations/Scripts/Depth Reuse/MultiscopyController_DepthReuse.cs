using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MultiscopyController_DepthReuse : MultiscopyController_Base // the code for standard multiscopic camera is reused
{
    [Tooltip("The resolution multiplier for the depth camera. This value is multiplied by the resolution of the application to determine the resolution of the depth render texture.")]
    public float depthCamResolutionMultiplier = 1f;
    [Tooltip("The depth system splits the camera frustum in two. This is the border between the two frustums. They could overlap.")]
    public float nearFarBorder = 80;
    [Tooltip("The depth system splits the camera frustum in two. This is the border between the two frustums. They could overlap.")]
    public float depthCamNearClipPlane = 60;
    [Tooltip("The layer mask of the depth camera. In order to increase performance you can set the depth camera to render the low resolution version of game geometry.")]
    public LayerMask depthCamLayerMask;

    private GameObject depthCam;    // a reference to the depth camera game object
    private RenderTexture depthRendTex;     // a reference to the render texture of the depth camera

    public override void CustomStart()
    {
        depthCamResolutionMultiplier = Mathf.Clamp(depthCamResolutionMultiplier, 0.02f, 10f);   // the depth resolution cannot be smaller than zero. It is also not advisable to be a very large number.
        nearFarBorder = Mathf.Clamp(nearFarBorder, 0.02f, Mathf.Infinity);      // the border between the two frustums should not be smaller than zero.
        depthCamNearClipPlane = Mathf.Clamp(depthCamNearClipPlane, 0.02f, Mathf.Infinity);
        depthRendTex = new RenderTexture((int)((depthCamResolutionMultiplier * Screen.width * 3) / 8f), (int)(depthCamResolutionMultiplier * Screen.height / 3f), 24);    // the depth camera is created and set up in a similar manner to the other multiscopic cameras
        depthCam = new GameObject("DepthCam ");
        depthCam.transform.parent = gameObject.transform;
        depthCam.transform.localPosition = Vector3.zero;
        depthCam.transform.localRotation = Quaternion.identity;
        depthCam.AddComponent<Camera>();
        depthCam.GetComponent<Camera>().fieldOfView = GetComponent<Camera>().fieldOfView;
        depthCam.GetComponent<Camera>().cullingMask = (int)depthCamLayerMask;       // the custom culling mask is assigned. 
        depthCam.GetComponent<Camera>().targetTexture = depthRendTex;
        depthCam.GetComponent<Camera>().nearClipPlane = depthCamNearClipPlane;      //the custom clipping plane is assigned
        depthCam.GetComponent<Camera>().useOcclusionCulling = false;
        base.CustomStart();
        for (int i = 0; i < 8; i++)     // for depth optimisation all the other cameras require small changes
        {
            camerasGO[i].GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camerasGO[i].GetComponent<Camera>().backgroundColor = new Color(255, 255, 255, 0);
            camerasGO[i].GetComponent<Camera>().farClipPlane = nearFarBorder;
            camerasGO[i].GetComponent<Camera>().nearClipPlane = GetComponent<Camera>().nearClipPlane;
        }
        mat.SetTexture("_FarDepthTex", depthRendTex);   // the material supplied is different from the material used for the standard multiscopic rendering. This material can receive additional depth texture.
        ChangeFocus(focusDistance); // set the focus distance of the depth camera
    }

    public override void CustomUpdate()
    {
        //base.CustomUpdate();
    }

    public override void CustomOnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.CustomOnRenderImage(source, destination);
    }

    public override void ChangeCamDistance(float _camDistance)
    {
        base.ChangeCamDistance(_camDistance);
    }

    public override void ChangeFocus(float _focusDistance)
    {
        base.ChangeFocus(_focusDistance);
        Vector3 tempFocusVector = transform.forward * _focusDistance;       // the camera focus point is determined
        depthCam.transform.LookAt(tempFocusVector);
    }

}


using UnityEngine;
using System.Collections;

public class StereoscopyController_DepthReuse : StereoscopyController_Base 
{
    [Tooltip("The resolution multiplier for the depth camera. This value is multiplied by the resolution of the application to determine the resolution of the depth render texture.")]
	public float depthCamResolutionMultiplier = 1f;
    [Tooltip("The depth system splits the camera frustum in two. This is the border between the two frustums. They could overlap.")]
	public float nearFarBorder = 80;
    [Tooltip("The depth system splits the camera frustum in two. This is the border between the two frustums. They could overlap.")]
	public float depthCamNearClipPlane = 60;
    [Tooltip("The layer mask of the depth camera. In order to increase performance you can set the depth camera to render the low resolution version of game geometry.")]
	public LayerMask depthCamLayerMask;
	private GameObject depthCam;    // the camera that will render geometry in the distance
    private RenderTexture depthRendTex;     // the render texture of the depth camera

	public override void CustomStart ()
	{
        depthCamResolutionMultiplier = Mathf.Clamp(depthCamResolutionMultiplier, 0.02f, 10f);   // the depth resolution cannot be smaller than zero. It is also not advisable to be a very large number.
        nearFarBorder = Mathf.Clamp(nearFarBorder, 0.02f, Mathf.Infinity);      // the border between the two frustums should not be smaller than zero.
        depthCamNearClipPlane = Mathf.Clamp(depthCamNearClipPlane, 0.02f, Mathf.Infinity);
        depthRendTex = new RenderTexture((int)(depthCamResolutionMultiplier * Screen.width), (int)(depthCamResolutionMultiplier * Screen.height), 24);    // the depth camera is created and set up
		depthCam = new GameObject("DepthCam ");
		depthCam.transform.parent = gameObject.transform;
		depthCam.transform.localPosition = Vector3.zero;
		depthCam.transform.localRotation = Quaternion.identity;
		depthCam.AddComponent<Camera>();

		switch(stereoMode)      // there are two different modes that are currently implemented. Horizontal interlaced and side by side. Their depth rextures has different aspect ratio requirements 
		{
		case Mode.Interlaced:
			depthCam.GetComponent<Camera>().aspect = (float)Screen.width/(float)Screen.height;
			break;
		case Mode.SBS:
			depthCam.GetComponent<Camera>().aspect = (float)Screen.width/2/(float)Screen.height;
			break;
		}

        depthCam.GetComponent<Camera>().fieldOfView = GetComponent<Camera>().fieldOfView;   // for more information about the following code you can check out the MultiscopyController_DepthReuse component
		depthCam.GetComponent<Camera>().cullingMask = (int)depthCamLayerMask;
		depthCam.GetComponent<Camera>().targetTexture = depthRendTex;
		depthCam.GetComponent<Camera>().nearClipPlane = depthCamNearClipPlane;
		depthCam.GetComponent<Camera>().useOcclusionCulling = false;
		base.CustomStart();
		leftCam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
		rightCam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
		leftCam.GetComponent<Camera>().backgroundColor = new Color(255,0,255,0);
		rightCam.GetComponent<Camera>().backgroundColor = new Color(255,0,255,0);
		leftCam.GetComponent<Camera>().farClipPlane = nearFarBorder;
		rightCam.GetComponent<Camera>().farClipPlane = nearFarBorder;
		leftCam.GetComponent<Camera>().nearClipPlane = GetComponent<Camera>().nearClipPlane;
		rightCam.GetComponent<Camera>().nearClipPlane = GetComponent<Camera>().nearClipPlane;
		mat.SetTexture("_FarDepthTex", depthRendTex);
	}
	
	public override void CustomUpdate ()
	{
		//base.CustomUpdate();
	}
	
	public override void CustomOnRenderImage (RenderTexture source, RenderTexture destination)
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
		Vector3 tempFocusVector = transform.forward * _focusDistance;
		depthCam.transform.LookAt(tempFocusVector);
	}

}

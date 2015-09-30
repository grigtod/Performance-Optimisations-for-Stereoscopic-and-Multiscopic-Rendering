using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoveatedMultiscopicCameraSetup : MonoBehaviour
{
    [Tooltip("The pefab of one camera for foveated multiscopiy.")]
    public GameObject foveatedCameraPrefab;
    [Tooltip("This value is the distance between cameras. This value symbolises the distance between the user eyes in the virtual world.")]
    public float camDistance = 0.04f;
    [Tooltip("The focus distance is the distance of the focus points for the cameras. The focus point is determined by multiplying the focusDistance to the forward vercor of the camera")]
    public float focusDistance = 10;
    [Tooltip("Reference to the material that will combine the views.")]
    public Material foveatedMultiscopyMat;
    private List<GameObject> camerasGO = new List<GameObject>();     // a list of all the multiscopic cameras.
    private UpdateViewport updateViewportRef;       //reference of the UpdaveViewport component of one of the cameras. It is needed to get the gaze point

    void Start()
    {

        for (int i = 0; i < 8; i++) // instantiate all the foveated cameras and set them up
        {
            camerasGO.Add((GameObject)Instantiate(foveatedCameraPrefab, transform.position, Quaternion.identity));
            camerasGO[i].name = "CamGO " + i;
            camerasGO[i].transform.parent = transform;
            camerasGO[i].transform.localPosition = Vector3.zero;
            camerasGO[i].transform.localRotation = Quaternion.identity;
        }

        Invoke("SetMatProperties", 0.1f); // This function is invoked because there needs to be a delay. 
        SetMatProperties(); //.it is called now anyway so that no error warning would appear
    }

    void SetMatProperties()
    {
        updateViewportRef = camerasGO[0].GetComponent<RefHelper>().ldRendTex.gameObject.GetComponent<UpdateViewport>();
        for (int i = 0; i < 8; i++)     //supply all the required information to the shader
        {
            foveatedMultiscopyMat.SetTexture("_ViewTex" + (i + 1), camerasGO[i].GetComponent<RefHelper>().screenRendTex.rendTex);
            foveatedMultiscopyMat.SetTexture("_LDViewTex" + (i + 1), camerasGO[i].GetComponent<RefHelper>().ldRendTex.rendTex);
        }
        foveatedMultiscopyMat.SetInt("_WidthRes", Screen.width);
        foveatedMultiscopyMat.SetInt("_HeightRes", Screen.height);
        foveatedMultiscopyMat.SetInt("_LDWidthRes", (int)(updateViewportRef.highPolyCamDimensions.x));
        foveatedMultiscopyMat.SetInt("_LDHeightRes", (int)(updateViewportRef.highPolyCamDimensions.y));

        foveatedMultiscopyMat.SetFloat("_multiplX", 1.0f / (float)((Screen.width * 3) / 8));
        foveatedMultiscopyMat.SetFloat("_multiplY", 1.0f / (float)(Screen.height));
        foveatedMultiscopyMat.SetFloat("_LDmultiplX", 1.0f / (float)((updateViewportRef.highPolyCamDimensions.x * 3) / 8));
        foveatedMultiscopyMat.SetFloat("_LDmultiplY", 1.0f / (float)(updateViewportRef.highPolyCamDimensions.y));

        foveatedMultiscopyMat.SetFloat("_LDRegionResolutionMultiplier", updateViewportRef.regionResolutionMultiplier);
        RefreshCameras();   // it sets the cameras appropriately
    }

    void Update()
    {
        //RefreshCameras();
    }

    void OnPostRender()
    {
        OutputOnScreen();
    }

    public void RefreshCameras()
    {
        ChangeCamDistance(camDistance);
        ChangeFocus();
    }

    void ChangeCamDistance(float _camDistance)
    {
        for (int i = 0; i < 8; i++)
        {
            camerasGO[i].transform.localPosition = new Vector3(-(_camDistance * 3) + (_camDistance * i), 0, 0); // properly distribute the 8 cameras so there are 4 for earch side of the centre (this) go
        }
    }

    void ChangeFocus()
    {
        Vector3 tempFocusVector = transform.forward * focusDistance;//transform.TransformPoint(transform.forward * _focusDistance);
        for (int i = 0; i < 8; i++)
        {
            camerasGO[i].transform.LookAt(tempFocusVector);
        }
    }

    public void OutputOnScreen()    //  call this from "void OnPostRender()". It displays the shader output on the screen
    {
        foveatedMultiscopyMat.SetFloat("_focusPointX", updateViewportRef.focusPoint.x);
        foveatedMultiscopyMat.SetFloat("_focusPointY", updateViewportRef.focusPoint.y);

        foveatedMultiscopyMat.SetPass(0);//the first pass of this shader is the pass that also combines the textures of cameras for foveated rendering
        GL.LoadOrtho();
        GL.Viewport(new Rect(0, 0, Screen.width, Screen.height));

        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(0.0f, 0, 0);
        GL.TexCoord2(1, 0);
        GL.Vertex3(1, 0, 0);
        GL.TexCoord2(1, 1);
        GL.Vertex3(1, 1, 0);
        GL.TexCoord2(0, 1);
        GL.Vertex3(0, 1, 0);
        GL.End();
    }

}


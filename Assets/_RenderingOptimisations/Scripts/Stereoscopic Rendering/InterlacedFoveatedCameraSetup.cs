using UnityEngine;
using System.Collections;

public class InterlacedFoveatedCameraSetup : MonoBehaviour
{
    [Tooltip("The pefab of one camera for stereoscopy.")]
    public GameObject interlacedOneViewPrefab;
    [Tooltip("This value is the distance between cameras. This value symbolises the distance between the user eyes in the virtual world.")]
    public float camDistance = 0.04f;
    [Tooltip("The focus distance is the distance of the focus points for the cameras. The focus point is determined by multiplying the focusDistance to the forward vercor of the camera")]
    public float focusDistance = 10;
    [Tooltip("Reference to the material that will combine the views.")]
    public Material stereoscopicInterlacedMat;
    private GameObject leftCam;
    private GameObject rightCam;
    private UpdateViewport updateViewportRef;

    void Start()
    {
        GetComponent<Camera>().enabled = false;     // the camera should be disabled otherwise there will be a few warnings for the first few frames.
        leftCam = (GameObject)Instantiate(interlacedOneViewPrefab, transform.position, Quaternion.identity);    // create and set up the two cameras for the foveated region
        leftCam.transform.parent = gameObject.transform;
        leftCam.transform.localPosition = Vector3.zero;
        leftCam.transform.localRotation = Quaternion.identity;

        rightCam = (GameObject)Instantiate(interlacedOneViewPrefab, transform.position, Quaternion.identity);
        rightCam.transform.parent = gameObject.transform;
        rightCam.transform.localPosition = Vector3.zero;
        rightCam.transform.localRotation = Quaternion.identity;

        //go through all the cams and set the mat textures...
        Invoke("SetMatProperties", 0.1f);       // because this should happen after some other things it is invoked with a small delay.
    }

    void SetMatProperties()
    {
        stereoscopicInterlacedMat.SetTexture("_ViewTexLeft", leftCam.GetComponent<RefHelper>().screenRendTex.rendTex); //supply all the required information to the shader
        stereoscopicInterlacedMat.SetTexture("_ViewTexRight", rightCam.GetComponent<RefHelper>().screenRendTex.rendTex);

        stereoscopicInterlacedMat.SetTexture("_LDViewTexLeft", leftCam.GetComponent<RefHelper>().ldRendTex.rendTex);
        stereoscopicInterlacedMat.SetTexture("_LDViewTexRight", rightCam.GetComponent<RefHelper>().ldRendTex.rendTex);

        stereoscopicInterlacedMat.SetInt("_WidthRes", Screen.width);
        stereoscopicInterlacedMat.SetInt("_HeightRes", Screen.height);
        stereoscopicInterlacedMat.SetInt("_LDWidthRes", (int)(leftCam.GetComponent<RefHelper>().ldRendTex.gameObject.GetComponent<UpdateViewport>().highPolyCamDimensions.x));
        stereoscopicInterlacedMat.SetInt("_LDHeightRes", (int)(leftCam.GetComponent<RefHelper>().ldRendTex.gameObject.GetComponent<UpdateViewport>().highPolyCamDimensions.y));

        stereoscopicInterlacedMat.SetFloat("_multiplX", 1.0f / (float)((Screen.width / 2)));
        stereoscopicInterlacedMat.SetFloat("_multiplY", 1.0f / (float)(Screen.height));
        stereoscopicInterlacedMat.SetFloat("_LDmultiplX", 1.0f / (float)((leftCam.GetComponent<RefHelper>().ldRendTex.gameObject.GetComponent<UpdateViewport>().highPolyCamDimensions.x)));
        stereoscopicInterlacedMat.SetFloat("_LDmultiplY", 1.0f / (float)(leftCam.GetComponent<RefHelper>().ldRendTex.gameObject.GetComponent<UpdateViewport>().highPolyCamDimensions.y));

        stereoscopicInterlacedMat.SetFloat("_LDRegionResolutionMultiplier", leftCam.GetComponent<RefHelper>().ldRendTex.gameObject.GetComponent<UpdateViewport>().regionResolutionMultiplier);

        updateViewportRef = leftCam.GetComponent<RefHelper>().ldRendTex.gameObject.GetComponent<UpdateViewport>();
        RefreshCameras();
        GetComponent<Camera>().enabled = true;
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

    public virtual void ChangeCamDistance(float _camDistance)
    {
        leftCam.transform.localPosition = new Vector3(-(_camDistance / 2f), 0, 0);
        rightCam.transform.localPosition = new Vector3((_camDistance / 2f), 0, 0);
    }

    void ChangeFocus()
    {
        Vector3 tempFocusVector = transform.forward * focusDistance;
        leftCam.transform.LookAt(tempFocusVector);
        rightCam.transform.LookAt(tempFocusVector);
    }

    public void OutputOnScreen()    // call this from "void OnPostRender() "
    {
        stereoscopicInterlacedMat.SetFloat("_focusPointX", updateViewportRef.focusPoint.x);
        stereoscopicInterlacedMat.SetFloat("_focusPointY", updateViewportRef.focusPoint.y);

        stereoscopicInterlacedMat.SetPass(0);   // the first pass of this shader is the pass that also combines the textures of cameras for foveated rendering
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


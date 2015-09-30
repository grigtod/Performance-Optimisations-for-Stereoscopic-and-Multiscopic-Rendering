using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]    // a camera component is required
public class UpdateViewport : MonoBehaviour
{
    [Tooltip("Provide a reference to the base camera. The base camera is the main camera that renders the whole screen before the foveated camera (this camera).")]
    public Camera baseCamRef;   // reference to the base camera
    [Tooltip("The foveated region size will be this value multiplied by the screen height.")]
    public float regionResolutionMultiplier = 0.5f;     // this is the multipliers that determines the size of the foveated region based of the screen height.
    public enum FocusPointModes { MousePosition, ExternallySupplied, MiddleOfScreen }; // there are 3 different ways the foveated region position can be supplied: mouse, when calling a special function or always staying at middle of the screen
    [Tooltip("How would you supply the foveated region position?")]
    public FocusPointModes focusPointMode;
    [Tooltip("If this is a multiscopic foveated rendering system this should be ticked.")]
    public bool multiscopic = false;

    [HideInInspector]
    public Vector2 focusPoint;      // the current position of gaze/focus for the foveated rendering. 
    [HideInInspector]
    public Vector2 highPolyCamDimensions;               // the camera render texture width and height. External scripts need access to this, but it shouldn't be accessible from the Unity Inspector and is therefore hidden

    private Camera myCamRef;                            // this is a reference to the camera component attached to this game object
    private float helperValX;                           // two helper variables 
    private float helperValY;
    private float multiscopicHelper;    // another helper value for the vanishing point of multiscopic cameras

    void OnEnable()     // in order to be able to dynamically enable and disable the camera this should not be executed in the 'Start' function. 'OnEnable' function is used.
    {
        StopCoroutine("FocusPointModeMousePosUpdate");      // stop the coroutine just in case
        regionResolutionMultiplier = Mathf.Clamp(regionResolutionMultiplier, 0.01f, 1f);    // the size of the foveated region cannot be bigger than the screen height or smaller or equal to 0.
        if (baseCamRef == null)
        {
            Debug.LogError("Missing references in " + this.name + ".cs . Please provide a reference to " + baseCamRef.name + ".");
        }
        Invoke("RestartFoveatedRendering", 0.05f);      // if the "RestartFoveatedRendering" function is called at the start of the application it will not work. It needs to be called with a small delay.
    }

    void RestartFoveatedRendering()
    {
        myCamRef = GetComponent<Camera>();      //assign the reference to my camera.
        highPolyCamDimensions = new Vector2((float)Screen.height * regionResolutionMultiplier, (float)Screen.height * regionResolutionMultiplier);     //the foveated region is a square and it's size is determined by the height resolution.

        if (multiscopic)
        {
            myCamRef.aspect = 1;    // it is important to explicitly set the aspect ratio of the multiscopic camera to 1 in order to keep the proper aspect ratio.
        }

        float frustumHeight = Mathf.Tan(baseCamRef.fieldOfView / 2f * Mathf.Deg2Rad); // simpliefied formula from here: " http://docs.unity3d.com/Manual/FrustumSizeAtDistance.html ". It is used to calculate the frustum height and that value is used to reverse calculate the field of view of the foveated camera.
        myCamRef.fieldOfView = 2f * Mathf.Atan(frustumHeight * regionResolutionMultiplier) * Mathf.Rad2Deg;     // a reverse calculation of the field of view of the foveated camera. 
        helperValY = frustumHeight * myCamRef.nearClipPlane * 2;    // this is a helper variable and is a result of a lot of trial and error. Originally the foveated system only worked for present field of view, aspect ratio and clipping plain. The helper variables make it possible to change these values and the system would still provide the proper vanishing point for the purposes of foveated rendering.
        helperValX = helperValY * (float)(Screen.width) / (float)(Screen.height);       // the helper for Y is the X helper multiplied by the aspect ratio of the screen. This enables the use of other aspect ratios.

        if (multiscopic)
        {
            multiscopicHelper = myCamRef.nearClipPlane / (1.6180f / helperValY) * ((float)Screen.width / (float)Screen.height);     // the multiscopic foveated rendering had a very persistant issue that required additional value to be added in the vanishing point calculation. The value had to be a different one for every aspect ratio, field of view and clipping plane setting. This is similar to 'helperValX' with the only difference that helperValY needs be divide a 'magic number' that was determined to be "1.6". As 1.6180f is the golden ratio it was tried and provided good results, it is therefore used. This is a classic example of a 'magic number' creeping out in code.                     
            //   the multiscopicHelper can also be calculated with: (2 * nearClipPlane * nearClipPlane * frustumHeight * aspect)/1.618
        }

        switch (focusPointMode)     // start the appropriate coroutine for the selected mode. If the mode is "ExternallySupplied", the user needs to supply a focus point. 
        {
            case FocusPointModes.MiddleOfScreen:
                SetFocusPoint(new Vector2(Screen.width / 2, Screen.height / 2));
                break;
            case FocusPointModes.MousePosition:
                StartCoroutine("FocusPointModeMousePosUpdate");
                break;
            //if it's FocusPointModes.ExternallySupplied the user will have to call SetFocusPoint to change the focus/gaze point
        }
    }

    IEnumerator FocusPointModeMousePosUpdate()      // set the Focus point to be the mouse position
    {
        while (true)
        {
            SetFocusPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetFocusPoint(Vector2 _newFocusPoint)       // this function sets the position of the foveated region.
    {
        if (_newFocusPoint.x > 0 && _newFocusPoint.x < Screen.width && _newFocusPoint.y > 0 && _newFocusPoint.y < Screen.height)    //check if the focus point is in the screen. If it's not (the mouse is outside of the application are or the user looks away from the application) the foveated system is not refreshed.    
        {
            focusPoint = new Vector2(
                            Mathf.Clamp(_newFocusPoint.x, 0, Screen.width),
                            Mathf.Clamp(_newFocusPoint.y, 0, Screen.height));     // clamping the focusPoint to minimize weird behaviour.

            if (multiscopic) //calls the SetVanishingPoint in the appropriate way. 
            {
                SetVanishingPoint(myCamRef, new Vector3((multiscopicHelper + (0.5f - focusPoint.x / Screen.width) * helperValX), ((0.5f - focusPoint.y / Screen.height) * helperValY)));    // it is not merely enough to call the vanishing point function with coordinates. In order to support a wide variety of resolutions, aspect ratios, field of views and clipping plane settings it is required to use the helpers. 
            }
            else
            {
                SetVanishingPoint(myCamRef, new Vector3(((0.5f - focusPoint.x / Screen.width) * helperValX), ((0.5f - focusPoint.y / Screen.height) * helperValY)));
            }
        }
    }

    float w, h, left, right, bottom, top;
    void SetVanishingPoint(Camera cam, Vector2 perspectiveOffset)       // this is a functio that sets the vanishing point of the camera matrix. This function is available from here:  : http://wiki.unity3d.com/index.php?title=OffsetVanishingPoint
    {
        w = 2f * cam.nearClipPlane / cam.projectionMatrix.m00;
        h = 2f * cam.nearClipPlane / cam.projectionMatrix.m11;

        left = -w / 2f - perspectiveOffset.x;
        right = left + w;
        bottom = -h / 2f - perspectiveOffset.y;
        top = bottom + h;

        cam.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
    }

    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)      // this is another function available from here: http://wiki.unity3d.com/index.php?title=OffsetVanishingPoint 
    {
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = (2f * near) / (right - left);
        m[0, 1] = 0;
        m[0, 2] = (right + left) / (right - left);
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = (2f * near) / (top - bottom);
        m[1, 2] = (top + bottom) / (top - bottom);
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = -(far + near) / (far - near);
        m[2, 3] = -(2f * far * near) / (far - near);
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = -1f;
        m[3, 3] = 0;
        return m;
    }

}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CreateRenderTexture : MonoBehaviour
{
    public enum Mode { Traditional, Stereoscopic, Multiscopic };
    [Tooltip("This value is multiplied to the resoluton of the screen to determine the resolution of the created render texture.")]
    public float resolutionMultiplier = 1f;
    [Tooltip("Tick this if the render texture is full screen.")]
    public bool fullScreen = false;
    public Mode currentMode;
    [Tooltip("Provide a reference to the 'UpdateViewport'. If this is not a foveated region render texture it does not require a reference.")]
    public UpdateViewport uviewport;
    public enum aaMode { off = 1, two = 2, four = 4, eight = 8 };
    [Tooltip("The current anti-aliasing mode.")]
    public aaMode currentAAMode = aaMode.off;

    [HideInInspector]
    public RenderTexture rendTex;   //the render texture for this camera.

    void Start()
    {
        resolutionMultiplier = Mathf.Clamp(resolutionMultiplier, 0.01f, 10);     // the resolution multiplier should not be smaller or equal to zero. It makes no sence to be larger than 10 either.
        if (fullScreen)
        {
            switch (currentMode)
            {
                case Mode.Traditional:
                    rendTex = new RenderTexture((int)(Screen.width * resolutionMultiplier), (int)(Screen.height * resolutionMultiplier), 16, RenderTextureFormat.ARGB32);   // for traditional 2D rendering the render texture created should have the resolution of the application multiplied by the resolution multiplier
                    break;
                case Mode.Stereoscopic:
                    GetComponent<Camera>().aspect = (float)Screen.width / (float)Screen.height;   // to keep proper aspect ratio it is very important to supply the aspect ratio of the application to the camera aspect.
                    rendTex = new RenderTexture((int)(Screen.width * resolutionMultiplier / 2), (int)(Screen.height * resolutionMultiplier), 16, RenderTextureFormat.ARGB32);     // if the rendering is stereoscopic (two views) the resolution of the render texture should be the same as the resolution of the render texture from the traditional rendering, but with the width divided by two. 
                    break;
                case Mode.Multiscopic:
                    rendTex = new RenderTexture((int)((Screen.width * resolutionMultiplier) * 3 / 8), (int)((Screen.height * resolutionMultiplier) / 3), 16, RenderTextureFormat.ARGB32);   // the width resolution of the render texture for multiscopic rendering should be multiplied by 3 (subpixel rendering increases width by two) and divided by 8 (for the 8 views). The height should be divided by 3. Both should be multiplied with the resolution multiplier.
                    break;
            }
        }
        else
        {
            if (uviewport == null)      // if this is not a fullscreen render texture a uviewport reference is not provided then something is wrong and the user should be notified.
            {
                Debug.LogError("Please provide a reference to the 'UpdateViewport' or tick the 'fullscreen' field in the " + this.name + " component of the " + gameObject.name + "game object.");
            }

            switch (currentMode)     // the render textures for foveated regions (not fullscreen) are very similar to the render textures that are fullscreen, but with the only difference that aditional multiplier fot he oveated region size is used. 
            {
                case Mode.Traditional:
                    rendTex = new RenderTexture((int)(Screen.height * resolutionMultiplier * uviewport.regionResolutionMultiplier), (int)(Screen.height * resolutionMultiplier * uviewport.regionResolutionMultiplier), 16, RenderTextureFormat.ARGB32);
                    break;
                case Mode.Stereoscopic:
                    GetComponent<Camera>().aspect = 1;
                    rendTex = new RenderTexture((int)(Screen.height * resolutionMultiplier * uviewport.regionResolutionMultiplier / 2f), (int)(Screen.height * resolutionMultiplier * uviewport.regionResolutionMultiplier), 16, RenderTextureFormat.ARGB32);
                    break;
                case Mode.Multiscopic:
                    rendTex = new RenderTexture((int)((Screen.height * resolutionMultiplier * uviewport.regionResolutionMultiplier) * 3 / 8), (int)((Screen.height * resolutionMultiplier * uviewport.regionResolutionMultiplier) / 3), 16, RenderTextureFormat.ARGB32);
                    break;
            }
        }
        rendTex.antiAliasing = (int)currentAAMode;
        GetComponent<Camera>().targetTexture = rendTex;
    }

}


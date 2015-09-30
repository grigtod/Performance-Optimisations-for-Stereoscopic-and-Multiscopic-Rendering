using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]   // this script requires a camera
public class StereoscopyController_DynamicResolution : StereoscopyController_Base 
{
    [Tooltip("Resolution multiplier for the width of the render texture.")]
	public float resolutionMultiplierWidth = 1.0f;
    [Tooltip("Resolution multiplier for the height of the render texture.")]
	public float resolutionMultiplierHeight = 1.0f;
    [Tooltip("If this is set to true the resize would only happen at the start of the application.")]
	public bool onlySetAtStart = false;
    [Tooltip("The minimum and maximum values for the 'resolutionMultiplierWidth'.")]
	public Vector2 resolutionWidthMultiplierMinMax = new Vector2(0.6f, 2f);
    [Tooltip("The minimum and maximum values for the 'resolutionMultiplierHeight'.")]
	public Vector2 resolutionHeightMultiplierMinMax = new Vector2(0.6f, 2f);
    [Tooltip("The current frames per second target")]
	public int targetFPS = 60;
    public enum Mode { Custom, Intel };
    [Tooltip("The current mode of dynamic resolution. \n\nCustom - a custom made implementation that may provide better experience but not as scalable as the Intel Implementation. \n\nIntel - this implementation was first proposed by Intel. It should work on any CPU. It provides better scalability but the change is very aparent and not very visually pleasing.")]
    public Mode dynamicResolutionMode;
    [Tooltip("This is the fate at which the resolution adapts. If you want this to happen every frame and you target 60 frames per second this value should be set to 0.016 (1/60)")]
    public float rateOfChange = 0.2f;       // the dynamic resolution will change at this rate. 
    [Tooltip("This toggle enables a helper gui at the right corner of the screen. It is useful for debugging and testing.")]
    public bool guiInfoDisplay = false;     // should the debug gui be displayed on the screen

    private PerformanceManager perfManager;     // the performance manager supplies the performance information and is therefore needed
    private float helper;       //used for some calculations

	public override void CustomStart ()
	{
		base.CustomStart();
		
		perfManager = Object.FindObjectOfType<PerformanceManager>();
        if (perfManager == null)     // the performance manager singleton is very important. If there isn't a performance manager in the scene a new one is loaded from the Resources folder.
        {
            GameObject pmTemp = Instantiate(Resources.Load("PerformanceManager", typeof(GameObject))) as GameObject;
            perfManager = pmTemp.GetComponent<PerformanceManager>();
        }
        resolutionWidthMultiplierMinMax = new Vector2(Mathf.Clamp(resolutionWidthMultiplierMinMax.x, 0.01f, 20), Mathf.Clamp(resolutionWidthMultiplierMinMax.y, 0.01f, 20));    // the resolution multipliers need to be clamped
        resolutionHeightMultiplierMinMax = new Vector2(Mathf.Clamp(resolutionHeightMultiplierMinMax.x, 0.01f, 20), Mathf.Clamp(resolutionHeightMultiplierMinMax.y, 0.01f, 20));     // the resolution multipliers need to be clampe

		if(onlySetAtStart)
		{
			RecreateRendTex();
		}
		else
		{
            RecreateRendTex();
			StartCoroutine("AdaptResolution");
		}
	}

	public override void CustomUpdate ()
	{
		
	}

    IEnumerator AdaptResolution()      // if enabled this coroutine will resize the render texture based on performance
	{
		while(true)
		{
            targetFPS = Mathf.Clamp(targetFPS, 1, 1000); // the targeted resolution should not be smaller than 1
            resolutionWidthMultiplierMinMax = new Vector2(Mathf.Clamp(resolutionWidthMultiplierMinMax.x, 0.01f, 20), Mathf.Clamp(resolutionWidthMultiplierMinMax.y, 0.01f, 20)); // the resolution multipliers need to be clamped
            resolutionHeightMultiplierMinMax = new Vector2(Mathf.Clamp(resolutionHeightMultiplierMinMax.x, 0.01f, 20), Mathf.Clamp(resolutionHeightMultiplierMinMax.y, 0.01f, 20)); // the resolution multipliers need to be clamped
            rateOfChange = Mathf.Clamp(rateOfChange, 0.01f, 10f);

            switch (dynamicResolutionMode)      //call the appropriate function based of the mode selected 
            {
                case Mode.Custom: DynamicResolutionCustom();
                    break;
                case Mode.Intel: DynamicResolutionIntel();
                    break;
            }
            yield return new WaitForSeconds(rateOfChange);
		}
	}

    void DynamicResolutionCustom()
    {
        helper = perfManager.fps - targetFPS;

        if (helper > 2 || helper < -2)      //my implementation of the dynamic resolution aims to adjust rarely. It's not as accurate as the Intel one, but might be better in some situations
        {
            resolutionMultiplierWidth += helper / 100;
            resolutionMultiplierHeight += helper / 100;
            RecreateRendTex();
        }
    }

    void DynamicResolutionIntel() // https://software.intel.com/en-us/articles/dynamic-resolution-rendering-article
    {
        helper = 1000f / targetFPS;
        resolutionMultiplierWidth = resolutionMultiplierWidth + (rateOfChange * resolutionMultiplierWidth * (helper - perfManager.ms) / helper);
        resolutionMultiplierHeight = resolutionMultiplierHeight + (rateOfChange * resolutionMultiplierHeight * (helper - perfManager.ms) / helper);
        RecreateRendTex();
    }
	
	void RecreateRendTex()
	{
		Destroy(leftCamRendTex);
		Destroy(rightCamRendTex);
        resolutionMultiplierWidth = Mathf.Clamp(resolutionMultiplierWidth, resolutionWidthMultiplierMinMax.x, resolutionWidthMultiplierMinMax.y);       //it's very important to clamp the multipliers
        resolutionMultiplierHeight = Mathf.Clamp(resolutionMultiplierHeight, resolutionHeightMultiplierMinMax.x, resolutionHeightMultiplierMinMax.y);
		leftCamRendTex = new RenderTexture((int)((resolutionMultiplierWidth*Screen.width)/2), (int) (resolutionMultiplierHeight * Screen.height), 24);
		rightCamRendTex = new RenderTexture((int)((resolutionMultiplierWidth*Screen.width)/2), (int) (resolutionMultiplierHeight * Screen.height), 24);
		leftCam.GetComponent<Camera>().targetTexture = leftCamRendTex;
		rightCam.GetComponent<Camera>().targetTexture = rightCamRendTex;
		mat.SetTexture("_LeftTex", leftCamRendTex);
		mat.SetTexture("_RightTex", rightCamRendTex);
		//mat.SetFloat("_WidthRes", Screen.width * resolutionMultiplierWidth);
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
	}

    int width = Screen.width;       //these are used for the debug gui.
    int height = Screen.height;
    GUIStyle guiStyle = new GUIStyle();
    Rect rect;
    float fontSize = 0.035f;

    void OnGUI()
    {
        if (guiInfoDisplay)      //displaying gui could further reduce performance and therefore should be used only for debugging. It should not be used when the performance is measured as the resulting measurement will not be the correct one.
        {
            rect = new Rect(0, 0, width, height * fontSize);
            guiStyle.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            guiStyle.alignment = TextAnchor.LowerRight;
            guiStyle.fontSize = (int)(height * fontSize);
            GUI.Label(rect, Mathf.Round(perfManager.fps * 1) / 1 + " fps (" + Mathf.Round(perfManager.ms * 10) / 10 + " ms)", guiStyle);
            rect = new Rect(0, height * fontSize, width, height * fontSize);
            GUI.Label(rect, dynamicResolutionMode.ToString() + " mode. " + Mathf.Round(resolutionMultiplierWidth * 100) / 100 + " width and " + Mathf.Round(resolutionMultiplierHeight * 100) / 100 + " height " + " multipliers. " + targetFPS + " fps target.", guiStyle);
            rect = new Rect(0, height * fontSize * 2, width, height * fontSize);
            GUI.Label(rect, "Current resolution: " + Mathf.Round(resolutionMultiplierWidth * Screen.width * 1) / 1 + " x " + Mathf.Round(resolutionMultiplierHeight * Screen.height * 1) / 1, guiStyle);
        }
    }

}

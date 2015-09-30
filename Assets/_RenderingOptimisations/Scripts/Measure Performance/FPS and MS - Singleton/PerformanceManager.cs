using UnityEngine;

public class PerformanceManager : MonoBehaviour 
{//Standard singleton script.
	////This is modified from the FPSDisplay.cs available here: http://wiki.unity3d.com/index.php?title=FramesPerSecond
	private static PerformanceManager _instance;

    public enum PerformanceDisplayMode {HidePerf, DisplayLeftCorner, DisplayRightCorner};
    public PerformanceDisplayMode perfMode = PerformanceDisplayMode.HidePerf;

	public static PerformanceManager instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<PerformanceManager>();
			}
			return _instance;
		}
	}
	
	public float fps;
	public float ms;
	private float dt = 0.0f;

	void Update()
	{
		dt += (Time.deltaTime - dt) * 0.1f;
		fps = 1.0f / dt;
		ms = dt * 1000.0f;
	}

	//here I will have different premade performance showing codes. These will be used in the creation of the video.

	int width = Screen.width;
	int height = Screen.height;
	GUIStyle guiStyle = new GUIStyle();
	Rect rect;
	float fontSize = 0.035f;

	//Left side fps and ms
	void OnGUI()
	{
        switch (perfMode)
        {
            case PerformanceDisplayMode.DisplayLeftCorner:
                rect = new Rect(0, 0, width, height * fontSize);
		        guiStyle.normal.textColor = new Color (0.0f, 1.0f, 0.0f, 1.0f);
		        guiStyle.alignment = TextAnchor.UpperLeft;
		        guiStyle.fontSize = (int)(height * fontSize);
		        GUI.Label(rect, Mathf.Round(fps * 1) / 1 + " fps (" +  Mathf.Round(ms * 10) / 10 + " ms)" , guiStyle);
            break;
            case PerformanceDisplayMode.DisplayRightCorner:
                rect = new Rect(0, 0, width, height * fontSize);
		        guiStyle.normal.textColor = new Color (0.0f, 1.0f, 0.0f, 1.0f);
		        guiStyle.alignment = TextAnchor.LowerRight;
		        guiStyle.fontSize = (int)(height * fontSize);
		        GUI.Label(rect, Mathf.Round(fps * 1) / 1 + " fps (" +  Mathf.Round(ms * 10) / 10 + " ms)" , guiStyle);
            break;
        }
	}


}

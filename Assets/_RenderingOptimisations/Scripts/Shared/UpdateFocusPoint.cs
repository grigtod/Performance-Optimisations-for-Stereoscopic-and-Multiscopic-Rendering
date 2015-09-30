using UnityEngine;
using System.Collections;

public class UpdateFocusPoint : MonoBehaviour 
{
	public GameObject focusPoint;

//    public GazePointDataComponent gazePointRef;

	public enum FocusPointUpdateModes{MousePosition, TobiiEyeX, ExternallySupplied, MiddleOfScreen};
	public FocusPointUpdateModes updateMode;


	/*void Update () 
	{
		Ray ray;
		if(gazePointRef.LastGazePoint.Screen.x>0 && gazePointRef.LastGazePoint.Screen.x<Screen.width-1)
		{
			ray = Camera.main.ScreenPointToRay(new Vector2(gazePointRef.LastGazePoint.Screen.x,  gazePointRef.LastGazePoint.Screen.y)); 
		}
		else
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);;
		}
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			//Debug.DrawLine(transform.position,hit.point, Color.red);
			focusPoint.transform.position = hit.point;
		}
		else
		{
			//Debug.DrawRay(transform.position, (new Vector3(eyeScreenPos.x,eyeScreenPos.y,0) + Camera.main.transform.forward) * 100f, Color.red);
			focusPoint.transform.position = (Camera.main.transform.forward) * 10f;
		}

	}*/
}

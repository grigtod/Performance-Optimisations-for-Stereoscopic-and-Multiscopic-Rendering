using UnityEngine;
using System.Collections;

public class OnOff : MonoBehaviour 
{
	/*public bool current = false;

	public GameObject[] hdCams;
	public GameObject[] ldCams;
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			if(current)
			{
				current = false;
                UpdateCams(false);
			}
			else
			{
				current = true;
                UpdateCams(true);
			}
		}
	}

	void UpdateCams(bool newVal)
	{
		for(int i=0;i<hdCams.Length;i++)
		{
			hdCams[i].SetActive(!newVal);
			if(hdCams[i].GetComponent<UpdateViewport>()!=null)
			{
				hdCams[i].GetComponent<UpdateViewport>().ToggleFoveatedRendering(!newVal);
			}
		}
		for(int i=0;i<ldCams.Length;i++)
		{
			ldCams[i].SetActive(newVal);
		}
	}
	*/
}

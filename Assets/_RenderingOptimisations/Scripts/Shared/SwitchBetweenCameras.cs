using UnityEngine;
using System.Collections;

public class SwitchBetweenCameras : MonoBehaviour 
{
	public GameObject[] camsToSpawn;
	public int currentlySelectedCamera = 0;

	private GameObject[] cameras;

	void Start () 
	{
		cameras = new GameObject[camsToSpawn.Length];

		for(int i=0;i<camsToSpawn.Length;i++)
		{
			cameras[i] = (GameObject)Instantiate(camsToSpawn[i], transform.position, Quaternion.identity);
			cameras[i].transform.parent = transform;
			cameras[i].SetActive(false);
		}
		cameras[currentlySelectedCamera].SetActive(true);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			EnableCamera(0);
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			EnableCamera(1);
		}
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			EnableCamera(2);
		}
		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			EnableCamera(3);
		}
		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			EnableCamera(4);
		}
		if(Input.GetKeyDown(KeyCode.Alpha6))
		{
			EnableCamera(5);
		}
		if(Input.GetKeyDown(KeyCode.Alpha7))
		{
			EnableCamera(6);
		}
		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			EnableCamera(7);
		}
		if(Input.GetKeyDown(KeyCode.Alpha9))
		{
			EnableCamera(8);
		}
	}

	void EnableCamera(int _camIndex)
	{
		if(_camIndex<camsToSpawn.Length)
		{
			for(int i=0;i<camsToSpawn.Length;i++)
			{
				cameras[i].SetActive(false);
			}
			cameras[_camIndex].SetActive(true);
			Debug.Log (cameras[_camIndex].name + " was enabled.");
		}
	}

}

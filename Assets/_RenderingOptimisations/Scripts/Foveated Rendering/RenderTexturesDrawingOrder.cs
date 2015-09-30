using UnityEngine;
using System.Collections;

public class RenderTexturesDrawingOrder : MonoBehaviour
{	//This class draws all the render textures at an order specified here. If you want to combine multiple instances of this gameobject to implement things like stereoscopy or multiscopy you need to disable this and use the render textures as you please.
    [Tooltip("The order here determines the drawing order on screen.")]
    public CreateRenderTexture[] rendTextures;

    void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))//only draw once a frame
        {
            for (int i = 0; i < rendTextures.Length; i++)
            {
                if (rendTextures[i].gameObject.activeInHierarchy)
                {
                    if (rendTextures[i].fullScreen)
                    {
                        Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), rendTextures[i].rendTex);     //if the render texture is from the whole screen draw the render texture full screen
                    }
                    else    //if not, draw it appropriately for foveated rendering at the right size and location.
                    {
                        Graphics.DrawTexture(new Rect(rendTextures[i].uviewport.focusPoint.x - (Screen.height * rendTextures[i].uviewport.regionResolutionMultiplier / 2), Screen.height - rendTextures[i].uviewport.focusPoint.y - (Screen.height * rendTextures[i].uviewport.regionResolutionMultiplier / 2), Screen.height * rendTextures[i].uviewport.regionResolutionMultiplier, Screen.height * rendTextures[i].uviewport.regionResolutionMultiplier), rendTextures[i].rendTex);
                    }
                }
            }
        }
    }

}


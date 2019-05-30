using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscrepancyHeadEffects : MonoBehaviour
{

	[SerializeField]
	[Tooltip("If not set, will be searched in scene")]
	SteamVR_Camera steamvrCam;

	[SerializeField]
	[Range(0,1)]
	float maxDistToBlack = 0.3f;

	void Start()
	{
		if (steamvrCam == null)
		{
			steamvrCam = GameObject.FindObjectOfType<SteamVR_Camera>();
			if (steamvrCam == null){
				Debug.LogError("no SteamVR_Camera found");
			}
			if (steamvrCam.gameObject.GetComponent<SteamVR_Fade>() == null){
				steamvrCam.gameObject.AddComponent<SteamVR_Fade>();
			}
		}
	}

	public void FadeToBlack(float distance)
	{
		float alpha = distance / maxDistToBlack;
		SteamVR_Fade.Start(new Color(0, 0, 0, alpha), 0);
	}
}
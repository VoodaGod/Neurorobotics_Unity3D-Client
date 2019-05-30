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


	[SerializeField]
	[Range(0, 1)]
	float maxDistToFullBlur = 0.3f;

	//  ---------------------------------------------------------------------------------------------------
	//	SuperBlur taken from: https://github.com/PavelDoGreat/Super-Blur
	//	---------------------------------------------------------------------------------------------------
	SuperBlur.SuperBlur superBlur;

	[SerializeField]
	[Range(1, 8)]
	int superBlurIterations = 5;

	[SerializeField]
	SuperBlur.BlurKernelSize superBlurKernelsize = SuperBlur.BlurKernelSize.Big;

	[SerializeField]
	[Tooltip("set to SuperBlur/Material/SuperBlurPostEffect material")]
	Material superBlurPostEffectMaterial;

	[SerializeField]
	[Tooltip("set to SuperBlur/Material/SuperBlurUI material")]
	Material superBlurUIMaterial;

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

		if (steamvrCam.gameObject.GetComponent<SuperBlur.SuperBlur>() == null){
			superBlur = steamvrCam.gameObject.AddComponent<SuperBlur.SuperBlur>();
		}
		if (superBlurPostEffectMaterial == null || superBlurUIMaterial == null){
			Debug.LogError("superBlur Material(s) not set");
		}
		superBlur.gammaCorrection = false;
		superBlur.iterations = superBlurIterations;
		superBlur.kernelSize = superBlurKernelsize;
		superBlur.downsample = 0;
		superBlur.interpolation = 0;
		superBlur.blurMaterial = superBlurPostEffectMaterial;
		superBlur.UIMaterial = superBlurUIMaterial;
	}

	void Update()
	{
		superBlur.iterations = superBlurIterations;
		superBlur.kernelSize = superBlurKernelsize;
	}

	public void FadeToBlack(float distance)
	{
		float alpha = distance / maxDistToBlack;
		SteamVR_Fade.Start(new Color(0, 0, 0, alpha), 0);
	}

	public void Blur(float distance)
	{
		float strength = distance / maxDistToFullBlur;
		superBlur.interpolation = strength;
	}
}
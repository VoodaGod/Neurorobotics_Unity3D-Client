using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy
{
	public class DiscrepancyHeadEffects : MonoBehaviour
	{

		[SerializeField]
		[Tooltip("If not set, will be searched in scene")]
		SteamVR_Camera steamvrCam;

		[SerializeField]
		[Range(0, 1)]
		float maxDistToBlack = 0.3f;


		[SerializeField]
		[Range(0, 1)]
		float maxDistToFullBlur = 0.3f;

		//	----------------------------------------------------------------------------------
		//	SuperBlur taken from: https://github.com/PavelDoGreat/Super-Blur
		//	----------------------------------------------------------------------------------
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

			StartCoroutine(ResetEffects()); //reset after each frame
		}

		IEnumerator ResetEffects(){
			yield return new WaitForEndOfFrame();
			FadeToBlack(0);
			Blur(0);
		}

		//needs to be called every frame it should apply
		public void FadeToBlack(Discrepancy disc)
		{
			FadeToBlack(disc.distance);
		}
		void FadeToBlack(float distance)
		{
			float alpha = distance / maxDistToBlack;
			SteamVR_Fade.Start(newColor: new Color(0, 0, 0, alpha), duration: 0);
		}

		//needs to be called every frame it should apply
		public void Blur(Discrepancy disc)
		{
			Blur(disc.distance);
		}
		void Blur(float distance)
		{
			float strength = distance / maxDistToFullBlur;
			superBlur.interpolation = strength;
		}
	}
}
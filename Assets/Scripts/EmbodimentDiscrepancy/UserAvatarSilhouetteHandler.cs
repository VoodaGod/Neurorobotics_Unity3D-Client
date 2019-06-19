using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAvatarSilhouetteHandler : MonoBehaviour {

	[SerializeField]
	[Tooltip("If not set, will be searched in scene")]
	GazeboSceneManager gazeboSceneManager;

	[SerializeField]
	[Tooltip("substring in name of remote UserAvatar Object")]
	string remoteUserAvatarSubstring = "user_avatar";

	[SerializeField]
	[Tooltip("set to Materials/Embodiment/MaskOneZLess")]
	Material mat_maskOneZLess; //sets stencil buffer to 1 behind object


	[SerializeField]
	[Tooltip("set to MeshRenderers (surface & joints) of local user avatar")]
	List<SkinnedMeshRenderer> localUserAvatarMeshRenderers;
	
	[SerializeField]
	Color silhouetteColor = Color.red;
	
	[SerializeField]
	[Tooltip("set to Materials/Embodiment/SilhouetteAlwaysVisible")]
	Material mat_silhouetteAlwaysVisible; //always renders silhouetteColor when behind any object unless an object set the stencilbuffer to 1

	List<Material> materialInstances = new List<Material>();

	// Use this for initialization
	void Start ()
	{
		if (gazeboSceneManager == null)
		{
			gazeboSceneManager = GameObject.FindObjectOfType<GazeboSceneManager>();
			if (gazeboSceneManager == null){
				Debug.LogError("no GazeboSceneManager found");
			}
		}

		if (localUserAvatarMeshRenderers.Count < 2){
			Debug.LogError("not all localUserAvatarMeshRenderers set");
		}

		if (mat_silhouetteAlwaysVisible == null){
			Debug.LogError("silhouetteAlwaysVisible Material not set");
		}

		if (mat_maskOneZLess == null){
			Debug.LogError("maskOneZLess material not set");
		}

		//add instance of silhouetteAlwaysVisibleMaterial to localUserAvatarMeshRenderers' materials
		foreach (SkinnedMeshRenderer renderer in localUserAvatarMeshRenderers)
		{
			var materials = new List<Material>(renderer.materials);
			Material newMat = Instantiate(mat_silhouetteAlwaysVisible);
			materials.Add(newMat);
			renderer.materials = materials.ToArray();
			//get reference to material instance
			int index = materials.IndexOf(newMat);
			materialInstances.Add(renderer.materials[index]);
		}
	}

	// Update is called once per frame
	bool addedMaskToRemoteUserAvatar = false;
	void Update () 
	{
		foreach (Material mat in materialInstances){
			mat.color = silhouetteColor;
		}

		//set masking material to all meshRenderers of remoteUserAvatar object so that localUserAvatar doesn't shine through it
		if (!addedMaskToRemoteUserAvatar && gazeboSceneManager.Models_parent != null)
		{
			foreach (Transform child in gazeboSceneManager.Models_parent.transform)
			{
				if (child.name.Contains(remoteUserAvatarSubstring)){
					MeshRenderer[] meshRenderers = child.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer entry in meshRenderers)
					{
						var materials = new List<Material>(entry.materials);
						materials.Add(mat_maskOneZLess);
						entry.materials = materials.ToArray();
					}
					addedMaskToRemoteUserAvatar = true;
				}
			}	
		}
	}
}

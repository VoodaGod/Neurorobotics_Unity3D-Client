using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFaceRenderingHandler : MonoBehaviour {

	public GameObject gazeboScene;
	
	[SerializeField]
	Material interiorMaterial;

	List<GameObject> initializedGazeboObjects = new List<GameObject>();

	void AddInteriorMaterialToGazeboObjects()
	{
		foreach (MeshRenderer childRenderer in gazeboScene.GetComponentsInChildren<MeshRenderer>())
		{
			if (!initializedGazeboObjects.Contains(childRenderer.gameObject))
			{
				initializedGazeboObjects.Add(childRenderer.gameObject);
				Material newMat = Instantiate(interiorMaterial);
				newMat.mainTexture = childRenderer.material.mainTexture;
				var materials = new List<Material>(childRenderer.materials);
				materials.Add(newMat);
				childRenderer.materials = materials.ToArray();
			}
		}
	}

	void Start(){
	}

	void Update(){
		AddInteriorMaterialToGazeboObjects();
	}

}

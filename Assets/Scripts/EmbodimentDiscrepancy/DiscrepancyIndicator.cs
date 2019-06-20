using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy 
{
	public class DiscrepancyIndicator : MonoBehaviour
	{

		[SerializeField]
		[Tooltip("if not set, will use attached gameObject's transform")]
		Transform pivot;

		//called every frame
		public void PointAtTarget(Transform WorldSpaceTarget)
		{
			pivot.gameObject.SetActive(true);
			//TODO
			Debug.Log("TODO point at Target " + WorldSpaceTarget);
			StartCoroutine(DisableAfterFrame());
		}

		IEnumerator DisableAfterFrame()
		{
			yield return new WaitForEndOfFrame();
			pivot.gameObject.SetActive(false);
		}

		// Use this for initialization
		void Start()
		{
			if (pivot == null){
				pivot = this.transform;
			}

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
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

		[SerializeField]
		[Tooltip("just to monitor what it's pointing at")]
		Transform target;

		//called every frame
		public void PointAtTarget(Transform WorldSpaceTarget)
		{
			target = WorldSpaceTarget;
			pivot.gameObject.SetActive(true);

			//Math from: https://youtu.be/b_dDgnfedIo
			//Indicator Sprite should be pointing down at rotation.z==0
			Vector3 dir = target.position;
			Vector3 rotation = new Vector3(0,0,0);
			rotation.z = Mathf.Atan2((pivot.transform.position.y - dir.y), (pivot.transform.position.x - dir.x)) * Mathf.Rad2Deg - 90;
			pivot.transform.rotation = Quaternion.Euler(rotation);

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
			PointAtTarget(target);

		}
	}
}
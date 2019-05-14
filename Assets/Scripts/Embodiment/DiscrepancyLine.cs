using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DiscrepancyLine : MonoBehaviour
{

	[SerializeField]
	LineRenderer lineRenderer;
	Transform source, target;

	// Use this for initialization
	void Start()
	{
		lineRenderer.positionCount = 2;
		lineRenderer.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void DrawLine(Transform source, Transform target, float thickness)
	{
		lineRenderer.SetPosition(0, source.position);
		lineRenderer.SetPosition(1, target.position);
		lineRenderer.widthMultiplier = thickness;
		lineRenderer.enabled = true;
	}

	public void StopDrawLine(){
		lineRenderer.enabled = false;
	}

}

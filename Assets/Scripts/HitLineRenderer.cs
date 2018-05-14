using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLineRenderer : MonoBehaviour
{
	public LineRenderer line;
	public float duration = 5f;
	public Vector3 startPoint;
	public Vector3 endPoint;
	
	public void Activate (float dura, Vector3 start, Vector3 end)
	{
		line = GetComponent<LineRenderer>();
		duration = dura;
		startPoint = start;
		endPoint = end;
		line.positionCount = 2;
		line.SetPosition(0, startPoint);
		line.SetPosition(1, endPoint);
		StartCoroutine(LifeTime());
	}
	public void Activate(float dura, Vector3 start, Vector3 end, Color c)
	{
		line = GetComponent<LineRenderer>();
		duration = dura;
		startPoint = start;
		endPoint = end;
		line.positionCount = 2;
		line.SetPosition(0, startPoint);
		line.SetPosition(1, endPoint);
		line.startColor = c;
		line.endColor = c;
		StartCoroutine(LifeTime());
	}
	IEnumerator LifeTime ()
	{
		yield return new WaitForSeconds(duration);
		Destroy(gameObject);
	}
}

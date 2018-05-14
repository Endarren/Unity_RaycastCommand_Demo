///Written by: Endar
///
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class RaycastDemo : MonoBehaviour
{
	#region Fields
	/// <summary>
	/// The prefab used to display hit lines, if useDebugLine is false.
	/// </summary>
	public GameObject linePrefab;
	/// <summary>
	/// The directions for each ray cast.
	/// </summary>
	public List<Vector3> castDirections = new List<Vector3>();
	/// <summary>
	/// The distances for each ray cast.
	/// </summary>
	public List<float> castDistances = new List<float>();
	/// <summary>
	/// Sets the max hit for the RaycastCommand.  Currently, it does not work.
	/// </summary>
	public int maxHits = 1;
	/// <summary>
	/// The layers hit by the cast.
	/// </summary>
	public LayerMask hitLayers;
	/// <summary>
	/// The minimum number of commands per job.
	/// </summary>
	public int minCommandsPerJob = 10;
	/// <summary>
	/// If true, it will draw hit lines with Debug.  Otherwise, it will use linerenderers.
	/// </summary>
	public bool useDebugLine = false;
	/// <summary>
	/// The color used to display a raycast that hit something.
	/// </summary>
	public Color hitLineColor = Color.red;
	/// <summary>
	/// How long in seconds the hit line will last.
	/// </summary>
	public float lineDuration = 5f;
	/// <summary>
	/// The number of casts for RandomizeCasts. 
	/// </summary>
	public int castCount = 3;
	/// <summary>
	/// The min distance for RandomizeCasts.
	/// </summary>
	public float minDistance = 1f;
	/// <summary>
	/// The max distance for RandomizeCasts.
	/// </summary>
	public float maxDistance = 1f;
	/// <summary>
	/// Handles the raycast job.
	/// </summary>
	JobHandle jobHandle;
	/// <summary>
	/// This array is used by the ray cast jobs to know what to do for each raycast.
	/// </summary>
	NativeArray<RaycastCommand> raycastCommands;
	/// <summary>
	/// Stores the results of each RaycastCommand.
	/// </summary>
	NativeArray<RaycastHit> commandHits;
	#endregion
	#region Methods
	/// <summary>
	/// Sets the number of casts to do.
	/// </summary>
	/// <param name="s"></param>
	public void SetCastCount (string s)
	{
		castCount = int.Parse(s);
	}
	public void SetMinDistance (string s)
	{
		minDistance = float.Parse(s);
	}
	public void SetMaxDistance(string s)
	{
		maxDistance = float.Parse(s);
	}
	/// <summary>
	/// Creates random casts in the XY directions.
	/// </summary>
	public void RandomizeCasts ()
	{
		castDirections.Clear();
		castDistances.Clear();
		for (int i = 0; i < castCount;i++)
		{
			castDirections.Add(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f));
			castDistances.Add(Random.Range(minDistance, maxDistance));
		}
	}
	/// <summary>
	/// Does several raycasts using the RaycasCommand job.
	/// </summary>
	public void DoRayCastCommands ()
	{
		//Initialize array to store the results of all the raycastcommands.
		//Number equals the number of casts times max hits.
		commandHits = new NativeArray<RaycastHit>(castDirections.Count * maxHits, Allocator.Temp);
		//Create the raycastcommands.
		raycastCommands = new NativeArray<RaycastCommand>(castDirections.Count, Allocator.Temp);
		for (int i = 0; i < castDirections.Count; i++)
		{
			if (castDistances.Count <= i)
				raycastCommands [i] = new RaycastCommand(transform.position, castDirections [i], castDistances [castDistances.Count - 1], hitLayers, maxHits);
			else
				raycastCommands [i] = new RaycastCommand(transform.position, castDirections [i], castDistances [i], hitLayers, maxHits);
		}
		//Schedule the job
		jobHandle = RaycastCommand.ScheduleBatch(raycastCommands, commandHits, minCommandsPerJob);

		jobHandle.Complete();
		GameObject temp;
		for (int i = 0; i < commandHits.Length;i++)
		{
			//A result hit something if the collider is not null.
			if (commandHits [i].collider != null)
			{
				if (!useDebugLine)
				{
					temp = Instantiate(linePrefab);
					temp.transform.position = transform.position;
					HitLineRenderer hlr = temp.GetComponent<HitLineRenderer>();
					hlr.Activate(lineDuration, transform.position, commandHits [i].point, hitLineColor);
				}
				else
					Debug.DrawLine(transform.position, commandHits [i].point, hitLineColor, lineDuration);
			}
		}
		//Dipose of results to prevent memory leak.
		commandHits.Dispose();
		raycastCommands.Dispose();
	}
	/// <summary>
	/// Does the same ray casts, just using the orignal method.
	/// </summary>
	public void DoRayCastOld()
	{
		RaycastHit hit = new RaycastHit();
		bool didHit = false;
		GameObject temp;
		for (int i = 0; i < castDirections.Count;i++)
		{
			if (castDistances.Count <= i)
			{
				didHit = Physics.Raycast(transform.position, castDirections [i], castDistances [castDistances.Count - 1], hitLayers);
			}
			else
			{
				didHit = Physics.Raycast(transform.position, castDirections [i], castDistances [i], hitLayers);
			}
			if (didHit)
			{
				if (!useDebugLine)
				{
					temp = Instantiate(linePrefab);
					temp.transform.position = transform.position;
					HitLineRenderer hlr = temp.GetComponent<HitLineRenderer>();
					hlr.Activate(lineDuration, transform.position, hit.point, hitLineColor);
				}
				else
					Debug.DrawLine(transform.position, hit.point, hitLineColor, lineDuration);
			}

		}
	}
	#endregion
}

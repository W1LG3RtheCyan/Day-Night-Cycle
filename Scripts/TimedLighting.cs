using UnityEngine;
using System.Collections;

[AddComponentMenu("Day Night Cycle/Timed Lighting")]
public class TimedLighting : MonoBehaviour
{
	public void OnEnable()
	{
		Messenger.AddListener<bool> ("Morning Light Time", OnToggleLight);
	}

	public void OnDisable()
	{
		Messenger.AddListener<bool> ("Morning Light Time", OnToggleLight);
	}

	private void OnToggleLight(bool b)
	{
		if (b)
		{
			GetComponent<Light> ().enabled = false;
		}
		else
		{
			GetComponent<Light> ().enabled = true;
		}
	}
}

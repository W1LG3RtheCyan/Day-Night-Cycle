using UnityEngine;
using System.Collections;

[AddComponentMenu("Day Night Cycle/Sun")]
public class Sun : MonoBehaviour
{
	public float _maxFlareBrightness;
	public float _minFlareBrightness;

	public float _maxLightBrightness;
	public float _minLightBrightness;

	public bool givesLight = false;

	void Start()
	{
		if (GetComponent<Light> ()) {
			givesLight = true;
		}
		else
		{
			givesLight = false;
		}
	}
}

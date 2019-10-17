/// <summary>
/// GameTime.cs
/// Bryan Wilger
/// Sept 12, 2016
/// 
/// Script that controls the day/night cycle, as well as in-game time;
/// </summary>
using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
	public enum TimeOfDay
	{
		Idle,
		SunRise,
		Sunset
	}
	public Transform[] sun;					//array to hold suns
	public float dayCycleInMinutes = 1;		//how many minutes the day will last for

	public float sunRise;					//time of sunrise
	public float sunSet;					//time of sunset
	public float skyBoxBlendModifier;		//speed at which the skybox textures blend

	public Color ambientLightMax;
	public Color ambientLightMin;

	public float morningLight;
	public float nightLight;

	private bool _morningEh = false;

	private Sun[] _sunScript;				//
	private float _degreeRotation;			//
	private float _timeOfDay;				//

	private float _dayCycleInSeconds;		//number of seconds in the day

	private float _halfDayLength;

	private const float SECOND = 1;
	private const float MINUTE = 60 * SECOND;
	private const float HOUR = 60 * MINUTE;
	private const float DAY = 24 * HOUR;
	private const float DEGREES_PER_SECOND = 360 / DAY;

	private TimeOfDay _time;
	private float _noon;

	// Use this for initialization
	void Start ()
	{
		_dayCycleInSeconds = dayCycleInMinutes * MINUTE;

		RenderSettings.skybox.SetFloat ("_Blend", 0);

		_sunScript = new Sun[sun.Length];

		for (int cnt = 0; cnt < sun.Length; cnt++)
		{
			Sun temp = sun[cnt].GetComponent<Sun> ();

			if (temp == null)
			{
				Debug.LogWarning ("Sun script not found");
				sun [cnt].gameObject.AddComponent<Sun> ();
				temp = sun [cnt].GetComponent<Sun> ();
			}

			_sunScript [cnt] = temp;
		}

		_timeOfDay = 0;
		_degreeRotation = DEGREES_PER_SECOND * DAY / (_dayCycleInSeconds);

		sunRise *= _dayCycleInSeconds;
		sunSet *= _dayCycleInSeconds;
		_noon = (sunRise + sunSet) / 2;
		_halfDayLength = sunSet - _noon;
		morningLight *= _dayCycleInSeconds;
		nightLight *= _dayCycleInSeconds;


		SetUpLighting ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//update the time of day
		_timeOfDay += Time.deltaTime;

		//if the day timer is over the limit of how long a day lasts, reset it
		if (_timeOfDay > _dayCycleInSeconds)
		{
			_timeOfDay -= _dayCycleInSeconds;
		}

		//control the outside lighting effects based on the time of day
		if(!_morningEh && _timeOfDay > morningLight && _timeOfDay < nightLight)
		{
			_morningEh = true;
			Messenger.Broadcast<bool>("Morning Light Time", true);
		}
		else if(_morningEh && _timeOfDay > nightLight)
		{
			_morningEh = false;
			Messenger.Broadcast<bool>("Morning Light Time", false);
		}

		//position the un in the sky by adjusting the angle that the flare is shining from
		for (int cnt = 0; cnt < sun.Length; cnt++)
		{
			sun [cnt].Rotate (new Vector3 (_degreeRotation, 0, 0) * Time.deltaTime);
		}


		if (_timeOfDay > sunRise && _timeOfDay < _noon) {
			AdjustLighting (true);
		}
		else if(_timeOfDay > _noon && _timeOfDay < sunSet)
		{
			AdjustLighting (false);
		}

		if (_timeOfDay > sunRise && _timeOfDay < sunSet && RenderSettings.skybox.GetFloat ("_Blend") < 1)
		{
			_time = GameTime.TimeOfDay.SunRise;
			BlendSkyBox ();
		}
		else if (_timeOfDay > sunSet && RenderSettings.skybox.GetFloat ("_Blend") > 0)
		{
			_time = GameTime.TimeOfDay.Sunset;
			BlendSkyBox();
		}
		else
		{
			_time = GameTime.TimeOfDay.Idle;
		}
	}

	private void BlendSkyBox()
	{
		float temp = 0;

		switch (_time)
		{
		case TimeOfDay.SunRise:
			temp = (_timeOfDay - sunRise)/_dayCycleInSeconds * skyBoxBlendModifier;
			break;
		case TimeOfDay.Sunset:
			temp = (_timeOfDay - sunSet) / _dayCycleInSeconds * skyBoxBlendModifier;
			temp = 1 - temp;
			break;
		}
			
		if (temp > 1)
		{
			temp = 1 - (temp - 1);
		}

		RenderSettings.skybox.SetFloat ("_Blend", temp);
	}

	private void SetUpLighting()
	{
		RenderSettings.ambientLight = ambientLightMin;

		for (int cnt = 0; cnt < _sunScript.Length; cnt++)
		{
			if (_sunScript [cnt].givesLight == true)
			{
				sun [cnt].GetComponent<Light> ().intensity = _sunScript [cnt]._minLightBrightness;
			}
		}
	}
	private void AdjustLighting(bool brighten)
	{
		Debug.Log (brighten);

		float pos = 0;

		if (brighten)
		{
			pos = (_timeOfDay - sunRise / _halfDayLength);
		}
		else
		{
			pos = (sunSet - _timeOfDay / _halfDayLength);
		}

		RenderSettings.ambientLight = new Color(ambientLightMin.r + ambientLightMax.r * pos, ambientLightMin.g + ambientLightMax.g * pos, ambientLightMin.b + ambientLightMax.b * pos);

		for (int cnt = 0; cnt < _sunScript.Length; cnt++)
		{
			if (_sunScript [cnt].givesLight == true)
			{
				sun [cnt].GetComponent<Light> ().intensity = _sunScript [cnt]._maxLightBrightness * pos;
			}
		}
	}
}

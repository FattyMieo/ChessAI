using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporarySpinningScript : MonoBehaviour
{
	public float speed = 5.0f;
	private int step = 0;

	// Update is called once per frame
	void Update ()
	{
		float curAngle = transform.rotation.eulerAngles.y;
		float toAngle = 90.0f * step;

		if(Mathf.Abs(curAngle - toAngle) > 1.0f)
		{
			transform.rotation = Quaternion.Euler(Vector3.up * Mathf.Lerp(curAngle, toAngle, Time.deltaTime * speed));
		}
		else
		{
			transform.rotation = Quaternion.Euler(Vector3.up * toAngle);

			if(step >= 4) step = 0;
			step++;
		}
	}
}

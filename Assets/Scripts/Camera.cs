using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class Camera : MonoBehaviour
{
	void Start()
	{

	}
	void Update()
	{

	}

	public IEnumerator CameraShake(int _loops)  //the more loops the more time the camera shakes, 2 is an average shake
	{
		Vector3 _originalPos = transform.position;
		while (_loops > 0)
		{
			transform.position = _originalPos + new Vector3(0.1f, 0.1f, 0f);
			yield return new WaitForSeconds(0.05f);
			transform.position = _originalPos + new Vector3(-0.1f, -0.1f, 0f);
			yield return new WaitForSeconds(0.05f);
			transform.position = _originalPos;
			_loops--;
			yield return null;
		}
	}
}
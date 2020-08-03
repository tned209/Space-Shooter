using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLaser : MonoBehaviour
{
    private float _xlaserspeed = -80f;
    private float _ylaserspeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveEffect());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(_xlaserspeed, _ylaserspeed, 0) * Time.deltaTime);
        if (transform.position.y >= 11.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    IEnumerator WaveEffect()
    {
        yield return new WaitForSeconds(0.05f);
        while (true)
        {
            _xlaserspeed = 80f;
            yield return new WaitForSeconds(0.1f);
            _xlaserspeed = -80f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}

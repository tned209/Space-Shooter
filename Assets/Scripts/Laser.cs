﻿using UnityEngine;

public class Laser : MonoBehaviour
{
    private float _laserspeed = 16f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _laserspeed * Time.deltaTime);
        if (transform.position.y >= 11.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator_mobile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.deltaPosition.x > 0f)
            {
                this.GetComponent<CameraController>().angle -= 2;
            }
            if (touch.deltaPosition.x < 0f)
            {
                this.GetComponent<CameraController>().angle += 2;
            }
        }
    }
}

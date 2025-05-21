using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int angle = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //this.transform.rotation = Quaternion.AngleAxis(1, Vector3.up);
            angle -= 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle += 1;
        }
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}

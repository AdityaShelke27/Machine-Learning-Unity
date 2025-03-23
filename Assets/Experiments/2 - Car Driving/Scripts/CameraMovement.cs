using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    Vector3 rot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotation();
    }

    void Move()
    {
        float hor = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float ver = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.position += transform.forward * ver + transform.right * hor;
    }

    void Rotation()
    {
        float hor = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float ver = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        //Vector3 rot = transform.eulerAngles;
        rot += new Vector3(-ver, hor, 0);
        transform.rotation = Quaternion.Euler(rot);
    }
}

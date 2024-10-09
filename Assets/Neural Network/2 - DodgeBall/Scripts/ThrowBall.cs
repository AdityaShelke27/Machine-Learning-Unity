using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    public GameObject sphere;
    public GameObject cube;
    public Material red;
    public Material green;
    DPerceptron perceptron;
    // Start is called before the first frame update
    void Start()
    {
        perceptron = GetComponent<DPerceptron>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            GameObject obj = Instantiate(sphere, transform.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().AddForce(0, 0, 500);
            obj.GetComponent<MeshRenderer>().material = red;

            perceptron.GetInput(0, 0, 0);

            Destroy(obj, 3);
        }
        else if (Input.GetKeyDown("2"))
        {
            GameObject obj = Instantiate(sphere, transform.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().AddForce(0, 0, 500);
            obj.GetComponent<MeshRenderer>().material = green;

            perceptron.GetInput(0, 1, 1);

            Destroy(obj, 3);
        }
        else if (Input.GetKeyDown("3"))
        {
            GameObject obj = Instantiate(cube, transform.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().AddForce(0, 0, 500);
            obj.GetComponent<MeshRenderer>().material = red;

            perceptron.GetInput(1, 0, 1);

            Destroy(obj, 3);
        }
        else if (Input.GetKeyDown("4"))
        {
            GameObject obj = Instantiate(cube, transform.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().AddForce(0, 0, 500);
            obj.GetComponent<MeshRenderer>().material = green;

            perceptron.GetInput(1, 1, 1);

            Destroy(obj, 3);
        }
    }
}

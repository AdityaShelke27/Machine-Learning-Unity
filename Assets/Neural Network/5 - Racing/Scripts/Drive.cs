using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public float speed = 50f;
    public float rotationSpeed = 100f;
    public float visibleDistance = 200f;
    StreamWriter streamWriter;
    [SerializeField] List<string> trainingData = new List<string>();
    StringBuilder strBuilder = new StringBuilder();
    // Start is called before the first frame update
    void Start()
    {
        streamWriter = File.CreateText(Application.dataPath + "\\TrainingData.txt");
        Debug.Log(streamWriter);
    }

    private void OnApplicationQuit()
    {
        foreach (string data in trainingData)
        {
            Debug.Log(data);
            streamWriter.WriteLine(data);
        }

        streamWriter.Close();
    }

    // Update is called once per frame
    void Update()
    {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        
        float translation = translationInput * speed * Time.deltaTime;
        float rotation = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, -transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.forward * visibleDistance, Color.red);

        RaycastHit hit;
        float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0;

        if(Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            fDist = 1 - Round(hit.distance / visibleDistance);
        }
        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rDist = 1 - Round(hit.distance / visibleDistance);
        }
        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            lDist = 1 - Round(hit.distance / visibleDistance);
        }
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, Vector3.up) * transform.forward, out hit, visibleDistance))
        {
            r45Dist = 1 - Round(hit.distance / visibleDistance);
        }
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.forward, out hit, visibleDistance))
        {
            l45Dist = 1 - Round(hit.distance / visibleDistance);
        }
        strBuilder.Append($"{fDist},{rDist},{lDist},{r45Dist},{l45Dist},{Round(translationInput)},{Round(rotationInput)}");
        if(!trainingData.Contains(strBuilder.ToString()))
        {
            trainingData.Add(strBuilder.ToString());
        }
        strBuilder.Clear();
    }

    float Round(float x)
    {
        return (float) System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2;
    }
}

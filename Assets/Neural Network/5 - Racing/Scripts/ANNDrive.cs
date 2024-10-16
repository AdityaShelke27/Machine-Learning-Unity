using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ANNDrive : MonoBehaviour
{
    ANN ann;
    public float visibleDistance = 200;
    public int epochs = 1000;
    public float speed = 50;
    public float rotationSpeed = 100;
    public double learningRate;
    [SerializeField] Activation hiddenActivation;
    [SerializeField] Activation outputActivation;

    bool trainingDone = false;
    float trainingProgress = 0;
    double sse = 0;
    double lastsse = 1;

    public float translation;
    public float rotation;

    List<List<double>> inputs = new();
    List<List<double>> outputs = new();

    StreamReader reader;
    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(5, 2, 1, 10, learningRate, hiddenActivation, outputActivation);
        LoadTrainingData();
        StartCoroutine(TrainANN());
    }

    // Update is called once per frame
    void Update()
    {
        if (!trainingDone)
        {
            return;
        }
        Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, -transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.forward * visibleDistance, Color.red);

        RaycastHit hit;
        float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0;

        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
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

        List<double> inputs = new List<double>() { fDist, rDist, lDist, r45Dist, l45Dist };

        List<double> outputs = ann.Test(inputs);

        float translationInput = (float)Map(-1, 1, 0, 1, (float)outputs[0]);
        float rotationInput = (float)Map(-1, 1, 0, 1, (float)outputs[1]);

        translation = translationInput * speed * Time.deltaTime;
        rotation = rotationInput * rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastsse);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + ann.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress);
    }
    IEnumerator TrainANN()
    {
        for (int i = 0; i < epochs; i++)
        {
            sse = 0;
            string currentWeights = ann.PrintWeights();
            for (int j = 0; j < inputs.Count; j++)
            {
                List<double> outP = ann.Train(inputs[j], outputs[j]);

                double error = Mathf.Pow((float)outputs[j][0] - (float)outP[0], 2) + Mathf.Pow((float)outputs[j][1] - (float)outP[1], 2);
                sse += error;
            }

            trainingProgress = (float)i / epochs;
            sse /= inputs.Count;
            if (lastsse < sse)
            {
                ann.LoadWeights(currentWeights);
                ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
            }
            else
            {
                ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
                lastsse = sse;
            }

            yield return null;
        }

        trainingDone = true;
    }
    void LoadTrainingData()
    {
        string path = Application.dataPath + "\\TrainingData.csv";
        if (File.Exists(path))
        {
            reader = File.OpenText(path);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');

                double out1 = double.Parse(values[5]);
                double out2 = double.Parse(values[6]);

                if (out1 * out2 != 0)
                {
                    List<double> inp = new();
                    List<double> outp = new();
                    for (int i = 0; i < 5; i++)
                    {
                        inp.Add(double.Parse(values[i]));
                    }

                    outp.Add(Map(0, 1, -1, 1, System.Convert.ToSingle(out1)));
                    outp.Add(Map(0, 1, -1, 1, System.Convert.ToSingle(out2)));

                    inputs.Add(inp);
                    outputs.Add(outp);
                }
            }

            reader.Close();
        }
        else
        {
            Debug.Log("FILE DOES NOT EXISTS");
        }
    }

    double Map(float newFrom, float newTo, float oldFrom, float oldTo, float value)
    {
        if (value <= oldFrom)
        {
            return newFrom;
        }
        else if (value >= oldTo)
        {
            return newTo;
        }
        else
        {
            return ((value - oldFrom) * ((newTo - newFrom) / (oldTo - oldFrom))) + newFrom;
        }
    }
    float Round(float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2;
    }
}

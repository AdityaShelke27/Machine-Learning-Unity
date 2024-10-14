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
        ann = new ANN(5, 2, 1, 10, 0.5);
        LoadTrainingData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void TrainANN()
    {
        for(int i = 0; i < epochs; i++)
        {
            sse = 0;

            for(int j = 0; j < inputs.Count; j++)
            {
                List<double> outP = ann.Train(inputs[j], outputs[j]);

                double error = Mathf.Pow((float)outputs[j][0] - (float)outP[0], 2) + Mathf.Pow((float)outputs[j][1] - (float)outP[1], 2);
                sse += error;
            }

            trainingProgress = i / epochs;

            sse /= inputs.Count;
            lastsse = sse;
        }
    }
    void LoadTrainingData()
    {
        string path = Application.dataPath + "\\TrainingData.txt";
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
}

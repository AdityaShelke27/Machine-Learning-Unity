using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class DPerceptron : MonoBehaviour
{
    List<TrainingSet> trainingSet = new();
    public double[] weights;
    public double bias;

    public Animator player;
    // Start is called before the first frame update
    void Start()
    {
        InitializeWeights();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            Save();
        }
        else if (Input.GetKeyDown("l"))
        {
            Load();
        }
    }
    void Load()
    {
        Debug.Log(Application.persistentDataPath + "\\weights.txt");
        using (StreamReader sr = File.OpenText(Application.persistentDataPath + "\\weights.txt"))
        {
            string[] list = sr.ReadLine().Split(',');

            weights[0] = System.Convert.ToDouble(list[0]);
            weights[1] = System.Convert.ToDouble(list[1]);
            bias = System.Convert.ToDouble(list[2]);
        }
    }
    void Save()
    {
        StreamWriter file = File.CreateText(Application.persistentDataPath + "\\weights.txt");
        file.WriteLine(weights[0] + "," + weights[1] + "," + bias);
        file.Close();
    }
    public void GetInput(int shape, int color, int output)
    {
        int action = (int) CalcOutput(shape, color);

        if(action == 0)
        {
            player.SetTrigger("Crouch");
        }
        TrainingSet dataPoint = new()
        {
            inputSet = new double[] { shape, color },
            outputSet = output
        };

        trainingSet.Add(dataPoint);

        Train();
    }
    void InitializeWeights()
    {
        weights = new double[] { Random.Range(-1f, 1f), Random.Range(-1f, 1f) };
        bias = Random.Range(-1f, 1f);
    }
    void Train()
    {
        for (int j = 0; j < trainingSet.Count; j++)
        {
            UpdateWeights(j);
        }
    }
    void UpdateWeights(int j)
    {
        double error = trainingSet[j].outputSet - CalcOutput(j);

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] += error * trainingSet[j].inputSet[i];
        }

        bias += error;
    }
    double CalcOutput(int i)
    {
        double dp = DotProductBias(trainingSet[i].inputSet, weights);

        if (dp > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    double CalcOutput(double x, double y)
    {
        double dp = DotProductBias(new double[] { x, y }, weights);

        if (dp > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    double DotProductBias(double[] input, double[] weights)
    {
        if (input.Length != weights.Length)
        {
            return -1;
        }

        double output = 0;
        for (int i = 0; i < input.Length; i++)
        {
            output += weights[i] * input[i];
        }

        output += bias;

        return output;
    }
}

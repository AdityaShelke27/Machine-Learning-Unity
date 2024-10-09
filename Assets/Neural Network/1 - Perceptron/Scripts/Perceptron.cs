using UnityEngine;

[System.Serializable]
public class TrainingSet
{
    public double[] inputSet;
    public double outputSet;
}
public class Perceptron : MonoBehaviour
{
    public TrainingSet[] trainingSet;
    public SimpleGrapher graph;
    public double[] weights;
    public double bias;
    public double totalError;
    public int epochs;
    // Start is called before the first frame update
    void Start()
    {
        DrawAllPoints();
        Train(epochs);
        Debug.Log(CalcOutput(0));
        Debug.Log(CalcOutput(1));
        Debug.Log(CalcOutput(2));
        Debug.Log(CalcOutput(3));
        graph.DrawRay((float) -(weights[0] / weights[1]), (float) -(bias / weights[1]), Color.white);

        double x = 0.5, y = 0.5;

        if(CalcOutput(x, y) == 1)
        {
            graph.DrawPoint((float) x, (float) y, Color.yellow);
        }
        else
        {
            graph.DrawPoint((float) x, (float) y, Color.blue);
        }
    }
    
    void DrawAllPoints()
    {
        for(int i = 0; i < trainingSet.Length; i++)
        {
            if (trainingSet[i].outputSet == 0)
            {
                graph.DrawPoint((float) trainingSet[i].inputSet[0], (float) trainingSet[i].inputSet[1], Color.red);
            }
            else
            {
                graph.DrawPoint((float)trainingSet[i].inputSet[0], (float)trainingSet[i].inputSet[1], Color.green);
            }
        }
    }
    void InitializeWeights()
    {
        weights = new double[] { Random.Range(-1f, 1f), Random.Range(-1f, 1f) };
        bias = Random.Range(-1f, 1f);
    }
    void Train(int epochs)
    {
        InitializeWeights();
        
        for(int i = 0; i < epochs; i++)
        {
            totalError = 0;
            for (int j = 0; j < trainingSet.Length; j++)
            {
                UpdateWeights(j);
                Debug.Log(j + ") " + weights[0] + " " + weights[1] + " " + bias);
            }
            Debug.Log("Total Error: " + totalError);
        }
    }
    void UpdateWeights(int j)
    {
        double error = trainingSet[j].outputSet - CalcOutput(j);
        totalError += error;
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] += error * trainingSet[j].inputSet[i];
        }

        bias += error;
    }
    double CalcOutput(int i)
    {
        double dp = DotProductBias(trainingSet[i].inputSet, weights);

        if(dp > 0)
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
        double dp = DotProductBias(new double[] {x, y}, weights);

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
        if(input.Length != weights.Length)
        {
            return -1;
        }

        double output = 0;
        for(int i = 0; i < input.Length; i++)
        {
            output += weights[i] * input[i];
        }

        output += bias;

        return output;
    }
}

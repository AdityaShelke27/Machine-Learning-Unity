using System.Collections.Generic;
using UnityEngine;

public class ANNBrain : MonoBehaviour
{
    ANN ann;
    double sumSquareError = 0;
    public int epochs;
    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(2, 1, 1, 2, 0.8);

        List<double> result;
        for (int i = 0; i < epochs; i++)
        {
            sumSquareError = 0;
            double desiredOutput = 1;
            result = Train(1, 1, desiredOutput);
            sumSquareError += Mathf.Pow((float)(result[0] - desiredOutput), 2);
            desiredOutput = 0;
            result = Train(1, 0, desiredOutput);
            sumSquareError += Mathf.Pow((float)(result[0] - desiredOutput), 2);
            desiredOutput = 0;
            result = Train(0, 1, desiredOutput);
            sumSquareError += Mathf.Pow((float)(result[0] - desiredOutput), 2);
            desiredOutput = 1;
            result = Train(0, 0, desiredOutput);
            sumSquareError += Mathf.Pow((float)(result[0] - desiredOutput), 2);
        }

        Debug.Log("SSE: " + sumSquareError);

        result = Test(1, 1);
        Debug.Log("1 1 " + result[0]);
        result = Test(1, 0);
        Debug.Log("1 0 " + result[0]);
        result = Test(0, 1);
        Debug.Log("0 1 " + result[0]);
        result = Test(0, 0);
        Debug.Log("0 0 " + result[0]);
    }

    List<double> Train(double i1, double i2, double o)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        inputs.Add(i1);
        inputs.Add(i2);
        outputs.Add(o);

        return ann.Train(inputs, outputs);
    }

    List<double> Test(double i1, double i2)
    {
        List<double> inputs = new()
        {
            i1,
            i2
        };

        return ann.Test(inputs);
    }
}

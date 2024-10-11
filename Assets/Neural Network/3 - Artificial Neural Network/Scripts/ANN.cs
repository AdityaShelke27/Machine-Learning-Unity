using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANN
{
    public int numInputs;
    public int numOutputs;
    public int numHidden;
    public int numNPerHidden;
    public double alpha;
    public List<Layer> layers = new List<Layer>();

    public ANN(int nI, int nO, int nH, int nPH, double a)
    {
        numInputs = nI;
        numOutputs = nO;
        numHidden = nH;
        numNPerHidden = nPH;
        alpha = a;

        if(numHidden > 0)
        {
            layers.Add(new Layer(numInputs, numNPerHidden));

            for(int i = 0; i < numHidden - 1; i++)
            {
                layers.Add(new Layer(numNPerHidden, numNPerHidden));
            }

            layers.Add(new Layer(numNPerHidden, numOutputs));
        }
        else
        {
            layers.Add(new Layer(numInputs, numOutputs));
        }
    }

    public List<double> Train(List<double> inputValues, List<double> desiredOutput)
    {
        List<double> output = Test(inputValues);
        UpdateWeights(output, desiredOutput);

        return output;
    }

    public List<double> Test(List<double> inputValues)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        if (inputValues.Count != numInputs)
        {
            Debug.Log("ERROR: Number of Inputs must be " + numInputs);
            return outputs;
        }

        inputs = new List<double>(inputValues);

        for (int i = 0; i < numHidden + 1; i++)
        {
            if (i > 0)
            {
                inputs = new List<double>(outputs);
            }
            outputs.Clear();

            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                double N = 0;
                layers[i].neurons[j].inputs.Clear();

                for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
                {
                    layers[i].neurons[j].inputs.Add(inputs[k]);
                    N += inputs[k] * layers[i].neurons[j].weights[k];
                }

                N -= layers[i].neurons[j].bias;
                if(i == numHidden)
                {
                    layers[i].neurons[j].output = ActivationO(N);
                }
                else
                {
                    layers[i].neurons[j].output = Activation(N);
                }
                outputs.Add(layers[i].neurons[j].output);
            }
        }

        return outputs;
    }

    void UpdateWeights(List<double> outputs, List<double> desiredOutputs)
    {
        double error;

        for(int i = numHidden; i >= 0; i--)
        {
            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                if(i == numHidden)
                {
                    
                    error = desiredOutputs[j] - outputs[j];
                    layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error;
                }
                else
                {
                    double sumError = 0;
                    for (int k = 0; k < layers[i + 1].numNeurons; k++)
                    {
                        sumError += layers[i + 1].neurons[k].errorGradient * layers[i + 1].neurons[k].weights[j];
                    }

                    layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1 - layers[i].neurons[j].output) * sumError;
                }

                for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
                {
                    if(i == numHidden)
                    {
                        error = desiredOutputs[j] - outputs[j];
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
                    }
                    else
                    {
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
                    }
                }

                layers[i].neurons[j].bias -= alpha * layers[i].neurons[j].errorGradient;
            }
        }
    }

    double Activation(double input)
    {
        return Tanh(input);
    }
    double ActivationO(double input)
    {
        return Sigmoid(input);
    }
    double Sigmoid(double input)
    {
        double expo = System.Math.Exp(input);
        return expo / (1 + expo);   
    }
    double Step(double input)
    {
        if(input > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    double Tanh(double input)
    {
        double expo = System.Math.Exp(-2 * input);
        return 2 / (1f + expo) - 1;
    }

    double ReLu(double input)
    {
        if(input > 0)
        {
            return input;
        }
        else
        {
            return 0.01 * input;
        }
    }
}

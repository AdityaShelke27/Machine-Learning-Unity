using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Activation
{
    Sigmoid,
    Step,
    Tanh,
    ReLu
}
public class ANN
{
    public int numInputs;
    public int numOutputs;
    public int numHidden;
    public int numNPerHidden;
    Activation hiddenActivation;
    Activation outputActivation;
    public double alpha;
    public List<Layer> layers = new List<Layer>();

    public ANN(int nI, int nO, int nH, int nPH, double a, Activation hiddenActivation, Activation outputActivation)
    {
        numInputs = nI;
        numOutputs = nO;
        numHidden = nH;
        numNPerHidden = nPH;
        alpha = a;

        if (numHidden > 0)
        {
            layers.Add(new Layer(numInputs, numNPerHidden));

            for (int i = 0; i < numHidden - 1; i++)
            {
                layers.Add(new Layer(numNPerHidden, numNPerHidden));
            }

            layers.Add(new Layer(numNPerHidden, numOutputs));
        }
        else
        {
            layers.Add(new Layer(numInputs, numOutputs));
        }

        this.hiddenActivation = hiddenActivation;
        this.outputActivation = outputActivation;
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

    public void CombineWeights(ANN parent1, ANN parent2)
    {
        for(int i = 0; i < parent1.layers.Count; i++)
        {
            for (int j = 0; j < parent1.layers[i].neurons.Count; j++)
            {
                for (int k = 0; k < parent1.layers[i].neurons[j].weights.Count; k++)
                {
                    layers[i].neurons[j].weights[k] = (parent1.layers[i].neurons[j].weights[k] + parent2.layers[i].neurons[j].weights[k]) / 2;
                }

                layers[i].neurons[j].bias = (parent1.layers[i].neurons[j].bias + parent2.layers[i].neurons[j].bias) / 2;
            }
        }
    }
    public void CombineWeights(ANN parent1, ANN parent2, float littleMutation)
    {
        for (int i = 0; i < parent1.layers.Count; i++)
        {
            for (int j = 0; j < parent1.layers[i].neurons.Count; j++)
            {
                for (int k = 0; k < parent1.layers[i].neurons[j].weights.Count; k++)
                {
                    layers[i].neurons[j].weights[k] = ((parent1.layers[i].neurons[j].weights[k] + parent2.layers[i].neurons[j].weights[k]) / 2) + Random.Range(-1f, 1f);
                }

                layers[i].neurons[j].bias = ((parent1.layers[i].neurons[j].bias + parent2.layers[i].neurons[j].bias) / 2) + Random.Range(-littleMutation, littleMutation);
            }
        }
    }
    public string PrintWeights()
    {
        string weightStr = "";
        foreach (Layer l in layers)
        {
            foreach (Neuron n in l.neurons)
            {
                foreach (double w in n.weights)
                {
                    weightStr += w + ",";
                }
                weightStr += n.bias + ",";
            }
        }
        return weightStr;
    }
    public void LoadWeights(string weightStr)
    {
        if (weightStr == "") return;
        string[] weightValues = weightStr.Split(',');
        int w = 0;
        foreach (Layer l in layers)
        {
            foreach (Neuron n in l.neurons)
            {
                for (int i = 0; i < n.weights.Count; i++)
                {
                    n.weights[i] = System.Convert.ToDouble(weightValues[w]);
                    w++;
                }
                n.bias = System.Convert.ToDouble(weightValues[w]);
                w++;
            }
        }
    }
    double Activation(double input)
    {
        switch(hiddenActivation)
        {
            case global::Activation.Sigmoid:
                return Sigmoid(input);
            case global::Activation.Tanh:
                return Tanh(input);
            case global::Activation.ReLu:
                return ReLu(input);
            case global::Activation.Step:
                return Step(input);
            default:
                return Sigmoid(input);
        }
    }
    double ActivationO(double input)
    {
        switch (hiddenActivation)
        {
            case global::Activation.Sigmoid:
                return Sigmoid(input);
            case global::Activation.Tanh:
                return Tanh(input);
            case global::Activation.ReLu:
                return ReLu(input);
            case global::Activation.Step:
                return Step(input);
            default:
                return Sigmoid(input);
        }
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

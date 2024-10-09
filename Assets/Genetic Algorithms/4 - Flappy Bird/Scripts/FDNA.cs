using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FDNA
{
    [SerializeField] List<float> genes = new List<float>();
    int dnaLength = 0;
    float maxValues = 0;

    public FDNA(int l, float v)
    {
        dnaLength = l;
        maxValues = v;

        SetRandom();
    }

    public void SetRandom()
    {
        genes.Clear();

        for (int i = 0; i < dnaLength; i++)
        {
            genes.Add(UnityEngine.Random.Range(-maxValues, maxValues));
        }
    }

    public void SetInt(int pos, int value)
    {
        genes[pos] = value;
    }

    public void Combine(FDNA parent1, FDNA parent2)
    {
        for (int i = 0; i < dnaLength; i++)
        {
            genes[i] = UnityEngine.Random.Range(0, 10) < 5 ? parent1.genes[i] : parent2.genes[i];
            float mutation = genes[i] * 0.05f;
            genes[i] += UnityEngine.Random.Range(-mutation, mutation);
        }
    }

    public void Mutate()
    {
        genes[UnityEngine.Random.Range(0, dnaLength)] = UnityEngine.Random.Range(-maxValues, maxValues);
    }

    public float GetGene(int pos)
    {
        return genes[pos];
    }
}

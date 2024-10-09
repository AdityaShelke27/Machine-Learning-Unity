using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GDNA
{
    [SerializeField] List<int> genes = new List<int>();
    int dnaLength = 0;
    int maxValues = 0;

    public GDNA(int l, int v)
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
            genes.Add(UnityEngine.Random.Range(0, maxValues));
        }
    }

    public void SetInt(int pos, int value)
    {
        genes[pos] = value;
    }

    public void Combine(GDNA parent1, GDNA parent2)
    {
        for (int i = 0; i < dnaLength; i++)
        {
            if (i < dnaLength / 2)
            {
                genes[i] = parent1.genes[i];
            }
            else
            {
                genes[i] = parent2.genes[i];
            }
        }
    }

    public void Mutate()
    {
        genes[UnityEngine.Random.Range(0, dnaLength)] = UnityEngine.Random.Range(0, maxValues);
    }

    public int GetGene(int pos)
    {
        return genes[pos];
    }
}

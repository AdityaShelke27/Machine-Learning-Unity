using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CarPopulationManager : MonoBehaviour
{
    [SerializeField] GameObject carPrefab;
    [SerializeField] int generation = 0;
    [SerializeField] int populationSize;
    [SerializeField] List<GameObject> population = new();
    [SerializeField] List<GameObject> sortedList = new();
    [SerializeField] Transform startPoint;
    [SerializeField] float maxTime;
    float fitness;
    [SerializeField] Image[] neuronInput;
    [SerializeField] Image[] neuronHidden;
    [SerializeField] Image[] neuronOutput;
    [SerializeField] LineRenderer[] neuronLines;
    float elapsed = 0;
    public static int alivePopulationCount;
    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 300, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 300, 30), string.Format("Best Fitness score: {0:0.00}", fitness), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }

    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject b = Instantiate(carPrefab, startPoint.position, startPoint.rotation);
            b.GetComponent<CarBrain>().Init();
            population.Add(b);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 2;
        alivePopulationCount = populationSize;
        elapsed = maxTime;
        int linenum = 0;
        for(int i = 0; i < neuronInput.Length; i++)
        {
            for (int j = 0; j < neuronHidden.Length; j++)
            {
                neuronLines[linenum].SetPosition(0, neuronInput[i].transform.position);
                neuronLines[linenum].SetPosition(1, neuronHidden[j].transform.position);

                linenum++;
            }
        }
        for (int i = 0; i < neuronOutput.Length; i++)
        {
            for (int j = 0; j < neuronHidden.Length; j++)
            {
                neuronLines[linenum].SetPosition(0, neuronOutput[i].transform.position);
                neuronLines[linenum].SetPosition(1, neuronHidden[j].transform.position);

                linenum++;
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) //alivePopulationCount <= 0 || elapsed <= 0)
        {
            BreedNewPopulation();
        }
        if(Input.GetKey(KeyCode.Alpha2))
        {
            Time.timeScale = 4;
        }
        else
        {
            Time.timeScale = 2;
        }
        NeuronPulse();
        elapsed -= Time.deltaTime;
    }

    GameObject Breed(GameObject parent1)
    {
        GameObject offspring = Instantiate(carPrefab, startPoint.position, startPoint.rotation);
        CarBrain b = offspring.GetComponent<CarBrain>();
        b.Init();
        //if (Random.Range(0, 100) != 1)
        //{
            b.Combine(parent1.GetComponent<CarBrain>());
        //}
        
        return offspring;
    }

    void BreedNewPopulation()
    {
        sortedList = population.OrderByDescending(o => o.GetComponent<CarBrain>().fitness).ToList();
        population.Clear();
        
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                population.Add(Breed(sortedList[i]));
            }
        }

        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        generation++;
        alivePopulationCount = populationSize;
        elapsed = maxTime;
    }
    void NeuronPulse()
    {
        GameObject obj = population.OrderByDescending(o => o.GetComponent<CarBrain>().fitness).First();
        CarBrain cb = obj.GetComponent<CarBrain>();
        if (cb.fitness > fitness)
        {
            fitness = cb.fitness;
        }
        int lineNum = 0;
        for(int i = 0; i < cb.input.Count; i++)
        {
            if (1 - cb.input[i] >= 0)
                neuronInput[i].color = Color.green;
            else
                neuronInput[i].color = Color.red;
        }
        for (int j = 0; j < cb.ann.layers[0].neurons.Count; j++)
        {
            for(int k = 0; k < cb.ann.layers[0].neurons[j].inputs.Count; k++)
            {
                if (cb.ann.layers[0].neurons[j].inputs[k] > 0.5)
                {
                    neuronLines[lineNum].startColor = Color.green;
                    neuronLines[lineNum].endColor = Color.green;
                }
                else
                {
                    neuronLines[lineNum].startColor = Color.red;
                    neuronLines[lineNum].endColor = Color.red;
                }
                lineNum++;
            }
            if (cb.ann.layers[0].neurons[j].output > 0.5f)
                neuronHidden[j].color = Color.green;
            else
                neuronHidden[j].color = Color.red;
        }
        for(int j = 0; j < cb.ann.layers[1].neurons.Count; j++)
        {
            for (int k = 0; k < cb.ann.layers[1].neurons[j].inputs.Count; k++)
            {
                if (cb.ann.layers[1].neurons[j].inputs[k] > 0.5)
                {
                    neuronLines[lineNum].startColor = Color.green;
                    neuronLines[lineNum].endColor = Color.green;
                }
                else
                {
                    neuronLines[lineNum].startColor = Color.red;
                    neuronLines[lineNum].endColor = Color.red;
                }
                lineNum++;
            }
        }
        
        if (cb.ann.layers[1].neurons[0].output > cb.ann.layers[1].neurons[1].output)
        {
            neuronOutput[0].color = Color.green;
            neuronOutput[1].color = Color.red;
        }
        else
        {
            neuronOutput[0].color = Color.red;
            neuronOutput[1].color = Color.green;
        }
        if (cb.ann.layers[1].neurons[2].output > cb.ann.layers[1].neurons[3].output)
        {
            neuronOutput[2].color = Color.green;
            neuronOutput[3].color = Color.red;
        }
        else
        {
            neuronOutput[2].color = Color.red;
            neuronOutput[3].color = Color.green;
        }
    }
}

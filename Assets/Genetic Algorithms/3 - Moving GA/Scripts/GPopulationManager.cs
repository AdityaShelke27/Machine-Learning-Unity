using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public int populationSize = 50;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public float trialTime = 5;
    int generation = 1;

    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject b = Instantiate(botPrefab, transform.position, Quaternion.identity);
            b.GetComponent<GBrain>().Init();
            population.Add(b);
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > trialTime)
        {
            BreedNewPopulation();
            elapsed = 0;
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        GameObject offspring = Instantiate(botPrefab, transform.position, Quaternion.identity);
        GBrain b = offspring.GetComponent<GBrain>();
        b.Init();
        if (Random.Range(0, 100) == 1)
        {
            b.dna.Mutate();
        }
        else
        {
            b.dna.Combine(parent1.GetComponent<GBrain>().dna, parent2.GetComponent<GBrain>().dna);
        }

        return offspring;
    }

    void BreedNewPopulation()
    {
        List<GameObject> sortedList = population.OrderBy(o => (o.GetComponent<GBrain>().distanceTravelled)).ToList();
        population.Clear();
        for (int i = (sortedList.Count / 2) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        generation++;
    }
}

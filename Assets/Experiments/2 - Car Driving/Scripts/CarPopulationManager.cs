using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarPopulationManager : MonoBehaviour
{
    [SerializeField] GameObject carPrefab;
    [SerializeField] int generation = 0;
    [SerializeField] int populationSize;
    [SerializeField] List<GameObject> population = new();
    [SerializeField] Transform startPoint;
    [SerializeField] float maxTime;
    float elapsed = 0;
    public static int alivePopulationCount;
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

    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject b = Instantiate(carPrefab, startPoint.position, startPoint.rotation);
            b.GetComponent<CarBrain>().Init();
            population.Add(b);
        }

        alivePopulationCount = populationSize;
        elapsed = maxTime;
    }

    void Update()
    {
        if (alivePopulationCount <= 0 || elapsed <= 0)
        {
            BreedNewPopulation();
        }
        elapsed -= Time.deltaTime;
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        GameObject offspring = Instantiate(carPrefab, startPoint.position, startPoint.rotation);
        CarBrain b = offspring.GetComponent<CarBrain>();
        b.Init();
        if (Random.Range(0, 100) != 1)
        {
            b.Combine(parent1.GetComponent<CarBrain>(), parent2.GetComponent<CarBrain>());
        }
        return offspring;
    }

    void BreedNewPopulation()
    {
        List<GameObject> sortedList = population.OrderByDescending(o => (o.GetComponent<CarBrain>().distanceTravelled / o.GetComponent<CarBrain>().timeAlive) + o.GetComponent<CarBrain>().distanceTravelled).ToList();
        population.Clear();
        for (int i = 0; i < (sortedList.Count / 4); i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        generation++;
        alivePopulationCount = populationSize;
        elapsed = maxTime;
    }
}

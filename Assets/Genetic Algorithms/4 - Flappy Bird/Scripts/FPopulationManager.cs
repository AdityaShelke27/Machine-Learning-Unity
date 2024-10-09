using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FPopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public Transform spawnPoint;
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
            GameObject b = Instantiate(botPrefab, spawnPoint.position, Quaternion.identity);
            b.GetComponent<FBrain>().Init();
            population.Add(b);
        }

        Time.timeScale = 5;
        using (StreamWriter writer = new StreamWriter("C:\\Users\\Admin\\Desktop\\Aditya\\data.txt"))
        {
            writer.Write("");
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
        FBrain b = offspring.GetComponent<FBrain>();
        b.Init();
        if (Random.Range(0, 100) == 1)
        {
            b.dna.Mutate();
        }
        else
        {
            b.dna.Combine(parent1.GetComponent<FBrain>().dna, parent2.GetComponent<FBrain>().dna);
        }

        return offspring;
    }

    void BreedNewPopulation()
    {
        List<GameObject> sortedList = population.OrderBy(o => (o.GetComponent<FBrain>().distanceTravelled - o.GetComponent<FBrain>().crash)).ToList();
        using (FileStream fs = new FileStream("C:\\Users\\Admin\\Desktop\\Aditya\\data.txt", FileMode.Append, FileAccess.Write))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                float distance = sortedList[^1].GetComponent<FBrain>().distanceTravelled;
                float crash = sortedList[^1].GetComponent<FBrain>().crash;
                writer.WriteLine(distance + " " + crash + " " + (distance - crash));
            }
        }
           
        
        population.Clear();
        for (int i = (3 * sortedList.Count / 4) - 1; i < sortedList.Count - 1; i++)
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
    }
}

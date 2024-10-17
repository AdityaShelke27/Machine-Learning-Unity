using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BirdReplay
{
    public List<double> states;
    public double reward;

    public BirdReplay(double top, double bottom, double position, double r)
    {
        states = new List<double>
        {
            top,
            bottom,
            position
        };
        reward = r;
    }
}
public class BirdBrain : MonoBehaviour
{
    public GameObject bird;
    public Activation hiddenActivation;
    public Activation outputActivation;
    public LayerMask wallLayer;

    ANN ann;
    Rigidbody2D birdRB;
    BirdState bs;
    float reward = 0;
    List<BirdReplay> BirdReplayMemory = new();
    int mCapacity = 10000;

    float discount = 0.99f;
    float exploreRate = 100f;
    float maxExploreRate = 100f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.0001f;

    Vector3 birdStartpos;
    int failCount = 0;
    float jumpForce = 100f;

    float timer = 0;
    float maxBalanceTime = 0;
    GUIStyle guiStyle = new();
    void Start()
    {
        ann = new ANN(3, 2, 1, 6, 0.2, hiddenActivation, outputActivation);
        birdStartpos = bird.transform.position;

        birdRB = bird.GetComponent<Rigidbody2D>();
        bs = bird.GetComponent<BirdState>();

        Time.timeScale = 5;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ResetBird();
        }
    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new();
        List<double> qs = new();

        RaycastHit hit;
        Physics.Raycast(transform.position, transform.up, out hit, 1000, wallLayer);
        states.Add(hit.distance);
        Physics.Raycast(transform.position, -transform.up, out hit, 1000, wallLayer);
        states.Add(hit.distance);
        states.Add(transform.position.y);

        qs = SoftMax(ann.Test(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);
        /*
        if(Random.Range(0, 100) < exploreRate)
            maxQIndex = Random.Range(0, 2);*/

        if (maxQIndex == 0)
            birdRB.AddForce(transform.up * jumpForce * (float)qs[maxQIndex]);
        else
            birdRB.AddForce(transform.up * -jumpForce * (float)qs[maxQIndex]);

        if (bs.dropped)
            reward = -1;
        else
            reward = 0.1f;

        BirdReplay lastMemory = new(states[0], states[1], states[2], reward);

        if (BirdReplayMemory.Count > mCapacity)
            BirdReplayMemory.RemoveAt(0);

        BirdReplayMemory.Add(lastMemory);

        if (bs.dropped)
        {
            for (int i = BirdReplayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputsOld = new();
                List<double> toutputsnew = new();

                toutputsOld = SoftMax(ann.Test(BirdReplayMemory[i].states));

                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.IndexOf(maxQOld);

                double feedback;
                if (i == BirdReplayMemory.Count - 1 || BirdReplayMemory[i].reward == -1)
                    feedback = BirdReplayMemory[i].reward;
                else
                {
                    toutputsnew = SoftMax(ann.Test(BirdReplayMemory[i + 1].states));
                    maxQ = toutputsnew.Max();
                    feedback = BirdReplayMemory[i].reward + discount * maxQ;
                }

                toutputsOld[action] = feedback;
                ann.Train(BirdReplayMemory[i].states, toutputsOld);
            }

            if (timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;
            bs.dropped = false;
            ResetBird();
            BirdReplayMemory.Clear();
            failCount++;

        }
    }

    private void ResetBird()
    {
        bird.transform.position = birdStartpos;
    }

    List<double> SoftMax(List<double> values)
    {
        double max = values.Max();
        float scale = 0;

        for (int i = 0; i < values.Count; ++i)
        {
            scale += Mathf.Exp((float)(values[i] - max));
        }

        List<double> result = new();
        for (int i = 0; i < values.Count; ++i)
        {
            result.Add(Mathf.Exp((float)(values[i] - max)) / scale);
        }

        return result;
    }
    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "States", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Failes: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + exploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + timer, guiStyle);
        GUI.EndGroup();
    }
}

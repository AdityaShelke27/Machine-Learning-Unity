using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BalancerBrain : MonoBehaviour
{
    [SerializeField] GameObject rod;
    [SerializeField] Activation hiddenActivation;
    [SerializeField] Activation outputActivation;
    [SerializeField] float moveSpeed;
    Vector2 balancerInitialPos;
    ANN ann;
    Rigidbody2D rodRb;
    float reward = 0;
    bool reset = false;
    [SerializeField] List<Replay> replayMemory = new();
    int mCapacity = 10000;

    float discount = 0.99f;
    float exploreRate = 100f;
    float maxExploreRate = 100f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.01f;
    
    int failCount = 0;

    float timer = 0;
    float maxBalanceTime = 0;
    GUIStyle guiStyle = new();
    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(2, 2, 2, 6, 0.1, hiddenActivation, outputActivation);
        rodRb = rod.GetComponent<Rigidbody2D>();
        balancerInitialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        List<double> states = new();
        List<double> qs = new();

        states.Add(rodRb.angularVelocity);
        states.Add(rod.transform.rotation.z);

        qs = SoftMax(ann.Test(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);
        
        if(UnityEngine.Random.Range(0, 100) < exploreRate)
            maxQIndex = UnityEngine.Random.Range(0, 2);

        if (maxQIndex == 0)
            transform.Translate(Vector3.right * moveSpeed * (float)qs[maxQIndex] * Time.deltaTime);
        else
            transform.Translate(Vector3.right * moveSpeed * (float)-qs[maxQIndex] * Time.deltaTime);

        if (reset)
            reward = -1f;
        else
            reward = 0.1f;

        Replay lastMemory = new(states, reward);

        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);
        //Debug.Log(qs[0]);
        if (reset)
        {
            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputsOld = new();
                List<double> toutputsnew = new();

                toutputsOld =  ann.Test(replayMemory[i].states);

                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.IndexOf(maxQOld);

                double feedback;
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    feedback = replayMemory[i].reward;
                else
                {
                    toutputsnew = ann.Test(replayMemory[i + 1].states);
                    qs[0] = toutputsnew.Max();
                    feedback = replayMemory[i].reward + discount * qs[0];
                }
                toutputsOld[action] = feedback;
                ann.Train(replayMemory[i].states, toutputsOld);
                //Debug.Log(replayMemory[i].reward + " " + discount + " " + qs[0] + " " + feedback);
            }

            if (timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;
            reset = false;
            transform.rotation = Quaternion.identity;
            ResetAll();
            replayMemory.Clear();
            failCount++;
        }
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

    private void ResetAll()
    {
        transform.position = balancerInitialPos;
        rod.transform.position = Vector2.zero;
        rodRb.velocity = Vector2.zero;
        rodRb.angularVelocity = 0;
        reset = false;
        rod.transform.rotation = Quaternion.identity;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.transform.root.tag);
        if(collision.transform.root.tag == "Rod")
        {
            reset = true;
        }
    }
}

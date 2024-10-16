using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Replay
{
    public List<double> states;
    public double reward;

    public Replay(double xr, double ballz, double ballvz, double r)
    {
        states = new List<double>
        {
            xr,
            ballz,
            ballvz
        };
        reward = r;
    }
}
public class PlatformBrain : MonoBehaviour
{
    public GameObject ball;
    public Activation hiddenActivation;
    public Activation outputActivation;

    ANN ann;
    Rigidbody ballRB;
    BallState bs;
    float reward = 0;
    List<Replay> replayMemory = new();
    int mCapacity = 10000;

    float discount = 0.99f;
    float exploreRate = 100f;
    float maxExploreRate = 100f;
    float minExploreRate = 0.01f;
    float exploreDecay = 0.0001f;

    Vector3 ballStartpos;
    int failCount = 0;
    float tiltSpeed = 0.5f;

    float timer = 0;
    float maxBalanceTime = 0;
    GUIStyle guiStyle = new GUIStyle();
    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(3, 2, 1, 6, 0.2, hiddenActivation, outputActivation);
        ballStartpos = ball.transform.position;

        ballRB = ball.GetComponent<Rigidbody>();
        bs = ball.GetComponent<BallState>();

        Time.timeScale = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            ResetBall();
        }    
    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        List<double> states = new();
        List<double> qs = new();

        states.Add(transform.rotation.x);
        states.Add(ball.transform.rotation.z);
        states.Add(ballRB.angularVelocity.x);

        qs = SoftMax(ann.Test(states));
        double maxQ = qs.Max();
        int maxQIndex = qs.IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        /*if(Random.Range(0, 100) < exploreRate)
            maxQIndex = Random.Range(0, 2);*/

        if (maxQIndex == 0)
            transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        else
            transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);

        if (bs.dropped)
            reward = -1;
        else
            reward = 0.1f;

        Replay lastMemory = new Replay(states[0], states[1], states[2], reward);

        if(replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        replayMemory.Add(lastMemory);

        if(bs.dropped)
        {
            for(int i = replayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputsOld = new();
                List<double> toutputsnew = new();

                toutputsOld = SoftMax(ann.Test(replayMemory[i].states));
                
                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.IndexOf(maxQOld);

                double feedback;
                if(i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    feedback = replayMemory[i].reward;
                else
                {
                    toutputsnew = SoftMax(ann.Test(replayMemory[i + 1].states));
                    maxQ = toutputsnew.Max();
                    feedback = replayMemory[i].reward + discount * maxQ;
                }
                
                toutputsOld[action] = feedback;
                ann.Train(replayMemory[i].states, toutputsOld);
            }

            if (timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;
            bs.dropped = false;
            transform.rotation = Quaternion.identity;
            ResetBall();
            replayMemory.Clear();
            failCount++;

        }
    }

    private void ResetBall()
    {
        ball.transform.position = ballStartpos;
        ballRB.velocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;
    }

    List<double> SoftMax(List<double> values)
    {
        double max = values.Max();
        float scale = 0;

        for(int i = 0; i < values.Count; ++i)
        {
            scale += Mathf.Exp((float) (values[i] - max));
        }

        List<double> result = new();
        for(int i = 0; i < values.Count; ++i)
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

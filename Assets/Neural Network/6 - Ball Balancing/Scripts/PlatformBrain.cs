using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

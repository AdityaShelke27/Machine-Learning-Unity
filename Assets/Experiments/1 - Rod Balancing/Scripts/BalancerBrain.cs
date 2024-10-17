using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancerBrain : MonoBehaviour
{
    [SerializeField] GameObject rod;
    [SerializeField] Activation hiddenActivation;
    [SerializeField] Activation outputActivation;
    ANN ann;
    Rigidbody2D rodRb;
    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(2, 1, 1, 3, 0.1, hiddenActivation, outputActivation);
        rodRb = rod.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

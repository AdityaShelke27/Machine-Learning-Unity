using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleBrain : MonoBehaviour
{
    public GameObject ball;
    public GameObject paddle;
    Rigidbody2D brb;
    public LayerMask backWall;
    [SerializeField] float yvel;
    [SerializeField] Activation hiddenActivation;
    [SerializeField] Activation outputActivation;
    float paddleMinY = 8.8f;
    float paddleMaxY = 17.4f;
    float paddleMaxSpeed = 50f;
    public float numSaved = 0;
    public float numMissed = 0;

    ANN ann;
    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(6, 1, 1, 4, 0.01, hiddenActivation, outputActivation);
        brb = ball.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float posy = Mathf.Clamp(paddle.transform.position.y + yvel * Time.deltaTime * paddleMaxSpeed, paddleMinY, paddleMaxY);
        paddle.transform.position = new Vector3(paddle.transform.position.x, posy, 0);

        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, brb.velocity, 1000, backWall);
        Debug.DrawRay(ball.transform.position, brb.velocity);

        List<double> output;
        if(hit)
        {
            if(hit.collider.tag == "tops")
            {
                Vector3 reflection = Vector2.Reflect(brb.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, backWall);
                Debug.DrawRay(hit.point, reflection);
            }
            if (hit && hit.collider.tag == "backwall")
            {
                float dy = hit.point.y - paddle.transform.position.y;

                output = Run(ball.transform.position, brb.velocity, paddle.transform.position, dy, true);

                yvel = (float)output[0];
            }
        }
        else
        {
            yvel = 0;
        }
    }

    List<double> Run(Vector2 ballPos, Vector2 ballVel, Vector2 paddlePos, float pv, bool train)
    {
        List<double> inputs = new() { ballPos.x, ballPos.y, ballVel.x, ballVel.y, paddlePos.x, paddlePos.y };
        List<double> outputs = new() { pv };

        if(train)
        {
            return ann.Train(inputs, outputs);
        }
        else
        {
            return ann.Test(inputs);
        }

    }
}

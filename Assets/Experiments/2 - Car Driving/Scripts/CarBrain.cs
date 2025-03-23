using System.Collections.Generic;
using UnityEngine;

public class CarBrain : MonoBehaviour
{
    public ANN ann;
    [SerializeField] Activation hiddenActivation;
    [SerializeField] Activation outputActivation;
    [SerializeField] LayerMask blockerLayer;
    [SerializeField] Transform sensorPos;
    [SerializeField] LineRenderer[] lineRenderers;
    public bool isDead = false;
    public float distanceTravelled;
    public float timeAlive;
    float limitViewDistance;
    Rigidbody rb;
    [SerializeField] float acceleration;
    [SerializeField] float breaking;
    [SerializeField] float maxViewDistance;
    [SerializeField] float turnRate;
    Vector3 initialPos;
    [Header("Stats")]
    [SerializeField] float accelerationValue;
    [SerializeField] float breakingValue;
    [SerializeField] float turnRateValue;
    public float fitness;
    public List<double> input;
    // Start is called before the first frame update
    void Start()
    {
        //Init();
    }
    public void Init()
    {
        ann = new ANN(7, 4, 1, 5, 0.2f, hiddenActivation, outputActivation);
        rb = GetComponent<Rigidbody>();

        initialPos = transform.position;
        limitViewDistance = maxViewDistance * 0.5f;
    }
    private void Update()
    {
        if (!isDead)
        {
            timeAlive += Time.deltaTime;
            fitness = 0.3f * (distanceTravelled / timeAlive) + distanceTravelled;
        }
        if (!isDead && Mathf.Abs(transform.position.y) > 2)
        {
            CarPopulationManager.alivePopulationCount--;
            isDead = true;
            rb.velocity = Vector3.zero;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            distanceTravelled += rb.velocity.magnitude * Time.fixedDeltaTime;

            RaycastHit hitF, hitR, hitL, hitR45, hitL45;
            input = new() { maxViewDistance, maxViewDistance, maxViewDistance, maxViewDistance, maxViewDistance, 0, 0};

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].SetPosition(0, sensorPos.position);
            }

            if (Physics.Raycast(sensorPos.position, transform.forward, out hitF, maxViewDistance, blockerLayer))
            {
                input[0] = Round(hitF.distance / maxViewDistance);
                //Debug.DrawRay(sensorPos.position, hitF.point - transform.position, Color.red);
                lineRenderers[0].SetPosition(1, hitF.point);
                if(Vector3.Distance(lineRenderers[0].GetPosition(0), lineRenderers[0].GetPosition(1)) < limitViewDistance)
                {
                    lineRenderers[0].startColor = Color.red;
                    lineRenderers[0].endColor = Color.red;
                }
                else
                {
                    lineRenderers[0].startColor = Color.green;
                    lineRenderers[0].endColor = Color.green;
                }
            }
            else
            {
                lineRenderers[0].SetPosition(1, transform.position + transform.forward * maxViewDistance);
                lineRenderers[0].startColor = Color.green;
                lineRenderers[0].endColor = Color.green;
            }
            if (Physics.Raycast(sensorPos.position, transform.right, out hitR, maxViewDistance, blockerLayer))
            {
                input[1] = Round(hitR.distance / maxViewDistance);
                //Debug.DrawRay(sensorPos.position, hitR.point - transform.position, Color.blue);
                lineRenderers[1].SetPosition(1, hitR.point);
                if (Vector3.Distance(lineRenderers[1].GetPosition(0), lineRenderers[1].GetPosition(1)) < limitViewDistance)
                {
                    lineRenderers[1].startColor = Color.red;
                    lineRenderers[1].endColor = Color.red;
                }
                else
                {
                    lineRenderers[1].startColor = Color.green;
                    lineRenderers[1].endColor = Color.green;
                }
            }
            else
            {
                lineRenderers[1].SetPosition(1, transform.position + transform.right * maxViewDistance);
                lineRenderers[1].startColor = Color.green;
                lineRenderers[1].endColor = Color.green;
            }
            if (Physics.Raycast(sensorPos.position, -transform.right, out hitL, maxViewDistance, blockerLayer))
            {
                input[2] = Round(hitL.distance / maxViewDistance);
                //Debug.DrawRay(sensorPos.position, hitL.point - transform.position, Color.green);
                lineRenderers[2].SetPosition(1, hitL.point);
                if (Vector3.Distance(lineRenderers[2].GetPosition(0), lineRenderers[2].GetPosition(1)) < limitViewDistance)
                {
                    lineRenderers[2].startColor = Color.red;
                    lineRenderers[2].endColor = Color.red;
                }
                else
                {
                    lineRenderers[2].startColor = Color.green;
                    lineRenderers[2].endColor = Color.green;
                }
            }
            else
            {
                lineRenderers[2].SetPosition(1, transform.position - transform.right * maxViewDistance);
                lineRenderers[2].startColor = Color.green;
                lineRenderers[2].endColor = Color.green;
            }
            Vector3 angle = Quaternion.AngleAxis(45, Vector3.up) * transform.forward;
            if (Physics.Raycast(sensorPos.position, angle, out hitL45, maxViewDistance, blockerLayer))
            {
                input[3] = Round(hitL45.distance / maxViewDistance);
                //Debug.DrawRay(sensorPos.position, hitL45.point - transform.position, Color.yellow);
                lineRenderers[3].SetPosition(1, hitL45.point);
                if (Vector3.Distance(lineRenderers[3].GetPosition(0), lineRenderers[3].GetPosition(1)) < limitViewDistance)
                {
                    lineRenderers[3].startColor = Color.red;
                    lineRenderers[3].endColor = Color.red;
                }
                else
                {
                    lineRenderers[3].startColor = Color.green;
                    lineRenderers[3].endColor = Color.green;
                }
            }
            else
            {
                lineRenderers[3].SetPosition(1, transform.position + angle * maxViewDistance);
                lineRenderers[3].startColor = Color.green;
                lineRenderers[3].endColor = Color.green;
            }
            angle = Quaternion.AngleAxis(-45, Vector3.up) * transform.forward;
            if (Physics.Raycast(sensorPos.position, angle, out hitR45, maxViewDistance, blockerLayer))
            {
                input[4] = Round(hitR45.distance / maxViewDistance);
                //Debug.DrawRay(sensorPos.position, hitR45.point - transform.position, Color.black);
                lineRenderers[4].SetPosition(1, hitR45.point);
                if (Vector3.Distance(lineRenderers[4].GetPosition(0), lineRenderers[4].GetPosition(1)) < limitViewDistance)
                {
                    lineRenderers[4].startColor = Color.red;
                    lineRenderers[4].endColor = Color.red;
                }
                else
                {
                    lineRenderers[4].startColor = Color.green;
                    lineRenderers[4].endColor = Color.green;
                }
            }
            else
            {
                lineRenderers[4].SetPosition(1, transform.position + angle * maxViewDistance);
                lineRenderers[4].startColor = Color.green;
                lineRenderers[4].endColor = Color.green;
            }

            input[5] = (double) rb.velocity.magnitude;
            input[6] = (double) rb.angularVelocity.magnitude;
            List<double> output = ann.Test(input);
            //Debug.Log(output[0] + " " + output[1] + " " + output[2] + " " + output[3]);
            if (output[0] > output[1])
            {
                accelerationValue = (float)output[0];
                rb.velocity = accelerationValue * acceleration * new Vector3(transform.forward.x, rb.velocity.y, transform.forward.z);
            }
            else
            {
                breakingValue = (float)output[1];
                rb.AddForce(breakingValue * -rb.velocity / breaking);
            }

            Vector3 turnDir;
            if (output[2] > output[3])
            {
                turnRateValue = (float)output[2];
                turnDir = turnRateValue * turnRate * transform.up * Time.fixedDeltaTime;
            }
            else
            {
                turnRateValue = (float)output[3];
                turnDir = turnRateValue * -turnRate * transform.up * Time.fixedDeltaTime;
            }
            //rb.velocity = (transform.forward + transform.up) * maxVelocity;
            //transform.Rotate(turnDir);
            rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles + turnDir);
        }
        /*Vector3 turnDir;
        rb.velocity = Input.GetAxis("Vertical") * acceleration * new Vector3(transform.forward.x, rb.velocity.y, transform.forward.z);
        turnDir = transform.up * Input.GetAxis("Horizontal") * turnRate;
        transform.Rotate(turnDir);*/
    }
    public void Combine(CarBrain parent1)
    {
        ann.CopyWeights(parent1.ann, 0.3f);
    }
    float Round(float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Blocker")
        {
            if (!isDead)
            {
                CarPopulationManager.alivePopulationCount--;
            }
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].enabled = false;
            }
            isDead = true;
            rb.velocity = Vector3.zero;
        }
    }
}

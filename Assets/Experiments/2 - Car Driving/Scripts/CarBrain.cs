using System.Collections.Generic;
using UnityEngine;

public class CarBrain : MonoBehaviour
{
    public ANN ann;
    [SerializeField] Activation hiddenActivation;
    [SerializeField] Activation outputActivation;
    [SerializeField] LayerMask blockerLayer;
    [SerializeField] Transform sensorPos;
    public bool isDead = false;
    public float distanceTravelled;
    public float timeAlive;
    Rigidbody rb;
    [SerializeField] float acceleration;
    [SerializeField] float breaking;
    [SerializeField] float maxViewDistance;
    [SerializeField] float turnRate;
    Vector3 initialPos;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public void Init()
    {
        ann = new ANN(7, 4, 1, 6, 0.2f, hiddenActivation, outputActivation);
        rb = GetComponent<Rigidbody>();

        initialPos = transform.position;
        
    }
    private void Update()
    {
        if (!isDead)
        {
            timeAlive += Time.deltaTime;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            distanceTravelled += rb.velocity.magnitude * Time.fixedDeltaTime;

            RaycastHit hitF, hitR, hitL, hitR45, hitL45;
            List<double> input = new() { maxViewDistance, maxViewDistance, maxViewDistance, maxViewDistance, maxViewDistance, 0, 0};
            
            if (Physics.Raycast(sensorPos.position, transform.forward, out hitF, maxViewDistance, blockerLayer))
            {
                input[0] = Round(hitF.distance / maxViewDistance);
                Debug.DrawRay(sensorPos.position, hitF.point - transform.position, Color.red);
            }
            if (Physics.Raycast(sensorPos.position, transform.right, out hitR, maxViewDistance, blockerLayer))
            {
                input[1] = Round(hitR.distance / maxViewDistance);
                Debug.DrawRay(sensorPos.position, hitR.point - transform.position, Color.blue);
            }
            if (Physics.Raycast(sensorPos.position, -transform.right, out hitL, maxViewDistance, blockerLayer))
            {
                input[2] = Round(hitL.distance / maxViewDistance);
                Debug.DrawRay(sensorPos.position, hitL.point - transform.position, Color.green);
            }
            if (Physics.Raycast(sensorPos.position, Quaternion.AngleAxis(45, Vector3.up) * transform.forward, out hitL45, maxViewDistance, blockerLayer))
            {
                input[3] = Round(hitL45.distance / maxViewDistance);
                Debug.DrawRay(sensorPos.position, hitL45.point - transform.position, Color.yellow);
            }
            if (Physics.Raycast(sensorPos.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.forward, out hitR45, maxViewDistance, blockerLayer))
            {
                input[4] = Round(hitR45.distance / maxViewDistance);
                Debug.DrawRay(sensorPos.position, hitR45.point - transform.position, Color.black);
            }

            input[5] = (double) rb.velocity.magnitude;
            input[6] = (double) rb.angularVelocity.magnitude;
            List<double> output = ann.Test(input);
            //Debug.Log(output[0] + " " + output[1] + " " + output[2] + " " + output[3]);
            if (output[0] > output[1])
            {
                rb.velocity = (float)output[0] * acceleration * new Vector3(transform.forward.x, rb.velocity.y, transform.forward.z);
            }
            else
            {
                rb.AddForce((float)output[1] * -rb.velocity / breaking);
            }

            Vector3 turnDir;
            if (output[2] > output[3])
            {
                turnDir = (float)output[2] * turnRate * transform.up * Time.fixedDeltaTime;
            }
            else
            {
                turnDir = (float)output[3] * -turnRate * transform.up * Time.fixedDeltaTime;
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
    public void Combine(CarBrain parent1, CarBrain parent2)
    {
        ann.CopyWeights(parent1.ann, 0.05f);
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
            isDead = true;
            rb.velocity = Vector3.zero;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBrain : MonoBehaviour
{
    public ANN ann;
    [SerializeField] Activation hiddenActivation;
    [SerializeField] Activation outputActivation;
    [SerializeField] LayerMask blockerLayer;
    public bool isDead = false;
    public float distanceTravelled;
    public float timeAlive;
    Rigidbody rb;
    [SerializeField] float maxVelocity;
    [SerializeField] float maxViewDistance;
    [SerializeField] float turnRate;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init()
    {
        ann = new ANN(5, 2, 1, 6, 0.2f, hiddenActivation, outputActivation);
        rb = GetComponent<Rigidbody>();

        rb.velocity = transform.forward * maxVelocity;
    }
    private void Update()
    {
        if(!isDead)
        {
            timeAlive += Time.deltaTime;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isDead)
        {
            distanceTravelled = maxVelocity * Time.deltaTime;

            RaycastHit hitF, hitR, hitL, hitR45, hitL45;
            List<double> input = new() { maxViewDistance, maxViewDistance, maxViewDistance, maxViewDistance, maxViewDistance };

            Debug.DrawRay(transform.position, transform.forward * maxViewDistance);
            Debug.DrawRay(transform.position, transform.right * maxViewDistance);
            Debug.DrawRay(transform.position, -transform.right * maxViewDistance);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * transform.forward * maxViewDistance);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.forward * maxViewDistance);

            if (Physics.Raycast(transform.position, transform.forward, out hitF, maxViewDistance, blockerLayer))
            {
                input[0] = hitF.distance;
            }
            if (Physics.Raycast(transform.position, transform.right, out hitR, maxViewDistance, blockerLayer))
            {
                input[1] = hitR.distance;
            }
            if (Physics.Raycast(transform.position, -transform.right, out hitL, maxViewDistance, blockerLayer))
            {
                input[2] = hitL.distance;
            }
            if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, transform.forward) * transform.forward, out hitL45, maxViewDistance, blockerLayer))
            {
                input[3] = hitL45.distance;
            }
            if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, transform.forward) * transform.forward, out hitR45, maxViewDistance, blockerLayer))
            {
                input[4] = hitR45.distance;
            }

            List<double> output = ann.Test(input);
            //rb.velocity = (float)output[0] * maxVelocity * new Vector3(transform.forward.x, rb.velocity.y, transform.forward.z);
            Vector3 turnDir;
            if (output[0] > output[1])
            {
                turnDir = transform.up * (float)output[0] * turnRate * Time.deltaTime;
            }
            else
            {
                turnDir = transform.up * (float)output[0] * -turnRate * Time.deltaTime;
            }
            rb.velocity = (transform.forward + transform.up) * maxVelocity;
            
            transform.Rotate(turnDir);
        }
    }
    public void Combine(CarBrain parent1, CarBrain parent2)
    {
        ann.CombineWeights(parent1.ann, parent2.ann, 0.3f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Blocker")
        {
            if(!isDead)
            {
                CarPopulationManager.alivePopulationCount--;
            }
            isDead = true;
            rb.velocity = Vector3.zero;
        }
    }
}

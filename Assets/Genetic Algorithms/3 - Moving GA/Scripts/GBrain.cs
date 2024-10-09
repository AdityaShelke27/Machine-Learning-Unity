using UnityEngine;

public class GBrain : MonoBehaviour
{
    int DNALength = 2;
    //public float timeAlive;
    //public float timeWalking;
    public GDNA dna;
    public GameObject eyes;
    public float distanceTravelled;
    bool alive;
    [SerializeField] bool seeWall;
    Vector3 initialPos;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!alive)
        {
            return;
        }
        distanceTravelled = Vector3.Distance(transform.position, initialPos);
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 0.3f);
        seeWall = false;
        RaycastHit hit;
        if(Physics.Raycast(eyes.transform.position, eyes.transform.forward, out hit, 0.3f))
        {
            if (hit.collider.tag == "wall")
            {
                seeWall = true;
            }
        }
        //timeAlive = GPopulationManager.elapsed;

        float turn = 0;
        float move = 0;

        if (!seeWall)
        {
            move = dna.GetGene(0);
        }
        else
        {
            turn = dna.GetGene(1);
        }

        transform.Translate(Vector3.forward * move * Time.deltaTime * 0.05f);
        transform.Rotate(Vector3.up * turn * Time.deltaTime);
    }
    public void Init()
    {
        //Initialize DNA
        //0 forward
        //1 turn Angle
        dna = new GDNA(DNALength, 360);
        //timeAlive = 0;
        alive = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "dead")
        {
            alive = false;
        }
    }
}

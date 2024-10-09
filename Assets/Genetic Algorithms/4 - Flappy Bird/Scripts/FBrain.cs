using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBrain : MonoBehaviour
{
    public int DNALength = 5;
    public FDNA dna;
    public GameObject eyes;
    bool seeDownWall;
    bool seeUpWall;
    bool seeTop;
    bool seeBottom;
    public float timeAlive = 0;
    public float distanceTravelled = 0;
    public int crash = 0;
    bool isAlive = true;
    float startPosition = 0;
    Rigidbody2D rb;
    [SerializeField] LayerMask wallLayer;
    void Update()
    {
        if(!isAlive)
        {
            return;
        }

        seeDownWall = false;
        seeUpWall = false;
        seeTop = false;
        seeBottom = false;

        Collider2D hitCol = Physics2D.OverlapCircle(eyes.transform.position, 0.3f, wallLayer);

        if(hitCol)
        {
            if (hitCol.tag == "upwall")
            {
                seeUpWall = true;
            }
            else if (hitCol.tag == "downwall")
            {
                seeDownWall = true;
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.up, 1);
        if(hit.collider)
        {
            if (hit.collider.tag == "top")
            {
                seeTop = true;
            }
        }
        hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.up, 1);
        if (hit.collider)
        {
            if (hit.collider.tag == "bottom")
            {
                seeBottom = true;
            }
        }

        //Debug.DrawRay(eyes.transform.position, eyes.transform.forward);
        //Debug.DrawRay(eyes.transform.position, -eyes.transform.forward);
        Debug.DrawRay(eyes.transform.position, eyes.transform.up);
        Debug.DrawRay(eyes.transform.position, -eyes.transform.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(eyes.transform.position, 0.3f);
    }

    private void FixedUpdate()
    {
        float upforce = 0;
        if(seeUpWall)
        {
            upforce = dna.GetGene(0);
        }
        else if(seeDownWall)
        {
            upforce = dna.GetGene(1);
        }
        else if (seeTop)
        {
            upforce = dna.GetGene(2);
        }
        else if (seeBottom)
        {
            upforce = dna.GetGene(3);
        }
        else
        {
            upforce = dna.GetGene(4);
        }

        rb.velocity = new Vector2(1, rb.velocity.y + upforce * 0.05f);

        distanceTravelled = transform.position.x - startPosition;
    }

    public void Init()
    {
        // Initialize DNA
        // 0 forward
        // 1 upwall
        // 2 downwall
        // 3 normal upward

        dna = new FDNA(DNALength, 200f);
        transform.Translate(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
        startPosition = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string objTag = collision.gameObject.tag;

        if(objTag == "dead")
        {
            isAlive = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        string objTag = collision.gameObject.tag;

        switch (objTag)
        {
            case "upwall":
            case "downwall":
            case "top":
            case "bottom":
                crash++;
                break;
        }
    }
}

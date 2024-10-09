using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{
    public int DNALength = 1;
    public float distanceTravelled;
    public Vector3 initialPos;
    public MDNA dna;

    ThirdPersonCharacter m_Character;
    Vector3 m_Move;
    bool m_Jump;
    bool m_Alive = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float h = 0, v = 0;
        bool crouch = false;
        switch(dna.GetGene(0))
        {
            case 0:
                v = 1;
                break;
            case 1:
                v = -1;
                break;
            case 2:
                h = -1;
                break;
            case 3:
                h = 1;
                break;
            case 4:
                m_Jump = true;
                break;
            case 5:
                crouch = true;
                break;
        }
        m_Move = v * Vector3.forward + h * Vector3.right;
        m_Character.Move(m_Move, crouch, m_Jump);
        m_Jump = false;
        if(m_Alive)
        {
            distanceTravelled = Vector3.Distance(initialPos, transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "dead")
        {
            m_Alive = false;
        }
    }

    public void Init()
    {
        //Initialize DNA
        //0 forward
        //1 back
        //2 left
        //3 right
        //4 jump
        //5 crouch

        dna = new MDNA(DNALength, 6);
        m_Character = GetComponent<ThirdPersonCharacter>();
        distanceTravelled = 0;
        m_Alive = true;

        initialPos = transform.position;
    }
}

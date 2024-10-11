using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    Vector3 ballStartPosition;
    Rigidbody2D rb;
    [SerializeField] float speed = 300;
    public AudioSource blip;
    public AudioSource blop;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballStartPosition = transform.position;
        ResetBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ResetBall();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Backwall")
        {
            blop.Play();
        }
        else
        {
            blip.Play();
        }
    }

    public void ResetBall()
    {
        transform.position = ballStartPosition;
        rb.velocity = Vector3.zero;
        Vector2 dir = new Vector2(Random.Range(100, 300), Random.Range(-100, 100));
        rb.AddForce(dir.normalized * speed);
    }
}

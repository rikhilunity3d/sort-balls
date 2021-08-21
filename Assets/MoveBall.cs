using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(0f,-moveSpeed);
    }
}

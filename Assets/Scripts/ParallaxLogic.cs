using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLogic : MonoBehaviour
{
    [SerializeField] private float endCoordinate;
    [SerializeField] private float parallaxSpeed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, -parallaxSpeed);
    }

    void Update()
    {
        if (transform.position.y <= -endCoordinate)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
}

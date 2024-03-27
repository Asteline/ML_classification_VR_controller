using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class Shooter : MonoBehaviour
{
    public GameObject ball;
    public Transform shooter, thumbsU;
    public float speed = 10f;
    public float upwardForce = 2f;
    public int points, shots, missed, missedInARow, madeInARow = 0;
    public bool madeLastShot = true;
    public float cameraX = 0f;
    public float xDist = 1f;
    public TextMeshProUGUI hits, throws, misses;
    public Animator animator;

    public bool onlyTraining, onlyTesting = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddPoints()
    {
        animator.SetTrigger("correct");

        if (madeLastShot)
        {
            madeInARow++;
        }

        madeLastShot = true;
        missedInARow = 0;
        points++;
        throws.text = shots.ToString();
        hits.text = points.ToString();
    }

    public void RemovePoints()
    {
        animator.SetTrigger("wrong");

        if (!madeLastShot)
        { 
            missedInARow++;
        }
        madeInARow = 0;
        madeLastShot = false;
        //missed++;
        throws.text = shots.ToString();
        //.text = missed.ToString();
    }

    public void ShootBall()
    {
        if (ball != null && GameObject.FindObjectsOfType<Ball>().Length == 0)
        {
            // Instantiate the ball at the starting point
            GameObject newBall = Instantiate(ball, shooter.position, Quaternion.identity);

            // Calculate the direction towards the main camera on y & z
            cameraX = UnityEngine.Random.Range(-xDist, xDist);
            Vector3 xAndZtoCamera = new Vector3(cameraX, Camera.main.transform.position.y, Camera.main.transform.position.z);

            Vector3 directionToCamera = xAndZtoCamera - shooter.position;
            Vector3 upwardComponent = Vector3.up * upwardForce; // Adjust the upward force
            Vector3 combinedDirection = (directionToCamera + upwardComponent).normalized;

            // Set the initial velocity of the ball
            Rigidbody ballRb = newBall.GetComponent<Rigidbody>();
            ballRb.velocity = combinedDirection * speed;

            shots++;
        }
    }

}

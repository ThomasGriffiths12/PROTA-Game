﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{

    Vector3 startPos = new Vector3(-10.0f, 0.07f, -9.5f);
    Vector3 velocity = new Vector3(1.0f, 0.0f, 0.0f);
    Vector3 otherVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 otherPos = new Vector3();

    public Quaternion initialRotation;

    bool facingLeft, staring, paying = false;

    public float minimumTimeToStay, minimumTimeToPay, minimumToPay;
    
    Rigidbody rb;

    Animator anim;

	// Use this for initialization
	void Start ()
    {
        facingLeft = (Random.value > 0.5f);

        velocity.x *= (Random.Range(1.0f, 2.0f));

        startPos.z = Random.Range(-10.0f, -6.0f);

        if (facingLeft)
        {
            startPos.x *= -1.0f;
            velocity.x *= -1.0f;
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        }

        initialRotation = transform.localRotation;

        transform.localPosition = startPos;

        rb = GetComponent<Rigidbody>();

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update ()
    {
        rb.velocity = velocity;

        if (transform.localPosition.x * transform.localPosition.x > 110)
            Destroy(transform.gameObject);

        if (staring)
        {
            otherPos = GameObject.Find("Capsule").transform.position;
            otherPos.y = transform.position.y;
            transform.LookAt(otherPos);
        }

        if (staring && !paying)
        {
            StartCoroutine(PayHomeless());
        }
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("NPC"))
        {
            otherVelocity = collision.transform.GetComponent<Rigidbody>().velocity;
            if (!staring)
                HandleNPCCollision(collision);
        }
        else if (collision.transform.CompareTag("Player"))
        {
            HandlePlayerCollision();
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("NPC"))
        {
            velocity.z = 0.0f;
        }
        transform.localRotation = initialRotation;
        rb.angularVelocity = Vector3.zero;
    }

    private void HandleNPCCollision(Collision collision)
    {
        if (velocity.x == 0.0f)
        {
            anim.SetBool("staring", false);
            velocity.x = otherVelocity.x;
            initialRotation = collision.transform.GetComponent<NPC>().initialRotation;
            transform.rotation = initialRotation;
            if (velocity.x < 0.0f)
                facingLeft = true;
            else
                facingLeft = false;
        }
        else if (facingLeft)
        {
            if (otherVelocity.x >= 0.0f)
                velocity.z = -1.0f;
            else if (otherVelocity.sqrMagnitude > velocity.sqrMagnitude)
                velocity.x = otherVelocity.x;
        }
        else if (transform.localPosition.z < -6.0f)
        {
            if (otherVelocity.x <= 0.0f)
                velocity.z = 1.0f;
            else if (otherVelocity.sqrMagnitude > velocity.sqrMagnitude)
                velocity.x = otherVelocity.x;
        }
    }

    private void HandlePlayerCollision()
    {
        int reaction = Mathf.CeilToInt(Random.Range(0.01f, 2.99f));

        switch (reaction)
        {
            case (1):
                velocity.x *= -2.0f;
                transform.rotation = initialRotation;
                transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
                initialRotation = transform.rotation;
                if (facingLeft)
                    facingLeft = false;
                else
                    facingLeft = true;
                break;
            case (2):
                velocity.x *= 2.0f;
                break;
            default:
                anim.SetBool("staring", true);
                StartCoroutine(StareAtHomeless());
                break;
        }
    }

    IEnumerator StareAtHomeless()
    {
        velocity.x = 0.0f;
        staring = true;

        yield return new WaitForSeconds(Random.Range(minimumTimeToStay, minimumTimeToStay + 10));

        staring = false;
    }

    IEnumerator PayHomeless()
    {
        paying = true;

        yield return new WaitForSeconds(Random.Range(minimumTimeToPay, minimumTimeToPay + 3));

        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        //// CODE TO GIVE MONEY STORED IN minimumToPay TO HOMELESS MAN ////
        ///////////////////////////////////////////////////////////////////
        ///// LEAVE IF TO ENSURE NPC IS STILL WATCHING HOMELESS MAN! //////
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        if (staring)
        {
            GameObject.Find("Money Text").GetComponent<Text>().text = (int.Parse(GameObject.Find("MoneyCounter").GetComponent<Text>().text) + Mathf.FloorToInt(Random.Range(minimumToPay, minimumToPay + 5))).ToString();
        }

        paying = false;
    }
}

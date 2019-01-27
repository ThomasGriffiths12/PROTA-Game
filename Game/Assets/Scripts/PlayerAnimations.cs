﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            animator.SetTrigger("Dance");
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            animator.SetTrigger("Busk");
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            animator.SetTrigger("Drinking");
        }
    }
}

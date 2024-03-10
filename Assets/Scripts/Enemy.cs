using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] public Transform targetLock;

    GameObject player;
    Animator animator;

    [HideInInspector] public bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
    }

    public void TakeDamage(float weaponDamage)
    {
        health -= weaponDamage;
        animator.SetTrigger("damage");

        if (health <= 0)
        {
            Die();
        }
        Debug.Log("Enemy damage received");
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead");
        //Destroy(gameObject, 5f);
    }
}

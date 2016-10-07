﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class MonsterAI : MonoBehaviour
{
    public GameObject[] enemySpawnPoints;
    GameObject player;
    int enemySpawnRandom;
    public int roamRadius;
    public int attackRadius;
    public int deAggro;

    bool runningCoroutine;
    string gameOverScene, winScreen;

    public enum enemyBehavior
    {
        patrol,
        chase,
        attack,
        runAway
    }

    Collider enemyLightSee;
    Collider enemyHearPlayer;

    public enemyBehavior currentBehavior;

    NavMeshAgent myNavMeshAgent;

    // Use this for initialization
    void Start ()
    {
        enemySpawnRandom = UnityEngine.Random.Range(0, enemySpawnPoints.Length);
        gameObject.transform.position = enemySpawnPoints[enemySpawnRandom].transform.position;
        currentBehavior = enemyBehavior.patrol;
        myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        switch(currentBehavior)
        {
            case enemyBehavior.patrol:
                CallCoroutine("Patrol");
                break;
            case enemyBehavior.chase:
                CallCoroutine("Chase");
                break;
            case enemyBehavior.attack:
                CallCoroutine("Attack");
                break;
            case enemyBehavior.runAway:
                CallCoroutine("RunAway");
                break;
        }   
	}

    public void CallCoroutine(string name)
    {
        if (!runningCoroutine)
        {
            runningCoroutine = true;
            StopAllCoroutines();
            StartCoroutine(name);
        }
    }

    IEnumerator RunAway()
    {
        print("run away");
        enemySpawnRandom = 0;
        gameObject.transform.position = enemySpawnPoints[enemySpawnRandom].transform.position;
        yield return new WaitForFixedUpdate();
        currentBehavior = enemyBehavior.patrol;
        runningCoroutine = false;
    }    

    IEnumerator Attack()
    {
        //Game Over
        currentBehavior = enemyBehavior.chase;
        yield return new WaitForFixedUpdate();
        runningCoroutine = false;
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator Chase()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        myNavMeshAgent.SetDestination(player.transform.position);

        if (distanceFromPlayer <= attackRadius)
        {
            currentBehavior = enemyBehavior.attack;
        }
        if (distanceFromPlayer >= deAggro)
        {
            currentBehavior = enemyBehavior.patrol;
        }

        yield return new WaitForFixedUpdate();
        runningCoroutine = false;
    }

    IEnumerator Patrol()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
        Vector3 finalPosition = hit.position;
        myNavMeshAgent.SetDestination(finalPosition);

        yield return new WaitForSeconds(5f);
        runningCoroutine = false;
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            runningCoroutine = false;
            currentBehavior = enemyBehavior.chase;
        }
    }
}

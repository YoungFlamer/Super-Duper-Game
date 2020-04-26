using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour //автор тов. Сергей Иевлев
{
    enum EnemyState
    {
        Staying,
        Chasing,
        Attacking
    }
    private EnemyState state = EnemyState.Staying;

    private NavMeshAgent agent;
    private GameObject player;

    [Header("Navigation Properties")]
    [SerializeField]
    private string playerTag;
    [SerializeField]
    private float visionDistance;
    [SerializeField]
    private float attackDistance;

    [Header("Attack properties")]
    [SerializeField]
    private float reloadTime;
    private float currentReloadTime;

    [Header("Animation Properties")]
    [SerializeField]
    private string movingParameterName;
    [SerializeField]
    private string attackingParameterName;
    private Animator anim;
    private bool isAttackingOnThisFrame = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        player = GameObject.FindGameObjectWithTag(playerTag);
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isAttackingOnThisFrame = false; //потом её можно и поменять, просто по умолчанию пускай будет false
        UpdateState();
        Act();
        Reload();
    }

    private void Reload()
    {
        if (currentReloadTime < reloadTime)
        {
            currentReloadTime += Time.deltaTime;
        }
    }

    void UpdateState()
    {
        float dst = Vector3.Distance(transform.position, player.transform.position);
        if (dst > attackDistance && dst < visionDistance)
        {
            state = EnemyState.Chasing;
        }
        if (dst < attackDistance)
        {
            state = EnemyState.Attacking;
        }
        if (dst > visionDistance)
        {
            state = EnemyState.Staying;
        }
    }
    void Act()
    {
        if (state == EnemyState.Chasing && agent.isStopped)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        } else
        {
            agent.isStopped = true;
        }
        if (state == EnemyState.Attacking)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (currentReloadTime > reloadTime)
        {
            DamagePlayer();
            currentReloadTime = 0;
            isAttackingOnThisFrame = true;
        }
    }

    private void DamagePlayer()
    {
        //тут пока ничего не пишу - Серёга Иевлев
    }

    void Animate()
    {
        if (state == EnemyState.Chasing)
        {
            anim.SetBool(movingParameterName, true);
        }
        if (state == EnemyState.Staying)
        {
            anim.SetBool(movingParameterName, false);
        }
        if (state == EnemyState.Attacking && !isAttackingOnThisFrame)
        {
            anim.SetBool(movingParameterName, false);
        }
        if (state == EnemyState.Attacking && isAttackingOnThisFrame)
        {
            anim.SetTrigger(attackingParameterName);
        }
    }
}

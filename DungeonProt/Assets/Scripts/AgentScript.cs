using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{
    [SerializeField] Transform target;

    private NavMeshAgent agent;
    public Animator animator;
    Vector3 prevPosition;
    Vector3 deltaPosition;



    public float attackRange = 1f;
    public Transform attackPoint;

    public float attackRate = 1f;
    private float nextAttackTime = 0f;


    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; //��������� ������� ������
        agent = GetComponent<NavMeshAgent>();//��������� ������ navmesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (gameObject.GetComponentInChildren<Enemy>().currentHealth > 0)
        {
            agent.SetDestination(target.position);//����������, ���� ���� �����(������� �����)


            deltaPosition = agent.nextPosition - prevPosition; // ������� ������ �����������

            animator.SetFloat("Horizontal", deltaPosition.x); //���������� � ���������� ������ �����������
            animator.SetFloat("Vertical", deltaPosition.y);
            if (deltaPosition.x != 0f || deltaPosition.y != 0f) //���������� ��������� �������� ��� ������ ����������� ��������� �� 0
                animator.SetFloat("Speed", 1f);
            else
                animator.SetFloat("Speed", 0f);
            prevPosition = agent.nextPosition; //���������� ������� ��� ����������   //�� �������



            if (gameObject.GetComponent<CapsuleCollider2D>().enabled && Vector3.Distance(target.position, gameObject.transform.position) <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else
        {
            agent.SetDestination(GetComponent<Transform>().position);
            agent.enabled = false;
            GetComponent<AgentScript>().enabled = false;
        }
    }

    private void Attack()
    {
        target.gameObject.GetComponent<Player>().TakeDamage(10);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

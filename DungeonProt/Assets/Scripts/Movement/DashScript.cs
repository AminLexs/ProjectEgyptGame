using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    public float Cooldown;
    public float Distance;
    public float Speed;
    public float DestinationMultiplier;
    public Transform CursorTransform;
    public LayerMask layerMask;

    float cooldownTimer;
    bool blinking = false;
    Vector3 destination;
    ParticleSystem trail;

    void Start()
    {
        trail = transform.Find("Trail").GetComponent<ParticleSystem>();
        cooldownTimer = Cooldown;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Blink();
        }

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
        else
            cooldownTimer = Cooldown;

        if (blinking)
        {
            var dist = Vector3.Distance(transform.position, destination);
            if (dist > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * Speed);
            }
            else
                blinking = false;
        }
    }

    void Blink()
    {
        trail.Play();

        var heading = CursorTransform.position - transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;

        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, distance, layerMask);

        if (hit.collider.gameObject.name == "Cursor")
        {
            destination = CursorTransform.position;
        }
        else
        {
            //destination.x = hit.point.x - 1.1f * (((CursorTransform.position.x - transform.position.x) * 0.3f) / (Mathf.Abs(CursorTransform.position.y - transform.position.y)));
            //destination.y = hit.point.y - 1.1f * 0.3f * Mathf.Sign(CursorTransform.position.y - transform.position.y);

            var dx = CursorTransform.position.x - transform.position.x;
            var dy = Mathf.Abs(CursorTransform.position.y - transform.position.y);

            if (dy < 1)
                dy = 1;

            destination.x = hit.point.x - ((dx * 0.3f) / dy);
            destination.y = hit.point.y - 0.3f * Mathf.Sign(dy);
        }
        blinking = true;
    }
}

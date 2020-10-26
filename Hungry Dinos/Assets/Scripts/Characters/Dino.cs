using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Dino : MonoBehaviour
{
    #region Readonly Static Fields
    readonly static float moveSpeedFast = 70;
    readonly static float moveSpeedNormal = 40;
    readonly static float closeEnough = 1f; //0.05f;
    readonly static float distanceFromTargetRun = 100;
    #endregion

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private Animator animator;
    
    float startXScale;

    Vector2 direction;

    public Vector2 Target { get; private set; }
    float distanceFromTarget = 0f;

    [HideInInspector]
    public int Number { get; private set; }

    void Awake()
    {
        SetTarget(new Vector2(transform.position.x, transform.position.y));
        startXScale = transform.localScale.x;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.StartsWith("Tent"))
        {
            other.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (ArrivedAt(Target))
        {
            rigidbody2D.position = Target;
            SetSpeed();

            if (Target.x <= -40)
            {
                // Game Over
                //SceneManager.LoadScene("Game");
            }
        }
        else
        {
            MoveTo(Target);
        }  
    }

    private void MoveTo(Vector2 _target)
    {
        direction = (rigidbody2D.position - _target).normalized;
        SetScale();
        rigidbody2D.MovePosition(rigidbody2D.position - (direction * moveSpeed * Time.deltaTime));
    }

    private void SetSpeed()
    {
        distanceFromTarget = Vector2.Distance(rigidbody2D.position, Target);

        float speed = 0f;

        if (distanceFromTarget > 0 && distanceFromTarget <= distanceFromTargetRun)
        {
            speed = moveSpeedNormal;
        }
        else if (distanceFromTarget > distanceFromTargetRun)
        {
            speed = moveSpeedFast;
        }

        moveSpeed = speed;
        animator.SetFloat("Speed", moveSpeed);
    }

    private void SetScale()
    {
        if (Math.Sign(direction.x) != Math.Sign(transform.localScale.x))
            transform.localScale = direction.x < 0 ? new Vector2(-startXScale, transform.localScale.y)
                                                   : new Vector2(startXScale, transform.localScale.y);
    }

    private bool ArrivedAt(Vector2 _targetPosition)
    {
        if (Mathf.Abs(rigidbody2D.position.x - _targetPosition.x) <= closeEnough
            && Mathf.Abs(rigidbody2D.position.y - _targetPosition.y) <= closeEnough)
            return true;
        return false;
    }

    public void SetNumber(int _number)
    {
        Number = _number;
    }

    public void SetTarget(Vector2 _target)
    {
        Target = _target;
        SetSpeed();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DinoMovement : MonoBehaviour
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
    private MouseControls mouseControls;
    Vector2 input_mousePosition;

    Vector2 direction;

    Vector2 target;
    float distanceFromTarget = 0f;

    void Awake()
    {
        target = new Vector2(transform.position.x, transform.position.y);
        startXScale = transform.localScale.x;
        mouseControls = new MouseControls();
    }

    void OnEnable()
    {
        mouseControls.Enable();
    }

    void OnDisable()
    {
        mouseControls.Disable();
    }

    void Start()
    {
        

        mouseControls.PlayerMouse.Position.performed += mP => input_mousePosition = mP.ReadValue<Vector2>();
        mouseControls.PlayerMouse.SetTarget.performed += SetTarget;
    }

    private void SetTarget(InputAction.CallbackContext context)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(input_mousePosition);
        Vector2 cell = Grid.GetCellOfWorldPosition(mousePos);
        Vector2 position = Grid.GetWorldPositionAt(cell);

        target = position;

        Debug.Log("mousePos: " + mousePos);
        Debug.Log("cell: " + cell);
        Debug.Log("position: " + position);
        SetSpeed();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (ArrivedAt(target))
        {
            rigidbody2D.position = target;
            SetSpeed();
        }
        else
        {
            MoveTo(target);
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
        distanceFromTarget = Vector2.Distance(rigidbody2D.position, target);

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float amountToMoveY = default;
    [SerializeField] private float speed = default;
    [SerializeField] private Rigidbody2D rb = default;
    [SerializeField] private int trailDistance = default;
    [Header("Movement limit")]
    [SerializeField] private float maximumHeight = default;
    [SerializeField] private float minimumHeight = default;
    private Vector2 horizontalMovement = default;
    private Vector2 targetPosition = default;
    public bool limitFrontPedal = false;
    public bool limitBackPedal = false;
    private bool firingTrail = false;
    private LineRenderer lineRenderer;
    
    void Awake() 
    {
        //targetPosition = transform.position;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        horizontalMovement.x = Input.GetAxisRaw("Horizontal");
        DrivingLimitations();
        LaneSwitching();
        TrailSwitch();
    }

    void FixedUpdate() 
    {
        HorizontalMovement();
    }

    private void LaneSwitching()
    {
        if (Input.GetKeyDown(KeyCode.W) && transform.position.y < maximumHeight)
        {
            targetPosition = new Vector2(rb.position.x, targetPosition.y + amountToMoveY);
        }
        else if (Input.GetKeyDown(KeyCode.S) && transform.position.y > minimumHeight)
        {
            targetPosition = new Vector2(rb.position.x, targetPosition.y - amountToMoveY);
        }
    }

    private void HorizontalMovement()
    {
        horizontalMovement.x = horizontalMovement.x * speed * Time.deltaTime;
        rb.MovePosition(new Vector2(rb.position.x + horizontalMovement.x, rb.position.y + targetPosition.y));
        targetPosition = new Vector2(0,0);
    }

    private void DrivingLimitations()
    {
        if (limitFrontPedal && horizontalMovement.x > 0)
        {
            horizontalMovement.x = 0;
        }

        if (limitBackPedal && horizontalMovement.x < 0)
        {
            horizontalMovement.x = 0;
        }
    }

    private void TrailSwitch()
    {
        Vector3 behindPosition = transform.position + Vector3.left;
        Vector3[] teste = new Vector3[2];
        Vector3[] vazio = new Vector3[2];
        teste[0] = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
        teste[1] = transform.position + Vector3.left * trailDistance; 

        if(Input.GetKey(KeyCode.J))
        {
            firingTrail = true;
        }

        if(Input.GetKeyUp(KeyCode.J))
        {
            firingTrail = false;
            lineRenderer.SetPositions(vazio);
        }

        if (firingTrail)
        {
            Debug.Log("Atirando!");
            lineRenderer.SetPositions(teste);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) 
    {
        //Limita a aceleração ao chegar na parede invisivel da frente para não acontecer problemas de fricção
        if (col.CompareTag("InvisibleWallFront"))
        {
            limitFrontPedal = true;
        }

        if (col.CompareTag("InvisibleWallBehind"))
        {
            limitBackPedal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col) 
    {
        //Limita a aceleração ao chegar na parede invisivel da frente para não acontecer problemas de fricção
        if (col.CompareTag("InvisibleWallFront"))
        {
            limitFrontPedal = false;
        }

        if (col.CompareTag("InvisibleWallBehind"))
        {
            limitBackPedal = false;
        }
    }
}

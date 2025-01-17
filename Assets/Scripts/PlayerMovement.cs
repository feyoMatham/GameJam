using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float movePos = 1f;
    private float moveTimeCount;
    private float moveTime = 0.5f;
    private bool isMoveFinished;
    private bool isEnterPressed;
    private bool moveForwardFinished;

    public Queue playerInputQueue = new Queue();
    private Stack playerBackwardStack = new Stack();
    private int playerInputCount = 0;
    private int playerBackwardCount = 0;
    public string playerCurInput;

    private Transform groundCheck;
    private Transform upCheck;
    private Transform leftCheck;
    private Transform rightCheck;
    private Transform trigger;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    private bool isDownBlocked;
    private bool isUpBlocked;
    private bool isLeftBlocked;
    private bool isRightBlocked;

    private List<Sprite> playerInputList = new List<Sprite>();

    void Start()
    {
        playerInputCount = 0;
        isMoveFinished = true;
        isEnterPressed = false;
        moveForwardFinished = true;
        isGrounded = true;
        isDownBlocked = false;
        isUpBlocked = false;
        isLeftBlocked = false;
        isRightBlocked = false;

        groundCheck = transform.Find("GroundCheck");
        upCheck = transform.Find("UpCheck");
        leftCheck = transform.Find("LeftCheck");
        rightCheck = transform.Find("RightCheck");
        trigger = transform.Find("Trigger");
        trigger.gameObject.SetActive(false);
    }
    void Update()
    {
        Debug.Log(playerInputQueue.Count);

        moveTimeCount -= Time.deltaTime;
        if (moveTimeCount < 0)
        {
            moveTimeCount = moveTime;

            if (playerInputCount > 0 && isEnterPressed)
            {
                if (isGrounded)
                {
                    PlayerMoveForward();
                }
            }
            if (playerBackwardCount > 0 && moveForwardFinished)
            {
                if (isGrounded)
                {
                    PlayerMoveBackward();
                }
            }

            if (!isGrounded)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - movePos, transform.position.z);
            }
        }

        if (isMoveFinished)
        {
            PlayerInput();
        }

        CheckGround();
    }

    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            playerInputQueue.Enqueue("Right");
            playerInputCount++;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerInputQueue.Enqueue("Up");
            playerInputCount++;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerInputQueue.Enqueue("Trigger");
            playerInputCount++;
        }
        if (Input.GetKeyDown(KeyCode.Return) && isMoveFinished)
        {
            isEnterPressed = true;
            isMoveFinished = false;
            moveForwardFinished = false;
        }
    }

    private void PlayerMoveForward()
    {
        string input = (string)playerInputQueue.Dequeue();
        playerCurInput = input;
        playerInputCount--;

        if (input == "Trigger")
        {
            StartCoroutine(SetTrigger());
            playerBackwardStack.Push("Trigger");
            playerBackwardCount++;
        }
        else if (input == "Right")
        {
            if (!isRightBlocked)
            {
                transform.position = new Vector3(transform.position.x + movePos, transform.position.y, transform.position.z);
            }
            playerBackwardStack.Push("Left");
            playerBackwardCount++;
        }
        else if (input == "Up")
        {
            if (!isUpBlocked)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + movePos, transform.position.z);
            }
            playerBackwardStack.Push("Down");
            playerBackwardCount++;
        }

        if (playerInputCount == 0)
        {
            StartCoroutine(WaitForMoveFinish());
        }
    }

    IEnumerator WaitForMoveFinish()
    {
        yield return new WaitForSeconds(1f);
        moveForwardFinished = true;
        isEnterPressed = false;
    }

    private void PlayerMoveBackward()
    {

        Debug.Log("MoveBack");
        string input = (string)playerBackwardStack.Pop();
        playerBackwardCount--;

        if (input == "Trigger")
        {
            StartCoroutine(SetTrigger());
        }
        else if (input == "Left")
        {
            if (!isLeftBlocked)
            {
                transform.position = new Vector3(transform.position.x - movePos, transform.position.y, transform.position.z);
            }
        }
        else if (input == "Down")
        {
            if (!isGrounded || !isDownBlocked)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - movePos, transform.position.z);
            }
        }

        if (playerBackwardCount == 0)
        {
            isMoveFinished = true;

        }
    }

    IEnumerator SetTrigger()
    {
        trigger.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        trigger.gameObject.SetActive(false);
    }

    private void CheckGround()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer))
        {
            Collider2D collider = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
            if (collider.CompareTag("Platform"))
            {
                isDownBlocked = false;
            }
            else
            {
                isDownBlocked = true;
            }
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (Physics2D.OverlapBox(upCheck.position, groundCheckSize, 0f, groundLayer))
        {
            Collider2D collider = Physics2D.OverlapBox(upCheck.position, groundCheckSize, 0f, groundLayer);
            if (collider.CompareTag("Platform"))
            {
                isUpBlocked = false;
            }
            else
            {
                isUpBlocked = true;
            }
        }
        else
        {
            isUpBlocked = false;
        }

        if (Physics2D.OverlapBox(leftCheck.position, groundCheckSize, 0f, groundLayer))
        {
            isLeftBlocked = true;
        }
        else
        {
            isLeftBlocked = false;
        }

        if (Physics2D.OverlapBox(rightCheck.position, groundCheckSize, 0f, groundLayer))
        {
            isRightBlocked = true;
        }
        else
        {
            isRightBlocked = false;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}

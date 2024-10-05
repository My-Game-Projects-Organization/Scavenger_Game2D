using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObj : MonoBehaviour
{
    public float moveTime = .05f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    protected bool isMoving = false;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f/moveTime;

    }

    // check canMove by raycasthit 
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir,yDir);

        // avoid raycast by yourself
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start,end,blockingLayer);
        boxCollider.enabled = true;

        Debug.Log("hit: " + hit.transform + " " + isMoving.ToString());
        if(hit.transform == null && !isMoving)
        {
            Debug.Log("Start moving");
            //StartCoroutine(SmoothMovement(end));
            SmoothMovement(end);
            return true;
        }
        return false;
    }

    protected void SmoothMovement(Vector3 end)
    {
        isMoving = true;

        //float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //while (sqrRemainingDistance > float.Epsilon)
        //{
        //    Vector3 newPos = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
        //    rb2D.MovePosition(newPos);
        //    sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        //    yield return null;
        //}
        end = new Vector3(Mathf.Round(end.x),Mathf.Round(end.y),end.z);
        rb2D.MovePosition(end);

        isMoving = false;   
    }

    /*
    Moves the object in the specified direction 
    Check for collision
    Get the components of the collision object
    If conversion is not possible and the object has a different component typeT , call the OnCanMove method to handle the interaction.
     */

    protected virtual void AttempMove <T> (int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;
        T hitComponent = hit.transform.GetComponent<T>();
        if(!canMove && hitComponent != null) 
            OnCanMove(hitComponent);
    }

    protected abstract void OnCanMove <T> (T component) 
        where T : Component;
}

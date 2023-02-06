using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualChild : MonoBehaviour
{

    public enum ChildState
    {
        Empty,
        Activated,
        WaitingToBeSelected,
        Selected,
        Locked,
        GivenAway
    }

    const float y_ActivatedOffset = 1.2f;
    const float y_EmptyOffset = -2.0f;

    float y_offset;
    float bouncePhase = 0.0f;
    float currentScale = 1.0f;

    public float bounceAmplitude = 1.0f;
    public float bounceSpeed = 1.0f;

    ChildState currentState;

    Vector3 unselectedPos;
    Quaternion unselectedRotation;
    Vector3 currentPos;

    public Child myChild;

    // Start is called before the first frame update
    void Start()
    {
        InitializeChild();
    }

    public void InitializeChild()
    {
        unselectedPos = transform.position;
        unselectedRotation = transform.rotation;
        currentPos = unselectedPos;
        y_offset = y_EmptyOffset;
        currentState = ChildState.Activated;
        currentScale = 0.0f;

    }

    private void SetChildState(ChildState cs)
    {
        if (currentState == ChildState.Selected || currentState==ChildState.Locked)
        {
            GameLogic.getInstance().DeSelectChild(this.gameObject);
        }

        currentState = cs;

        if (cs == ChildState.Selected)
        {
            GameLogic.getInstance().SelectChild(this.gameObject);
        }
    }

    private void LateUpdate()
    {
        if (currentState==ChildState.Locked)
        {
            Vector3 targetPos= GameLogic.getInstance().GetSelectedOffset(this.gameObject);
            
            currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 30.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation,Camera.main.transform.rotation, Time.deltaTime * 5.0f);
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
            SetChildState(ChildState.Selected);

        if (Input.GetKeyDown(KeyCode.S))
            SetChildState(ChildState.WaitingToBeSelected);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 toChild = transform.position - Camera.main.transform.position;
            toChild.Normalize();

            if (Vector3.Dot(toChild,ray.direction.normalized)>0.9999 && GameLogic.getInstance().HasEmptySelectionSlot())
            {
                 SetChildState(ChildState.Selected);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentState == ChildState.Selected || currentState == ChildState.Locked)
                SetChildState(ChildState.WaitingToBeSelected);
        }

        bool bounce = true;
        switch (currentState)
        {
            case ChildState.Empty:
                {
                    bounce = false;
                    if (currentScale > 0)
                    {
                        currentScale -= Time.deltaTime;
                    }

                    y_offset = Mathf.Lerp(y_offset, y_EmptyOffset, Time.deltaTime);
                    transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                }
                break;
            case ChildState.Activated:
                {
                    bounce = true;
                    if (currentScale < 1.0f)
                    {
                        currentScale += Time.deltaTime;
                    }
                    y_offset = Mathf.Lerp(y_offset, y_ActivatedOffset, Time.deltaTime);
                    transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                    transform.rotation = Quaternion.Slerp(transform.rotation, unselectedRotation, Time.deltaTime * 5.0f);
                }
                break;
            case ChildState.WaitingToBeSelected:
                {
                    bounce = true;
                    Vector3 targetPos = unselectedPos;
                    currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 10.0f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, unselectedRotation, Time.deltaTime*5.0f);
                }
                break;
            case ChildState.Selected:
            case ChildState.Locked:
                {
                    Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward*4.0f;
                    float dist = (currentPos - targetPos).magnitude;
                    if (dist<0.1f || currentState==ChildState.Locked)
                    {
                        currentState = ChildState.Locked;
                        currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 50.0f);
                        //currentPos = targetPos;
                    }
                    else
                    {
                        currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 10.0f);
                    }
                    
                    bounce = false;
                }
                break;

        }


        float currentBounceAmplitude = (bounce) ? bounceAmplitude : 0.0f;
        float verticalOffset = Mathf.Sin(bouncePhase) * currentBounceAmplitude;

        bouncePhase += Time.deltaTime * bounceSpeed;
        if (bouncePhase > Mathf.PI * 2.0f)
            bouncePhase -= Mathf.PI * 2.0f;

        Vector3 pos = currentPos;
        pos.y += y_offset + verticalOffset;
        transform.position = pos;
    }
}

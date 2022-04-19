using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CraftingAnims;

public class StateChanger : MonoBehaviour
{
    public GameObject player;
    public GameObject homePos;
    public List<GameObject> TargetPos;
    public float oneSecondMoveDistance = 0; //20�� ���� 3.5�� ������.

    CrafterActions actions;
    CrafterController crafterController;

    bool MoveOn = false;
    bool endCoroutine = true;

    float distance = 0;
    float moveTime = 0;
    float moveRate = 0;
    float rate = 0;

    int startNum;
    int targetNum;
    int swingCount = 0;

    Vector3 tmpVec;
    Vector3 StartPos;


    // Start is called before the first frame update
    void Start()
    {
        actions = player.GetComponent<CrafterActions>();
        crafterController = player.GetComponent<CrafterController>();

        TargetPos.Add(homePos); //����Ʈ�� �� ĭ �þ
        targetNum = TargetPos.Count - 1;
        startNum = TargetPos.Count - 1;

        ChangeTarget();
        CheckTargetAndSet();
    }

    // Update is called once per frame
    void Update()
    {
        rate += Time.deltaTime;
        if (rate > 10f)
        {
            rate = 0;
            ChangeTarget();
            CheckTargetAndSet();
        }

        if (MoveOn == true && moveRate <= 1)
        {
            moveRate += Time.deltaTime / moveTime;
            if (moveRate >= 1)
            {
                MoveOn = false;
                actions.TakeAction("WalkEnd");
                swingCount = 0;
            }
        }
        else if (MoveOn == false && moveRate >= 1 && endCoroutine  == true)
        {
            StartCoroutine(SetSwingAni());
        }


    }

    private void FixedUpdate()
    {
        if (MoveOn == true && moveRate <= 1)
        {
            MoveToTarget();
        }
    }

    void ChangeTarget()
    { //�ӽ�
        startNum = targetNum;
        targetNum = Random.Range(0, TargetPos.Count - 1); //�����δ� -2�������� ����ϰ� ��
        Debug.Log("���� Ÿ����" + targetNum + ", ���� Ÿ����" + startNum);
    }

    void CheckTargetAndSet()
    {
        tmpVec = TargetPos[targetNum].transform.position - player.transform.position;
        distance = tmpVec.magnitude;
        Debug.Log("�Ÿ�: " + distance);
        if (distance < 1f)
        {
            MoveOn = false;
            Debug.Log("Can't move, Distance less than 1.0f");
        }
        else
        {
            if(crafterController.charState == CrafterState.PickAxing || crafterController.charState == CrafterState.PickAxe)
                StartCoroutine(EndSwingAndWalkAni());
            else
                StartCoroutine(WalkAni());
        }
    }

    void CheckMoveTime()
    {
        // �Ÿ�/�ӷ� = �ð�
        // ��, distance / �ʴ� �̵��� �Ÿ�(�ִϸ��̼� ���� ����, ĳ���� ũ�⿡ ���) = moveTime
        moveTime = distance / oneSecondMoveDistance;
    }

    void TurnToTarget()
    {
        tmpVec = TargetPos[targetNum].transform.position;
        tmpVec.y = player.transform.position.y; //���ƺ��� y���� �� �ٲ��
        player.transform.LookAt(tmpVec);
    }

    void MoveToTarget()
    {
        player.transform.position =
            Vector3.Lerp(StartPos, TargetPos[targetNum].transform.position, moveRate);
    }

    IEnumerator SetSwingAni()
    {
        endCoroutine = false;
        if (crafterController.charState != CrafterState.PickAxing)
        {
            actions.TakeAction("Get PickAxe");
            yield return new WaitForSecondsRealtime(1.0f);

            actions.TakeAction("Start PickAxing");
            yield return new WaitForSecondsRealtime(1.0f);
        }
        actions.TakeAction("Swing Horizontal");
        swingCount++;
        yield return new WaitForSecondsRealtime(1.0f);
        endCoroutine = true;
    }

    IEnumerator EndSwingAndWalkAni()
    {
        if (crafterController.charState == CrafterState.PickAxing)
        {
            actions.TakeAction("Finish PickAxing");
            yield return new WaitForSecondsRealtime(1.0f);
        }

        if (crafterController.charState == CrafterState.PickAxe)
        {
            actions.TakeAction("Drop Item");
            yield return new WaitForSecondsRealtime(1.0f);
        }

        StartPos = player.transform.position;
        moveRate = 0;
        CheckMoveTime();
        TurnToTarget();

        actions.TakeAction("Walk");
        MoveOn = true;
    }

    IEnumerator WalkAni()
    {
        yield return new WaitForSecondsRealtime(0.0f);

        StartPos = player.transform.position;
        moveRate = 0;
        CheckMoveTime();
        TurnToTarget();

        actions.TakeAction("Walk");
        MoveOn = true;
    }
}


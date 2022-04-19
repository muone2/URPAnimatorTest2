using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CraftingAnims;

public class StateChanger : MonoBehaviour
{
    public GameObject player;
    public GameObject homePos;
    public List<GameObject> TargetPos;
    public CrafterActions actions;
    public CrafterController crafterController;

    bool MoveOn = true;

    float distance = 0;
    float moveTime = 0;
    float moveRate = 0;

    int startNum;
    int targetNum;

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

        ChangeState();
        CheckTargetAndSet();
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveOn == true && moveRate <= 1)
        {
            moveRate += Time.deltaTime / moveTime;
            if (moveRate >= 1)
                MoveOn = false;
        }
    }

    private void FixedUpdate()
    {
        if (MoveOn == true && moveRate <= 1)
        {
            MoveToTarget();
        }
    }

    void ChangeState()
    { //�ӽ�
        startNum = targetNum;
        targetNum = Random.Range(0, TargetPos.Count - 1); //�����δ� -2�������� ����ϰ� ��
        Debug.Log("���� Ÿ����" + targetNum + ", ���� Ÿ����" + startNum);
    }

    void CheckTargetAndSet()
    {
        tmpVec = TargetPos[targetNum].transform.position - player.transform.position;
        distance = tmpVec.magnitude;
        Debug.Log(distance);
        if (distance < 1f)
        {
            MoveOn = false;
            Debug.Log("Can't move, Distance less than 1.0f");
        }
        else
        {
            MoveOn = true;
            StartPos = player.transform.position;
            moveRate = 0;
            CheckMoveTime();
            TurnToTarget();
            crafterController.ChangeCharacterState(0f, CrafterState.Sit);
        }
    }

    void CheckMoveTime()
    {
        // �Ÿ�/�ӷ� = �ð�
        // ��, distance / �ʴ� �̵��� �Ÿ�(�ִϸ��̼� ���� ����, ĳ���� ũ�⿡ ���) = moveTime
        moveTime = distance / 5.0f; //�ӽ÷� 5 
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
}

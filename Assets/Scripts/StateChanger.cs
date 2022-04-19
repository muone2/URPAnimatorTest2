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

        TargetPos.Add(homePos); //리스트가 한 칸 늘어남
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
    { //임시
        startNum = targetNum;
        targetNum = Random.Range(0, TargetPos.Count - 1); //실제로는 -2지점까지 계산하게 됨
        Debug.Log("다음 타겟은" + targetNum + ", 이전 타겟은" + startNum);
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
        // 거리/속력 = 시간
        // 즉, distance / 초당 이동할 거리(애니메이션 보고 결정, 캐릭터 크기에 비례) = moveTime
        moveTime = distance / 5.0f; //임시로 5 
    }

    void TurnToTarget()
    {
        tmpVec = TargetPos[targetNum].transform.position;
        tmpVec.y = player.transform.position.y; //돌아봐도 y값은 안 바뀌게
        player.transform.LookAt(tmpVec);
    }

    void MoveToTarget()
    {
        player.transform.position = 
            Vector3.Lerp(StartPos, TargetPos[targetNum].transform.position, moveRate);
    }
}

using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MainBelts : MonoBehaviour
{
    [SerializeField] Transform Conveyor;
    Vector3 StartPos1position = new Vector3(-6.533f, 0.158f, -0.5f);
    Vector3 StartPos2position = new Vector3(-3.9f, 0.45f, -0.5f);
    Vector3 StartPos3position = new Vector3(-4.47f, 2.21f, -0.5f);
    Vector3 EndPos1position = new Vector3(-15.879f, 10f, -0.5f);
    Vector3 EndPos2position = new Vector3(-16.5f, 10f, -0.5f);
    Vector3 EndPos3position = new Vector3(-20.4f, 10f, -0.5f);
    Vector3 EndPos4position = new Vector3(-21.5f, 8.54f, -0.6f);
    Vector3 EndPos5position = new Vector3(-20.4f, 8f, -0.6f);
    Vector3 EndPos6position = new Vector3(-16.5f, 8f, -0.6f);
    Vector3 EndPos7position = new Vector3(-16f, 8, -0.6f);

    Quaternion StartPos1Rotation = Quaternion.Euler(0, 0, -215);
    Quaternion StartPos2Rotation = Quaternion.Euler(0, 0, -125);
    Quaternion StartPos3Rotation = Quaternion.Euler(0, 0, -35);
    Quaternion EndPos1Rotation = Quaternion.Euler(0, 0, -35);
    Quaternion EndPos2Rotation = Quaternion.Euler(0, 0, 0);
    Quaternion EndPos3Rotation = Quaternion.Euler(0, 0, 0);
    Quaternion EndPos4Rotation = Quaternion.Euler(0, 0, 90);
    Quaternion EndPos5Rotation = Quaternion.Euler(0, 0, 180);
    Quaternion EndPos6Rotation = Quaternion.Euler(0, 0, 180);
    Quaternion EndPos7Rotation = Quaternion.Euler(0, 0, -205);

    public static MainBelts instance;

    private bool isMoving = true;

    void Start()
    {
        StartCoroutine(ConveyorRoutine());
        MainConveyor.instance.GetSpeed();
    }

    private void Update()
    {
        MainConveyor.instance.IsOn();
        // 여기서 isMoving 상태를 체크하여 필요시 멈춤
        if (!MainConveyor.instance.IsOn())
        {
            isMoving = false;
        }
        else if (!isMoving)
        {
            isMoving = true; // 다시 시작
            StartCoroutine(ConveyorRoutine());
        }
    }

    IEnumerator ConveyorRoutine()
    {
        while (true) // 무한 루프
        {
            yield return MoveConveyor(StartPos1position, StartPos1Rotation, StartPos2position, StartPos2Rotation);
            yield return MoveConveyor(StartPos2position, StartPos2Rotation, StartPos3position, StartPos3Rotation);
            yield return MoveConveyor(StartPos3position, StartPos3Rotation, EndPos1position, EndPos1Rotation);
            yield return MoveConveyor(EndPos1position, EndPos1Rotation, EndPos2position, EndPos2Rotation);
            yield return MoveConveyor(EndPos2position, EndPos2Rotation, EndPos3position, EndPos3Rotation);
            yield return MoveConveyor(EndPos3position, EndPos3Rotation, EndPos4position, EndPos4Rotation);
            yield return MoveConveyor(EndPos4position, EndPos4Rotation, EndPos5position, EndPos5Rotation);
            yield return MoveConveyor(EndPos5position, EndPos5Rotation, EndPos6position, EndPos6Rotation);
            yield return MoveConveyor(EndPos6position, EndPos6Rotation, EndPos7position, EndPos7Rotation);
            yield return MoveConveyor(EndPos7position, EndPos7Rotation, StartPos1position, StartPos1Rotation);
        }
    }

    IEnumerator MoveConveyor(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot)
    {
        while (!isMoving)
        {
            yield return null; // 대기
        }

        if (MainConveyor.instance.IsOn())
        {
            float journeyLength = Vector3.Distance(startPos, endPos);
            float journeyTime = journeyLength / MainConveyor.instance.GetSpeed();
            float time = 0f;

            while (time < journeyTime)
            {
                while (!isMoving)
                {
                    yield return null; // 대기
                }

                time += Time.deltaTime;
                Conveyor.localPosition = Vector3.Lerp(startPos, endPos, time / journeyTime);
                Conveyor.localRotation = Quaternion.Slerp(startRot, endRot, time / journeyTime);
                yield return null;
            }
        }
    }
}

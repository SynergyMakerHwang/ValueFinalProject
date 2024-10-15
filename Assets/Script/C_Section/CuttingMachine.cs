using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CuttingMachine : MonoBehaviour
{
    [SerializeField] GameObject CuttingPart;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    public static CuttingMachine Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public bool BladeLsPLC;
    public bool RightTottSensorPLC;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("토트박스센서"))
        {
            RightTottSensorPLC = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("토트박스센서"))
        {
            RightTottSensorPLC = false;
        }

    }
IEnumerator MoveCuttingPart()
{
    float CurrentTime = 0;
    float duration = 1;

    // Go 동작
    while (CurrentTime < duration)
    {
        CurrentTime += Time.deltaTime;
        CuttingPart.transform.localPosition = Vector3.Lerp(StartPos.localPosition, EndPos.localPosition, CurrentTime / duration);
        yield return null;
    }
    // 위치 정리
    CuttingPart.transform.localPosition = EndPos.localPosition;
    BladeLsPLC = true;

    // Back 동작을 위한 리셋
    CurrentTime = 0; // 시간을 리셋합니다.

    while (CurrentTime < duration)
    {
        CurrentTime += Time.deltaTime;
        CuttingPart.transform.localPosition = Vector3.Lerp(EndPos.localPosition, StartPos.localPosition, CurrentTime / duration);
        yield return null;
    }
    // 위치 정리
    CuttingPart.transform.localPosition = StartPos.localPosition;
    BladeLsPLC = false;
}

    public void BladeGoBackPLC()
    {
        StartCoroutine(MoveCuttingPart());
    }
 
}

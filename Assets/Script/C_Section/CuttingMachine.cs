using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CuttingMachine : MonoBehaviour
{
    [SerializeField] GameObject CuttingPart;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;


  public  bool BladeLsPLC;
    bool RightTottSensorPLC;


    // 정위치 센서 펄링 펄스로 한번만 불러오기때문에, exit 없어도 됨
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("토트박스"))
        {
            RightTottSensorPLC = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("토트박스"))
        {
            RightTottSensorPLC = false;
        }

    }
    private void Start()
    {
  
    }

    IEnumerator Go()
    {

        float CurrentTime = 0;
        float duration = 2;

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            CuttingPart.transform.localPosition = Vector3.Lerp(StartPos.localPosition, EndPos.localPosition, CurrentTime / duration);
            yield return null;
        }
        //위치 정리
        CuttingPart.transform.localPosition = EndPos.localPosition;
        BladeLsPLC = true;
    }
    IEnumerator Back()
    {

        float CurrentTime = 0;
        float duration = 2;

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            CuttingPart.transform.localPosition = Vector3.Lerp(EndPos.localPosition, StartPos.localPosition, CurrentTime / duration);
            yield return null;
        }
        //위치 정리
        CuttingPart.transform.localPosition = StartPos.localPosition;
        BladeLsPLC = false;
    }
    public void BladeGoPLC()
    {
        StartCoroutine(Go());
    }
    public void BaldeBackPLC()
    {
        StartCoroutine(Back());
    }
}

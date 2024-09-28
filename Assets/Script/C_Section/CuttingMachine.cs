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


    // ����ġ ���� �޸� �޽��� �ѹ��� �ҷ����⶧����, exit ��� ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("��Ʈ�ڽ�����"))
        {
            RightTottSensorPLC = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("��Ʈ�ڽ�����"))
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
        //��ġ ����
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
        //��ġ ����
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

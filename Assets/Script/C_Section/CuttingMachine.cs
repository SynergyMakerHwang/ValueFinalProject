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
IEnumerator MoveCuttingPart()
{
    float CurrentTime = 0;
    float duration = 1;

    // Go ����
    while (CurrentTime < duration)
    {
        CurrentTime += Time.deltaTime;
        CuttingPart.transform.localPosition = Vector3.Lerp(StartPos.localPosition, EndPos.localPosition, CurrentTime / duration);
        yield return null;
    }
    // ��ġ ����
    CuttingPart.transform.localPosition = EndPos.localPosition;
    BladeLsPLC = true;

    // Back ������ ���� ����
    CurrentTime = 0; // �ð��� �����մϴ�.

    while (CurrentTime < duration)
    {
        CurrentTime += Time.deltaTime;
        CuttingPart.transform.localPosition = Vector3.Lerp(EndPos.localPosition, StartPos.localPosition, CurrentTime / duration);
        yield return null;
    }
    // ��ġ ����
    CuttingPart.transform.localPosition = StartPos.localPosition;
    BladeLsPLC = false;
}

    public void BladeGoBackPLC()
    {
        StartCoroutine(MoveCuttingPart());
    }
 
}

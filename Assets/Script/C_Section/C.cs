using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class C : MonoBehaviour
{
    // �Ǻ����� ��¦ ������
    [SerializeField] bool DoorValue;
    [SerializeField] Transform Pivot;

    bool DoorCheck = true;

    //2�ʾȿ� ���ݱ� ����
    float duration = 2;
    private void Update()
    {

        // ���ǿ� ���� ���� �޴´�.
        if (Pivot.transform.localRotation.eulerAngles.y == 0 && DoorValue == true)
        {
            StartCoroutine(OpenClose(DoorValue));
        }
        if (Pivot.transform.localRotation.eulerAngles.y == 90 && DoorValue == false)
        {
            StartCoroutine(OpenClose(DoorValue));
        }
    }


    IEnumerator OpenClose(bool Btn)
    {
        //�ʱⰪ ����
        float StartDoorY = Pivot.transform.localRotation.eulerAngles.y;
        float EndDoorY = Btn ? 90 : 0;
        float CurrentTIme = 0;

        while (CurrentTIme < duration)
        {
            CurrentTIme += Time.deltaTime;
            float NewY = Mathf.Lerp(StartDoorY, EndDoorY, CurrentTIme / duration);
            Pivot.transform.localRotation = Quaternion.Euler(0, NewY, 0);
            yield return null;
        }



    }

}

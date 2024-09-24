using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CuttingMachine : MonoBehaviour
{
    [SerializeField] GameObject CuttingPart;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] GameObject 칸막이;
    GameObject CreatedWall;
    bool IsSensor;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("토트박스"))
        {
           
            Cutter.Instance.TurnOnOff();


            if (IsSensor)
            {
                StartCoroutine(GOGOSSING());


            }
        }
    }
    IEnumerator GOGOSSING()
    {
        yield return StartCoroutine(CuttingGo(StartPos.localPosition, EndPos.localPosition));
        yield return StartCoroutine(CuttingGo(EndPos.localPosition, StartPos.localPosition));

        yield return Cutter.Instance.TurnOnOff();
       // Destroy(CreatedWall);

    }

    // 절단기계 구동해주는
    IEnumerator CuttingGo(Vector3 From, Vector3 to)
    {
        float CurrentTime = 0;
        float duration = 2;

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            CuttingPart.transform.localPosition = Vector3.Lerp(From, to, CurrentTime / duration);
            yield return null;
        }
    }
}

using System.Collections;
using UnityEditor;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField] GameObject CuttingPart;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] GameObject 칸막이;

    bool IsSensor;
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Banana") || other.name.Contains("Apples"))
        {
            Cutter.Instance.TurnOnOff();
            // 관성떄문에 컨베이어가 멈추어도 칸막이로 막아주어야됨
            Instantiate(칸막이);
          
            IsSensor = true;
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

    }
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

using System.Collections;
using UnityEngine;

public class SubConveyor : MonoBehaviour
{
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] float duration;
    [SerializeField] bool Power;

    public static SubConveyor Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public bool SubConveyorOnOff()
    {
        return Power = !Power ;
    }
    private void Start()
    {
        //초기설정
        Belt.localPosition = StartPos.localPosition;
    }
    // Update is called once per frame
    void Update()
    {

        if (Power && Belt.localPosition == StartPos.localPosition)
            StartCoroutine(Moving(StartPos.localPosition, EndPos.localPosition));


    }
    IEnumerator Moving(Vector3 from, Vector3 to)
    {
        float CurrentTIme = 0;

        while (CurrentTIme < duration)
        {
            if (Power)
            {
                CurrentTIme += Time.deltaTime;
                Belt.localPosition = Vector3.Lerp(from, to, CurrentTIme / duration);
                yield return null;
            }
            else if (!Power)
            {
                Belt.localPosition = from;
                yield break;
            }
        }
        Belt.localPosition = from;
    }
}


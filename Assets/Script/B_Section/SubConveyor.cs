using System.Collections;
using UnityEngine;

public class SubConveyor : MonoBehaviour
{
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform MiddlePos1;
    [SerializeField] Transform MiddlePos2;
    [SerializeField] Transform EndPos;
    [SerializeField] float duration;
    [SerializeField] bool Power;
    [SerializeField] GameObject TottBox;

    public static SubConveyor Instance;

    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public bool SubConveyorOnOffPLC()
    {
        return Power = !Power;
    }
    private void Start()
    {
        //초기설정
        Belt.localPosition = StartPos.localPosition;
        SpawnTottPLC();
    }
    // Update is called once per frame
    void Update()
    {

        if (Power && Belt.localPosition == StartPos.localPosition)
        {
            StartCoroutine(Moving(StartPos.localPosition, MiddlePos1.localPosition));


        }
        else if (Power && Belt.localPosition == MiddlePos1.localPosition)
        {
            StartCoroutine(Moving(MiddlePos2.localPosition, EndPos.localPosition));

        }

    }
    IEnumerator Moving(Vector3 from, Vector3 to)
    {
        float CurrentTIme = 0;
        Belt.localPosition = from;
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
                Belt.localPosition = StartPos.localPosition;
                yield break;
            }
        }
        Belt.localPosition = to;
        if (Belt.localPosition == EndPos.localPosition)
        {
            Belt.localPosition = StartPos.localPosition;
        }

    }
    public void SpawnTottPLC()
    {
        Instantiate(TottBox);
    }
}


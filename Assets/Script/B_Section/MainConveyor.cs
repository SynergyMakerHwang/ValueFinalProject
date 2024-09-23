using UnityEngine;
using System;
using System.Collections;

public class MainConveyor : MonoBehaviour
{
    //속성 값

    [SerializeField] float speed;
    [SerializeField] bool ConveyorOn;
    [SerializeField] float WaterLevel;

    //생성개체들
    [SerializeField] GameObject Prefebs;
    [SerializeField] Transform PrefebsMom;
    [SerializeField] GameObject waters;
    [SerializeField] GameObject Apples;


    public static MainConveyor instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public float GetSpeed()
    {
        return speed;
    }
    public bool IsOn()
    {
        return ConveyorOn;
    }

    void Start()
    {


    }
    int CurrentNums = 0;
    private void Update()
    {
        if (CurrentNums == 0 && ConveyorOn)
            StartCoroutine(SpawnPrefabs());
    }

  
    private IEnumerator SpawnPrefabs()
    {



        for (int i = 0; i < 12; i++)
        {
            CurrentNums++;
            yield return new WaitForSeconds(0.55f); // 0.5초 대기
            Instantiate(Prefebs, PrefebsMom);

        }
        if (CurrentNums == 11)
            yield return null;


    }
    public IEnumerator WaterFlowPLC()
    {
        float moveDistance = 0.01f; // 이동 속도

        // Instantiate(waters); // Instantiate는 필요 없을 경우 주석 처리
        Vector3 originalPosition = waters.transform.localPosition; // 원래 위치 저장

        while (waters.transform.localPosition.y <= WaterLevel)
        {
            waters.transform.localPosition = new Vector3(originalPosition.x, waters.transform.localPosition.y + moveDistance, originalPosition.z);
            yield return new WaitForSeconds(0.01f);
        }


    }
    public bool MainConveyorOnOffPLC()
    {
        return ConveyorOn = !ConveyorOn;
    }

}




using UnityEngine;
using System;
using System.Collections;

public class WashserController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool ConveyorOn;
    [SerializeField] GameObject Prefebs;
    [SerializeField] GameObject waters;
    [SerializeField] float WaterLevel;
    public static WashserController instance;

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
        StartCoroutine(SpawnPrefabs());
        StartCoroutine(WaterFlow());
    }

    private IEnumerator SpawnPrefabs()
    {

        for (int i = 0; i < 12; i++)
        {
            yield return new WaitForSeconds(0.55f); // 0.5초 대기
            Instantiate(Prefebs);

        }
    }
    private IEnumerator WaterFlow()
    {
        float moveDistance = 0.1f; // 이동 거리
      
        // Instantiate(waters); // Instantiate는 필요 없을 경우 주석 처리
        Vector3 originalPosition = waters.transform.position; // 원래 위치 저장

        while (waters.transform.position.y < WaterLevel)
        {
            waters.transform.position = new Vector3(originalPosition.x, waters.transform.position.y + moveDistance, originalPosition.z);
            yield return new WaitForSeconds(0.1f);
        }
    }
}




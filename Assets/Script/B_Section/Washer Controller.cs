using UnityEngine;
using System;
using System.Collections;

public class WasherController : MonoBehaviour
{
    //�Ӽ� ��
 
    [SerializeField] float speed;
    [SerializeField] bool ConveyorOn;
    [SerializeField] float WaterLevel;

    //������ü��
    [SerializeField] GameObject Prefebs;
    [SerializeField] Transform PrefebsMom;
    [SerializeField] GameObject waters;
    [SerializeField] GameObject Apples;

    
    public static WasherController instance;

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
    public bool CommuteTott()
    {
        return ConveyorOn = false;
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
            yield return new WaitForSeconds(0.55f); // 0.5�� ���
            Instantiate(Prefebs, PrefebsMom);

        }
    }
    private IEnumerator WaterFlow()
    {
        float moveDistance = 0.01f; // �̵� �ӵ�

        // Instantiate(waters); // Instantiate�� �ʿ� ���� ��� �ּ� ó��
        Vector3 originalPosition = waters.transform.localPosition; // ���� ��ġ ����

        while (waters.transform.localPosition.y < WaterLevel)
        {
            waters.transform.localPosition = new Vector3(originalPosition.x, waters.transform.localPosition.y + moveDistance, originalPosition.z);
            yield return new WaitForSeconds(0.01f);
        }
    }
}




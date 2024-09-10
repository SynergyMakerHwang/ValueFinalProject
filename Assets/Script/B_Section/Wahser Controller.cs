using UnityEngine;

public class WahserController : MonoBehaviour
{
    [SerializeField] float speed;
    public static WahserController instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public float  GetSpeed()
    {
        return speed;   
    }
    void Start()
    {

    }

    void Update()
    {

    }
}

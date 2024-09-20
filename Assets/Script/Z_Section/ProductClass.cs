using UnityEngine;

public class ProductClass : MonoBehaviour
{

    static public ProductClass Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    public class Product
    {
        public string productid;
        public string productname;
        public string processlist;
    }
   
}

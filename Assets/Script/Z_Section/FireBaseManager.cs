using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// Firebase Realtime Database에 접속, 데이터를 읽고 쓴다.
/// </summary>
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    [Serializable]
    public class User
    {
        public string id;
        public string pw;
        public string role;
        public string products;
        public string history;
    }


    public class Product
    {
        public string productid;
        public string productname;
        public string processlist;        
    }


    public class Process
    {
        public string processid;
        public string processname;
        public string section;
        public string startposition;
        public string endposition;
        public string starthight;
        public string endhight;
        public string startrotation;
        public string endrotation;
    }

    [SerializeField] string dbURL = "";    
    public DatabaseReference dbRef;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
  

  
    void Start()
    {

        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);

        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

     
    }


    [SerializeField] Dictionary<string, Dictionary<string, string>> productGroupList = new Dictionary<string, Dictionary<string, string>>();
    [SerializeField] Dictionary<string, string> product = new Dictionary<string, string>();


    //데이터 가져오기
    public string ReadDataWithNewtonJson()
    {
        string result = "";
        dbRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Product product;
                foreach (var item in snapshot.Children)
                {
                    string json = item.GetRawJsonValue();
                    print(json);

                    product = JsonConvert.DeserializeObject<Product>(json);                    
                }

                print("데이터를 잘 받았습니다.");
                
            }
        });
        return result;
    }


}
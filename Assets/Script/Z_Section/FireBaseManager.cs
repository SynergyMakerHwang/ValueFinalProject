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


    //데이터 가져오기
   /* public string ReadDataWithNewtonJson()
    {
        string result = "";
        dbRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
               
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
    }*/
    public IEnumerable<DataSnapshot> ReadDataWithNewtonJsonDataSnapshot()
    {
        IEnumerable<DataSnapshot> result = null;
        dbRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;                
                result = snapshot.Children;               
                print("데이터를 잘 받았습니다.");
                
            }
        });
        return result;
    }


}
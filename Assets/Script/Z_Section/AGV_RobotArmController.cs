using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Net.WebSockets;

public class AGV_RobotArmController : MonoBehaviour
{
    public static AGV_RobotArmController instance;

    [Header("설정 관련")]
    public List<Step> steps = new List<Step>();
    [SerializeField] TMP_InputField fileNameVal;
    [SerializeField] int stepCnt;
    [SerializeField] int currentStep;
    [SerializeField] TMP_InputField stepVal;
    [SerializeField] TMP_Text totalStepTxt;
    [SerializeField] TMP_InputField speedVal;
    [SerializeField] TMP_InputField delayVal;
    [SerializeField] UnityEngine.UI.Toggle isGripperVal;
    [SerializeField] UnityEngine.UI.Toggle isGripperOpenVal;
    [SerializeField] TMP_InputField angleAVal;
    [SerializeField] TMP_InputField angleBVal;
    [SerializeField] TMP_InputField angleCVal;
    [SerializeField] TMP_InputField angleDVal;
    [SerializeField] TMP_InputField angleEVal;
    [SerializeField] TMP_InputField angleFVal;
    public GameObject canvas;

    [Header("로봇팔 관절")]
    [SerializeField] Transform DoF_1;
    [SerializeField] Transform DoF_2;
    [SerializeField] Transform DoF_3;
    [SerializeField] Transform DoF_4;
    [SerializeField] Transform DoF_5;
    [SerializeField] Transform DoF_6;



    float currentTime;
    bool isCycleAction = false;
    [SerializeField] bool isProcessCycleEndAction = false;
    [SerializeField] int tottCnt = 0;

    public bool IsCycleAction { get => isCycleAction; set => isCycleAction = value; }
    public bool IsProcessCycleEndAction { get => isProcessCycleEndAction; set => isProcessCycleEndAction = value; }
    public int TottCnt { get => tottCnt; set => tottCnt = value; }

    string fileName = "program.csv";
    string tmpFileName = "";
    [SerializeField] int publicTottIndex = 0;


    [Serializable]
    public class Step
    {
        public int StepNum { get => step; set => step = value; }
        [SerializeField] int step;

        public float Delay { get => delay; set => delay = value; }
        [SerializeField] float delay = 1;

        public bool isGripper { get => isGripperON; set => isGripperON = value; }
       [SerializeField] bool isGripperON;

        public bool isGripperOpen { get => isGripperOpenON; set => isGripperOpenON = value; }
        [SerializeField] bool isGripperOpenON;

        public float Speed { get => speed; set => speed = value; }
        [SerializeField] float speed = 1;


        public float angleAValue { get => angleA; set => angleA = value; }
        [SerializeField] float angleA;

        public float angleBValue { get => angleB; set => angleB = value; }
        [SerializeField] float angleB;

        public float angleCValue { get => angleC; set => angleC = value; }
        [SerializeField] float angleC;


        public float angleDValue { get => angleD; set => angleD = value; }
        [SerializeField] float angleD;

        public float angleEValue { get => angleE; set => angleE = value; }
        [SerializeField] float angleE;

        public float angleFValue { get => angleF; set => angleF = value; }
        [SerializeField] float angleF;






        public Step(int _step, float _speed, float _delay, bool _isGripperOn, bool _isGripperOpenOn)
        {
            step = _step;
            speed = _speed;
            delay = _delay;
            isGripperON = _isGripperOn;
            isGripperOpenON = _isGripperOpenOn;
        }
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clearSetting();
    }


    public void OnTeachBtnClkEvent()
    {


        Step step;
        string TmpNum = "0";
        if (stepVal.text != "" && stepVal != null)
        {
            TmpNum = Regex.Replace(stepVal.text, @"[^0-9]", "");
        }

        //수정
        if (int.Parse(TmpNum) < steps.Count && steps.Count > 0)
        {
            step = steps[int.Parse(TmpNum)];
            step.angleAValue = float.Parse(angleAVal.text);
            step.angleBValue = float.Parse(angleBVal.text);
            step.angleCValue = float.Parse(angleCVal.text);
            step.angleDValue = float.Parse(angleDVal.text);
            step.angleEValue = float.Parse(angleEVal.text);
            step.angleFValue = float.Parse(angleFVal.text);

            step.Delay = float.Parse(delayVal.text);
            step.Speed = float.Parse(speedVal.text);
            step.isGripper = isGripperVal.isOn;
            step.isGripperOpen = isGripperOpenVal.isOn;

            //추가
        }
        else
        {

            float delay = float.Parse(delayVal.text);
            float speed = float.Parse(speedVal.text);

            step = new Step(stepCnt++, speed, delay, isGripperVal.isOn, isGripperOpenVal.isOn);
            step.angleAValue = float.Parse(angleAVal.text);
            step.angleBValue = float.Parse(angleBVal.text);
            step.angleCValue = float.Parse(angleCVal.text);
            step.angleDValue = float.Parse(angleDVal.text);
            step.angleEValue = float.Parse(angleEVal.text);
            step.angleFValue = float.Parse(angleFVal.text);
            steps.Add(step);

        }

        stepVal.text = $"{stepCnt}";
        totalStepTxt.text = $"Total Step Count : {steps.Count}";
        SaveCSV(step);
    }

    private void SaveCSV(Step step)
    {
        if ("" != fileNameVal.text) {
            fileName=  fileNameVal.text;
        }

        FileStream fs = new FileStream(fileName, FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);

        string line = $"{step.StepNum},{step.Speed},{step.Delay}" +
            $",{step.angleAValue},{step.angleBValue},{step.angleCValue},{step.angleDValue},{step.angleEValue},{step.angleFValue}" +
            $",{step.isGripper},{step.isGripperOpen}";
        sw.WriteLine(line);

        sw.Close();
        fs.Close();
    }

    public void OnLoadCSVBtnClkEvent()
    {
        if ("" != fileNameVal.text)
        {
            fileName = fileNameVal.text;
        }
        else {
            fileNameVal.text = fileName;
        }

        
        steps.Clear();
        steps = new List<Step>();
        FileStream fs = new FileStream(fileName, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

        Step step;
        string line;
        string[] info;
        while ((line = sr.ReadLine()) != null)
        {
            info = line.Split(",");

            step = new Step(int.Parse(info[0]), float.Parse(info[1]), float.Parse(info[2]), bool.Parse(info[9]), bool.Parse(info[10]));
            step.angleAValue = float.Parse(info[3]);
            step.angleBValue = float.Parse(info[4]);
            step.angleCValue = float.Parse(info[5]);
            step.angleDValue = float.Parse(info[6]);
            step.angleEValue = float.Parse(info[7]);
            step.angleFValue = float.Parse(info[8]);
            //수정
            if (steps.Count > int.Parse(info[0]))
            {
                steps[int.Parse(info[0])] = step;
                //추가
            }
            else
            {
                steps.Add(step);
            }

        }
        totalStepTxt.text = $"Total Step Count : {steps.Count}";
        sr.Close();
        fs.Close();

    }
    public void excuteCycleEvent(string processFileName, int tottIndex)
    {
        publicTottIndex = tottIndex;
        if (loadCSVFileEvent(processFileName, tottIndex)) {          
            isCycleAction = true;        

            StartCoroutine(RunStep(1));
                 
            
        }
        
    }

    public bool loadCSVFileEvent(string processFileName, int tottIndex)
    {
        bool result = false;
        if (tmpFileName != processFileName) {        
            tmpFileName = processFileName;
        }

  
        if (tottIndex > TottCnt)
        {
            return false;
        }

        processFileName = tmpFileName + "_" + tottIndex + ".csv";
        fileNameVal.text = processFileName;

        steps.Clear();
        steps = new List<Step>();
        FileStream fs = new FileStream(processFileName, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

        Step step;
        string line;
        string[] info;
        while ((line = sr.ReadLine()) != null)
        {
            info = line.Split(",");

            step = new Step(int.Parse(info[0]), float.Parse(info[1]), float.Parse(info[2]), bool.Parse(info[9]), bool.Parse(info[10]));
            step.angleAValue = float.Parse(info[3]);
            step.angleBValue = float.Parse(info[4]);
            step.angleCValue = float.Parse(info[5]);
            step.angleDValue = float.Parse(info[6]);
            step.angleEValue = float.Parse(info[7]);
            step.angleFValue = float.Parse(info[8]);
            //수정
            if (steps.Count > int.Parse(info[0]))
            {
                steps[int.Parse(info[0])] = step;
                //추가
            }
            else
            {
                steps.Add(step);
            }

        }

        if (steps.Count > 0) {
            result = true;
        }

        totalStepTxt.text = $"Total Step Count : {steps.Count}";
        sr.Close();
        fs.Close();

        
        return result;
    }



    public void OnChangeGripperBtnClkEvent(UnityEngine.UI.Toggle isGripperON)
    {

        AGV_RobotArmGripper.instance.isGripperMode = isGripperON.isOn;
        //grip 해제시 자식 제거
        if (!isGripperON.isOn)
        {
            AGV_RobotArmGripper.instance.removeChild(isGripperON.isOn);
        }
        
    }

    public void OnChangeGripperOpenBtnClkEvent(UnityEngine.UI.Toggle isGripperOpenON)
    {
        
        //그리퍼 open
        if (isGripperOpenON.isOn)
        {            
            AGV_RobotArmGripper.instance.OnForwardBtnClkEvent();
        //그리퍼 close
        }
        else {
            AGV_RobotArmGripper.instance.OnBackwardBtnClkEvent();
        }
    }


    public void OnStepUpBtnClkEvent()
    {

        int enterStep = int.Parse(stepVal.text);
        currentStep = enterStep;
        currentStep++;

        if (currentStep >= steps.Count)
        {
            currentStep = 0;
        }

        stepVal.text = $"{currentStep}";

        getStepSetting(currentStep);
        StartCoroutine(RotateAngle(steps[enterStep], steps[currentStep]));
    }

    public void OnStepDownBtnClkEvent()
    {
        int enterStep = int.Parse(stepVal.text);
        currentStep = enterStep;
        currentStep--;

        if (currentStep < 0)
        {
            currentStep = steps.Count - 1;
        }

        stepVal.text = $"{currentStep}";

        getStepSetting(currentStep);
        if (enterStep != currentStep)
        {
            StartCoroutine(RotateAngle(steps[enterStep], steps[currentStep]));
        }

    }



    public void getStepSetting(int stepNum)
    {
        if (steps != null && steps.Count > 0)
        {
            Step stepSetting = steps[stepNum];
            stepVal.text = stepSetting.StepNum.ToString();
            delayVal.text = stepSetting.Delay.ToString();
            speedVal.text = stepSetting.Speed.ToString();
            isGripperVal.isOn = stepSetting.isGripper;
            isGripperOpenVal.isOn = stepSetting.isGripperOpen;

            angleAVal.text = stepSetting.angleAValue.ToString();
            angleBVal.text = stepSetting.angleBValue.ToString();
            angleCVal.text = stepSetting.angleCValue.ToString();
            angleDVal.text = stepSetting.angleDValue.ToString();
            angleEVal.text = stepSetting.angleEValue.ToString();
            angleFVal.text = stepSetting.angleFValue.ToString();
        }

    }


    public void OnClearBtnClkEvent()
    {
        clearSetting();
        //OriginRotation();
        StartCoroutine(movingMotor("origin", 0));

    }

    private void clearSetting()
    {
        stepCnt = 0;
        steps.Clear();
        stepVal.text = "0";
        delayVal.text = "0";
        speedVal.text = "0";
        totalStepTxt.text = "Total Step Count : 0";

        angleAVal.text = "0";
        angleBVal.text = "0";
        angleCVal.text = "0";
        angleDVal.text = "0";
        angleEVal.text = "0";
        angleFVal.text = "0";
        isGripperVal.isOn = false;
        isGripperOpenVal.isOn = false;


    }
    public void OnStopBtnClkEvent()
    {
        isCycleAction = false;
    }
    public void OnCycleBtnClkEvent()
    {
        isCycleAction = true;
        StartCoroutine(RunStep(0));
    }

    public void excuteSingleCycleEvent()
    {
        StartCoroutine(RunStep(1));
    }

    public void OnSingleCycleBtnClkEvent()
    {
        isCycleAction = true;
        StartCoroutine(RunStep(1));

    }

    bool isActAngle = true;

    public void OnAngleValUp(TMP_InputField target)
    {
        int limitAngle = 360;
        if (target.name.Contains("A "))
        {
            limitAngle = 160;
        }
        else if (target.name.Contains("B "))
        {
            limitAngle = 60;
        }
        else if (target.name.Contains("C "))
        {
            limitAngle = 90;
        }
        else if (target.name.Contains("D "))
        {
            limitAngle = 210;
        }
        else if (target.name.Contains("E "))
        {
            limitAngle = 125;
        }
        else if (target.name.Contains("F "))
        {
            limitAngle = 210;
        }


       
        if (isActAngle)
        {
            float angel = 0;
            if (target.text != null && target.text != "")
            {
                angel = float.Parse(target.text);
            }

            if (angel<=limitAngle) {
                angel += 0.1f;
                angel = angel % 360;
            }
            else {
                angel = limitAngle;
            }
            movingMotorRotation(target.name, angel);

            target.text = angel.ToString();


        }

    }

    public void OnAngleValExit()
    {
        isActAngle = false;
    }


    public void OnAngleValDown(TMP_InputField target)
    {

        int limitAngle = 360;
        if (target.name.Contains("A "))
        {
            limitAngle = 160;
        }
        else if (target.name.Contains("B "))
        {
            limitAngle = 76;
        }
        else if (target.name.Contains("C "))
        {
            limitAngle = 75;
        }
        else if (target.name.Contains("D "))
        {
            limitAngle = 210;
        }
        else if (target.name.Contains("E "))
        {
            limitAngle = 125;
        }
        else if (target.name.Contains("F "))
        {
            limitAngle = 210;
        }


        if (isActAngle)
        {
            float angel = 0;
            if (target.text != null && target.text != "")
            {
                angel = float.Parse(target.text);
            }

         
            if (angel >= -limitAngle)
            {
                angel -= 0.1f;
                angel = angel % 360;
            }
            else
            {
                angel = -limitAngle;
            }

            movingMotorRotation(target.name, angel);
            target.text = angel.ToString();


        }
        isActAngle = true;
    }

    public void movingMotorRotation(string targetName, float angle)
    {
        if (targetName.Contains("A "))
        {
            //1DoF - z축
            DoF_1.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        }
        else if (targetName.Contains("B "))
        {
            //2DoF - x축
            DoF_2.localRotation = Quaternion.Euler(new Vector3(angle, 0, 0));
        }
        else if (targetName.Contains("C "))
        {
            //3DoF - x축
            DoF_3.localRotation = Quaternion.Euler(new Vector3(angle, 0, 0));
        }
        else if (targetName.Contains("D "))
        {
            //4DoF - y축
            DoF_4.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));
        }
        else if (targetName.Contains("E "))
        {
            //5DoF - x축
            DoF_5.localRotation = Quaternion.Euler(new Vector3(angle, 0, 0));
        }
        else if (targetName.Contains("F "))
        {
            //6DoF - y축
            DoF_5.localRotation = Quaternion.Euler(new Vector3(0, angle,  0));
        }


        }


    public IEnumerator movingMotor(string targetName, float angle)
    {

        if (targetName.Contains("A "))
        {
            //1DoF - z축
            yield return (RotateSettingAngle("A", angle));

        }
        else if (targetName.Contains("B "))
        {
            //2DoF - x축
            yield return (RotateSettingAngle("B", angle));
        }
        else if (targetName.Contains("C "))
        {
            //3DoF - x축
            yield return (RotateSettingAngle("C", angle));
        }
        else if (targetName.Contains("D "))
        {
            //4DoF - y축
            yield return (RotateSettingAngle("D", angle));
        }
        else if (targetName.Contains("E "))
        {
            //5DoF - x축
            yield return (RotateSettingAngle("E", angle));
        }
        else if (targetName.Contains("F "))
        {
            //6DoF - y축
            yield return (RotateSettingAngle("F", angle));
        }
        else if (targetName.Contains("origin"))
        {

            yield return (RotateSettingAngle("origin", angle));

        }


    }




    //delay에 따라 step을 작동
    public IEnumerator RunStep(int requestRoutine)
    {
        
        int loopCnt = 0;
        isProcessCycleEndAction = false;
        while (isCycleAction)
        {
           
            if (steps.Count > 0)
            {
      

                if (requestRoutine != 0 && loopCnt >= requestRoutine)
                {
                    break;
                }

                loopCnt++;

                /****** Orgin Class ********/
                Step orginStep = new Step(0, 1, 1, false, false);
                orginStep.angleAValue = 0;
                orginStep.angleBValue = 0;
                orginStep.angleCValue = 0;
                orginStep.angleDValue = 0;
                orginStep.angleEValue = 0;
                orginStep.angleFValue = 0;
                /**************************/


                for (int i = 0; i < steps.Count; i++)
                {
                    getStepSetting(i);
                    // origin에서 사이클 시작
                    if (i == 0)
                    {
                        yield return (RotateAngle(orginStep, steps[i]));
                    }
                    else
                    {
                       

                        //cycle && 마지막 step일 경우, origin으로 
                        if (requestRoutine == 0 && i == steps.Count - 1)
                        {
                            yield return (RotateAngle(steps[i], orginStep));
                        }
                        else {
                            yield return (RotateAngle(steps[i - 1], steps[i]));
                        }
                      

                        if (steps.Count - 1 == i && publicTottIndex >= TottCnt)
                        {
                            isProcessCycleEndAction = true;
                        }


                    }

                }

              
               

            }
            else
            {
               
                break;
            }


        }
    }

    public void OriginRotation()
    {
        //1DoF - z축
        DoF_1.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //2DoF - x축
        DoF_2.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //3DoF - x축
        DoF_3.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //4DoF - y축
        DoF_4.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //5DoF - x축
        DoF_5.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //6DoF - y축
        DoF_6.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

    }

    //delay안에서 모터를 회전(slerp)
    IEnumerator RotateAngle(Step preStep, Step step)
    {

        if (step.Speed == 0)
        {
            step.Speed = 1;
        }

        if (step.Delay == 0)
        {
            step.Delay = 1;
        }

        currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime * step.Speed;

            if (currentTime > step.Delay)
            {
                currentTime = 0;

                break;
            }
            //1DoF - z축        
            DoF_1.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(0, 0, preStep.angleAValue)), Quaternion.Euler(new Vector3(0,  0, step.angleAValue)), currentTime / step.Delay);

            //2DoF - x축
            DoF_2.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(preStep.angleBValue, 0, 0)), Quaternion.Euler(new Vector3(step.angleBValue, 0, 0)), currentTime / step.Delay);

            //3DoF - x축
            DoF_3.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(preStep.angleCValue, 0, 0)), Quaternion.Euler(new Vector3(step.angleCValue, 0, 0)), currentTime / step.Delay);

            //4DoF - y축
            DoF_4.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(0, preStep.angleDValue, 0 )), Quaternion.Euler(new Vector3(0, step.angleDValue, 0)), currentTime / step.Delay);

            //5DoF - x축
            DoF_5.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(preStep.angleEValue, 0, 0)), Quaternion.Euler(new Vector3(step.angleEValue, 0, 0)), currentTime / step.Delay);

            //6DoF - y축
            DoF_6.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(0, preStep.angleFValue, 0)), Quaternion.Euler(new Vector3(0, step.angleFValue, 0)), currentTime / step.Delay);


            yield return new WaitForEndOfFrame();

        }


    }


    IEnumerator RotateSettingAngle(string moterType, float angle)
    {

        float delay = 2f;
        float speed = 1f;
        float currentTime = 0;

        Vector3 baseAVector = DoF_1.transform.localPosition;
        Vector3 baseBVector = DoF_2.transform.localPosition;
        Vector3 baseCVector = DoF_3.transform.localPosition;
        Vector3 baseDVector = DoF_4.transform.localPosition;
        Vector3 baseEVector = DoF_5.transform.localPosition;
        Vector3 baseFVector = DoF_6.transform.localPosition;


        while (true)
        {
            currentTime += Time.deltaTime * speed;

            if (currentTime > delay)
            {
                currentTime = 0;
                break;
            }

            switch (moterType)
            {
                case "A":
                    //1DoF - z축
                    DoF_1.localRotation = Quaternion.Slerp(Quaternion.Euler(baseAVector), Quaternion.Euler(new Vector3(0,  0, angle)), currentTime / delay);
                    break;
                case "B":

                    //2DoF - x축
                    DoF_2.localRotation = Quaternion.Slerp(Quaternion.Euler(baseBVector), Quaternion.Euler(new Vector3(angle, 0, 0 )), currentTime / delay);
                    break;
                case "C":
                    //3DoF - x축
                    DoF_3.localRotation = Quaternion.Slerp(Quaternion.Euler(baseCVector), Quaternion.Euler(new Vector3(angle, 0, 0)), currentTime / delay);

                    break;
                case "D":
                    //4DoF - y축
                    DoF_4.localRotation = Quaternion.Slerp(Quaternion.Euler(baseDVector), Quaternion.Euler(new Vector3(0, angle , 0)), currentTime / delay);

                    break;

                case "E":
                    //5DoF - x축
                    DoF_5.localRotation = Quaternion.Slerp(Quaternion.Euler(baseEVector), Quaternion.Euler(new Vector3(angle, 0, 0)), currentTime / delay);
                    break;

                case "F":
                    //6DoF - y축
                    DoF_6.localRotation = Quaternion.Slerp(Quaternion.Euler(baseEVector), Quaternion.Euler(new Vector3(0, angle, 0)), currentTime / delay);
                    break;

                default:
                    //1DoF - y축
                    DoF_1.localRotation = Quaternion.Slerp(Quaternion.Euler(baseAVector), Quaternion.Euler(new Vector3(0, 0, 0)), currentTime / delay);
                    //2DoF - z축
                    DoF_2.localRotation = Quaternion.Slerp(Quaternion.Euler(baseBVector), Quaternion.Euler(new Vector3(0, 0, 0)), currentTime / delay);
                    //3DoF - x축
                    DoF_3.localRotation = Quaternion.Slerp(Quaternion.Euler(baseCVector), Quaternion.Euler(new Vector3(0, 0, 0)), currentTime / delay);
                    //4DoF - z축
                    DoF_4.localRotation = Quaternion.Slerp(Quaternion.Euler(baseDVector), Quaternion.Euler(new Vector3(0, 0, 0)), currentTime / delay);
                    //5DoF - x축
                    DoF_5.localRotation = Quaternion.Slerp(Quaternion.Euler(baseEVector), Quaternion.Euler(new Vector3(0, 0, 0)), currentTime / delay);
                    //6DoF - y축
                    DoF_6.localRotation = Quaternion.Slerp(Quaternion.Euler(baseEVector), Quaternion.Euler(new Vector3(0, 0, 0)), currentTime / delay);
                    break;
            }



            yield return new WaitForEndOfFrame();

        }


    }


}

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class RobotTest : MonoBehaviour
{
    public static RobotTest instance;
    public List<Step> steps = new List<Step>();
    [SerializeField] int stepCnt;
    [SerializeField] int currentStep;

    [SerializeField] TMP_InputField stepVal;
    [SerializeField] TMP_Text totalStepTxt;
    [SerializeField] TMP_InputField speedVal;
    [SerializeField] TMP_InputField delayVal;
    [SerializeField] UnityEngine.UI.Toggle isSuctionVal;
    [SerializeField] TMP_InputField angleAVal;
    [SerializeField] TMP_InputField angleBVal;
    [SerializeField] TMP_InputField angleCVal;
    [SerializeField] TMP_InputField angleDVal;
    [SerializeField] TMP_InputField angleEVal;
    public GameObject canvas;


    [SerializeField] Transform DoF_1;
    [SerializeField] Transform DoF_2;
    [SerializeField] Transform DoF_3;
    [SerializeField] Transform DoF_4;
    [SerializeField] Transform DoF_5;
    float currentTime;
    bool isCycleAction = false;

    public bool IsCycleAction { get => isCycleAction; set => isCycleAction = value; }

    string fileName = "program.csv";


    [Serializable]
    public class Step
    {
        public int StepNum { get => step; set => step = value; }
        [SerializeField] int step;

        public float Delay { get => delay; set => delay = value; }
        [SerializeField] float delay = 1;

        public bool isSuction { get => isSuctionON; set => isSuctionON = value; }
        [SerializeField] bool isSuctionON;

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






        public Step(int _step, float _speed, float _delay, bool _isSuctioinOn)
        {
            step = _step;
            speed = _speed;
            delay = _delay;
            isSuctionON = _isSuctioinOn;
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

            step.Delay = float.Parse(delayVal.text);
            step.Speed = float.Parse(speedVal.text);
            step.isSuction = isSuctionVal.isOn;

            //추가
        }
        else
        {

            float delay = float.Parse(delayVal.text);
            float speed = float.Parse(speedVal.text);

            step = new Step(stepCnt++, speed, delay, isSuctionVal.isOn);
            step.angleAValue = float.Parse(angleAVal.text);
            step.angleBValue = float.Parse(angleBVal.text);
            step.angleCValue = float.Parse(angleCVal.text);
            step.angleDValue = float.Parse(angleDVal.text);
            step.angleEValue = float.Parse(angleEVal.text);
            steps.Add(step);

        }

        stepVal.text = $"{stepCnt}";
        totalStepTxt.text = $"Total Step Count : {steps.Count}";
        SaveCSV(step);
    }

    private void SaveCSV(Step step)
    {

        FileStream fs = new FileStream(fileName, FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);

        string line = $"{step.StepNum},{step.Speed},{step.Delay}" +
            $",{step.angleAValue},{step.angleBValue},{step.angleCValue},{step.angleDValue},{step.angleEValue}" +
            $",{step.isSuction}";
        sw.WriteLine(line);

        sw.Close();
        fs.Close();
    }

    public void OnLoadCSVBtnClkEvent()
    {
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

            step = new Step(int.Parse(info[0]), float.Parse(info[1]), float.Parse(info[2]), bool.Parse(info[8]));
            step.angleAValue = float.Parse(info[3]);
            step.angleBValue = float.Parse(info[4]);
            step.angleCValue = float.Parse(info[5]);
            step.angleDValue = float.Parse(info[6]);
            step.angleEValue = float.Parse(info[7]);
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

    public void OnChangeSuctionBtnClkEvent(UnityEngine.UI.Toggle isSuctionON)
    {
        AGV_RobotArmGripper.instance.isSuctionMode = isSuctionON.isOn;
        //suction 해제시 자식 제거
        if (!isSuctionON.isOn)
        {

            AGV_RobotArmGripper.instance.removeChild(isSuctionON.isOn);
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
            isSuctionVal.isOn = stepSetting.isSuction;

            angleAVal.text = stepSetting.angleAValue.ToString();
            angleBVal.text = stepSetting.angleBValue.ToString();
            angleCVal.text = stepSetting.angleCValue.ToString();
            angleDVal.text = stepSetting.angleDValue.ToString();
            angleEVal.text = stepSetting.angleEValue.ToString();
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
        isSuctionVal.isOn = false;


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

        if (isActAngle)
        {
            float angel = 0;
            if (target.text != null && target.text != "")
            {
                angel = float.Parse(target.text);
            }
            angel += 1f;
            angel = angel % 360;
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
        if (isActAngle)
        {
            float angel = 0;
            if (target.text != null && target.text != "")
            {
                angel = float.Parse(target.text);
            }

            angel -= 1f;

            angel = angel % 360;
            movingMotorRotation(target.name, angel);
            target.text = angel.ToString();


        }
        isActAngle = true;
    }

    public void movingMotorRotation(string targetName, float angle)
    {
        if (targetName.Contains("A "))
        {
            //1DoF - y축
            DoF_1.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));

        }
        else if (targetName.Contains("B "))
        {
            //2DoF - z축
            DoF_2.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else if (targetName.Contains("C "))
        {
            //3DoF - x축
            DoF_3.localRotation = Quaternion.Euler(new Vector3(angle, 0, 0));
        }
        else if (targetName.Contains("D "))
        {
            //4DoF - z축
            DoF_4.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else if (targetName.Contains("E "))
        {
            //5DoF - x축
            DoF_5.localRotation = Quaternion.Euler(new Vector3(angle, 0, 0));
        }


    }


    public IEnumerator movingMotor(string targetName, float angle)
    {

        if (targetName.Contains("A "))
        {
            //1DoF - y축
            yield return (RotateSettingAngle("A", angle));

        }
        else if (targetName.Contains("B "))
        {
            //2DoF - z축
            yield return (RotateSettingAngle("B", angle));
        }
        else if (targetName.Contains("C "))
        {
            //3DoF - x축
            yield return (RotateSettingAngle("C", angle));
        }
        else if (targetName.Contains("D "))
        {
            //4DoF - z축
            yield return (RotateSettingAngle("D", angle));
        }
        else if (targetName.Contains("E "))
        {
            //5DoF - x축
            yield return (RotateSettingAngle("E", angle));
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
                Step orginStep = new Step(0, 1, 1, false);
                orginStep.angleAValue = 0;
                orginStep.angleBValue = 0;
                orginStep.angleCValue = 0;
                orginStep.angleDValue = 0;
                orginStep.angleEValue = 0;
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
                        yield return (RotateAngle(steps[i - 1], steps[i]));

                        //cycle && 마지막 step일 경우, origin으로 
                        if (requestRoutine == 0 && i == steps.Count - 1)
                        {
                            yield return (RotateAngle(steps[i], orginStep));
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
        //1DoF - y축
        DoF_1.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //2DoF - z축
        DoF_2.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //3DoF - x축
        DoF_3.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //4DoF - z축
        DoF_4.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //5DoF - x축
        DoF_5.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

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
            //1DoF - y축        
            DoF_1.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(0, preStep.angleAValue, 0)), Quaternion.Euler(new Vector3(0, step.angleAValue, 0)), currentTime / step.Delay);

            //2DoF - z축
            DoF_2.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(0, 0, preStep.angleBValue)), Quaternion.Euler(new Vector3(0, 0, step.angleBValue)), currentTime / step.Delay);

            //3DoF - x축
            DoF_3.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(preStep.angleCValue, 0, 0)), Quaternion.Euler(new Vector3(step.angleCValue, 0, 0)), currentTime / step.Delay);

            //4DoF - z축
            DoF_4.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(0, 0, preStep.angleDValue)), Quaternion.Euler(new Vector3(0, 0, step.angleDValue)), currentTime / step.Delay);

            //5DoF - x축
            DoF_5.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(preStep.angleEValue, 0, 0)), Quaternion.Euler(new Vector3(step.angleEValue, 0, 0)), currentTime / step.Delay);

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
                    //1DoF - y축
                    DoF_1.localRotation = Quaternion.Slerp(Quaternion.Euler(baseAVector), Quaternion.Euler(new Vector3(0, angle, 0)), currentTime / delay);
                    break;
                case "B":

                    //2DoF - z축
                    DoF_2.localRotation = Quaternion.Slerp(Quaternion.Euler(baseBVector), Quaternion.Euler(new Vector3(0, 0, angle)), currentTime / delay);
                    break;
                case "C":
                    //3DoF - x축
                    DoF_3.localRotation = Quaternion.Slerp(Quaternion.Euler(baseCVector), Quaternion.Euler(new Vector3(angle, 0, 0)), currentTime / delay);

                    break;
                case "D":
                    //4DoF - z축
                    DoF_4.localRotation = Quaternion.Slerp(Quaternion.Euler(baseDVector), Quaternion.Euler(new Vector3(0, 0, angle)), currentTime / delay);

                    break;

                case "E":
                    //5DoF - x축
                    DoF_5.localRotation = Quaternion.Slerp(Quaternion.Euler(baseEVector), Quaternion.Euler(new Vector3(angle, 0, 0)), currentTime / delay);
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
                    break;
            }



            yield return new WaitForEndOfFrame();

        }


    }


}

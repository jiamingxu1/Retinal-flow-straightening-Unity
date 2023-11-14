using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentRunner : MonoBehaviour
{
    private void Init()
    {
        Debug.Log("Init");

        isExperiment = true;
        TrialGenerator.Setup();
        DataWriter.Init();
        NullifyConditionChecks();
        trialState = "PreTrial";
    }

    private void NullifyConditionChecks()
    {
        playerReady    = false;
        playerFixating = false;
        timeFixating   = 0f;
    }

    private void PreTrial()
    {
        UpdateTrialDetails();
        ImageLoader.LoadImage(); //MAKE METHOD //load fixation 

        CameraBias.NullCameraBias(true);  //?

        //Debug.Log(currentTrialType + ", " + currentTrialEnvironment + ", #" + currentTrial.ToString());

        trialState = "StartCheck";
        NullifyConditionChecks();
    }

    private void StartConditions()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerReady = true;
        }

        if(!playerFixating)
        {
            playerFixating = 0f;
            return;
        }
       
        if(playerFixating < 0.5f)
        {
            playerFixating += Time.deltaTime; 
            return;
        }

        //CameraBias.NullCameraBias();
        CameraBias.SetBias(currentTrialType,currentTrialSign,currentTrialIncrement);
        //CameraBias.NullCameraBias(false);
        ImageLoader.LoadImage(currentTrialEnvironment);
        DataWriter.Open();
        trialState = "Trial";
        Debug.Log("Trial #" + currentTrial.ToString() + ": " + currentTrialType + "_" + currentTrialSign + "_" + currentTrialIncrement + ", " + currentTrialEnvironment);
    }

    private void Trial()
    {
        DataWriter.WriteData();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

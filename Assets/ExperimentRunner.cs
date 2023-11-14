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
        
        trialImages = ImageLoader.LoadTrialImages(subjI, trialNum); //INITIALIZE 

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

        if(!playerReady)
        {
            playerFixating = 0f;
            return;
        }
       
        if(playerFixating < 1f) //player has to fixate for 1 s to start the trial 
        {
            playerFixating += Time.deltaTime; 
            return;
        }

        //CameraBias.NullCameraBias();
        CameraBias.SetBias(currentTrialType,currentTrialSign,currentTrialIncrement); //?
        //CameraBias.NullCameraBias(false);
        DataWriter.Open();
        trialState = "Trial"; //advancing to the next state, start the trial
        Debug.Log("Trial #" + currentTrial.ToString() + ": " + currentTrialType + "_" + currentTrialSign + "_" + currentTrialIncrement + ", " + currentTrialEnvironment);
    }

    private void Trial()
    {
        StartCoroutine(SequenceTrial());
    }

    private IEnumerator SequenceTrial()
    {        
        // Show first image
        trialImages[0].SetActive(true);
        yield return new WaitForSeconds(0.2f); // Wait for 200 ms
        trialImages[0].SetActive(false);
        
        yield return new WaitForSeconds(0.15f); // Wait for 150 ms inter-trial-interval

        // Show second image
        trialImages[1].SetActive(true);
        yield return new WaitForSeconds(0.2f); // Wait for 200 ms
        trialImages[1].SetActive(false);

        yield return new WaitForSeconds(0.15f); // Wait for 150 ms inter-trial-interval

        // Show third image
        trialImages[2].SetActive(true);
        yield return new WaitForSeconds(0.2f); // Wait for 200 ms
        trialImages[2].SetActive(false);

        // After showing all images, proceed to get the response
        PromptForResponse();
        
        // Record response
        DataWriter.WriteData(); 
    }

    private void PromptForResponse()
    {
        // Code to prompt the user for a response
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

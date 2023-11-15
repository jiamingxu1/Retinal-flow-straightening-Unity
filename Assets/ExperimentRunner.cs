using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentRunner : MonoBehaviour
{
    
    // Define some objects and variables 
    public GameObject fixationCross; //CREATE IN UNITY
    public Text responsePrompt; // CREATE IN Unity UI

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
        fixationCross.SetActive(true); //present fixation cross

        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerReady = true;
        }

        if(!playerReady)
        {
            playerFixating = 0f;
            return;
        }
       
        if(playerFixating < 1f) //player has to fixate for 1s to start the trial 
        {
            playerFixating += Time.deltaTime; 
            return;
        }

        //CameraBias.NullCameraBias();
        CameraBias.SetBias(currentTrialType,currentTrialSign,currentTrialIncrement); //?
        //CameraBias.NullCameraBias(false);
        DataWriter.Open();
        //advancing to the next state, start the trial
        trialState = "Trial"; 
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

         // Hide fixation cross
        fixationCross.SetActive(false); 

        yield return new WaitForSeconds(0.15f); // Wait for 150 ms inter-trial-interval

        // After showing all images, proceed to get the response
        PromptForResponse();
    }

    private void PromptForResponse()
    {
        responsePrompt.text = "Please enter your response (1 or 2)";
        responsePrompt.gameObject.SetActive(true);
    }
   
    private void ProcessResponse(int response)
    {
    // Deactivate the prompt text
    responsePrompt.gameObject.SetActive(false);

    // Record response
    DataWriter.WriteData(response); 
    
    // Handle the response
    Debug.Log("User responded with: " + response);
    }

    private void EndConditon()
    {
        // check if response is properly saved 
        bool responseSaved = DataWriter.CheckIfRespSaved(userResponse) //MODIFY DATAWRITER
        if (responseSaved)
        {
            Debug.Log("Trial ended, response saved");
            userResponse = 0; //reset user response??
            DataWriter.Close();
            trialState = "PostTrial";
        }
        else
        {
            Debug.LogError("Error: response not saved.");
        }
    }

    private void PostTrial()
    {
        currentTrial++;
        trialState = "PreTrial";
    }
    
    private void Exit()
    {
        isExperiment = false;
    }
    
    private void UpdateTrialDetails()  //CHANGE THIS
    {
        currentTrialType = TrialGenerator.trialList[currentTrial].Split("_")[0];
        currentTrialSign = TrialGenerator.trialList[currentTrial].Split("_")[1];
        currentTrialIncrement = TrialGenerator.trialList[currentTrial].Split("_")[2];
        currentTrialEnvironment = TrialGenerator.trialList[currentTrial].Split("_")[3];
        //currentTrialEnvironment = "FullFlow";
    }

    private void Awake()
    {
        currentTrialType = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) Init();
        
        // HERE do eye tracker calibration
        
        if(!isExperiment) return;

        if(TrialGenerator.trialList == null) //CHANGE THIS
        {
            isExperiment = false;
            Debug.Log("Init failed.");
            return;
        }

        //Check for response
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            ProcessResponse(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            ProcessResponse(2);
        }
        
        //End of the experiment, all trials done
        if(currentTrial == TrialGenerator.trialList.Count)
        {
            Exit();
            return;
        }

        switch(trialState)
        {
            case "PreTrial":
                PreTrial();
                break;
            case "StartCheck":
                StartConditions();
                break;
            case "Trial":
                Trial();
                EndConditions();
                break;
            case "PostTrial":
                PostTrial();
                break;
            default:
                Debug.Log("ERROR: Missing/Incorrect Trial State.");
                Time.timeScale = 0;
                break;
        }
    }
}

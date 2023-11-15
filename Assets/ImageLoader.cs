using UnityEngine;

public static class ImageLoader
{
    public string subjI;
    public string currentTrial;

    public static GameObject[] LoadTrialImages(string aSubjI, int aCurrentTrial)
    {
        subjI = aSubjI;
        currentTrial = aCurrentTrial;

        GameObject[] trialImages = new GameObject[3]; //3 = number of images presented in a trial

        string basePath = "Images/" + subjI; // e.g., "Images/JX"

        for (int i = 0; i < 3; i++)
        {
            // Construct the full path, e.g., "Images/sujI/Image1"
            string path = basePath + "/Image" + (i + 1).ToString(); //CHANGE HERE - look this up in trial matrix
            trialImages[i] = Resources.Load<GameObject>(path);

            // Check if the image was loaded correctly
            if (trialImages[i] != null)
            {
                trialImages[i].SetActive(false); // Deactivate the image by default
            }
            else
            {
                Debug.LogError("Failed to load image at path: " + path);
            }
        }

        return trialImages;
    }
}

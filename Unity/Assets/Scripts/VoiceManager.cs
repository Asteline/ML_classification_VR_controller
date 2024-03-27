using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip testingNow, mistakes, missed, goodJob, accuracy, directions;
    public MLprediction mlpred;
    public Shooter shooterScript;
    public bool testing = true;
    public bool te, mi, miss, go, acc, dir = false;

    void Start()
    {
        audioSource.clip = testingNow;
        audioSource.Play();
    }

    private void LateUpdate()
    {
        //if (!audioSource.isPlaying && !te)
        //{
        //    audioSource.clip = testingNow;
        //    audioSource.Play();
        //    te = true;
        //}

        if ((shooterScript.missedInARow>2) && !mi)
        {
            audioSource.clip = mistakes;
            audioSource.Play();
            mi = true;
        }

        if ((shooterScript.madeInARow > 4) && !go)
        {
            audioSource.clip = goodJob;
            audioSource.Play();
            go = true;
        }

        if (mlpred.count>3 && mlpred.wrongPred && !miss)
        {
            audioSource.clip = missed;
            audioSource.Play();
            miss = true;
        }

        if (mlpred.perc > 89 && mlpred.count > 10 && !acc)
        {
            audioSource.clip = accuracy;
            audioSource.Play();
            acc = true;
        }
    }
}

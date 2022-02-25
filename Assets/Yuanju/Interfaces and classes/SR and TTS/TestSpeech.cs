using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechLib;
public class TestSpeech : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            var speech = new SpVoice();
            speech.Voice = speech.GetVoices(string.Empty, string.Empty).Item(0);
            //read speed，range from -10 to 10，0 by default
            speech.Rate = 2;
            //read volume，range from 0 to 100，100 by default
            speech.Volume = 100;

            speech.Speak("I'm talking", SpeechVoiceSpeakFlags.SVSFlagsAsync); //asynchronous read. 

        }

    }
}

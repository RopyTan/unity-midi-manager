using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiTest : MonoBehaviour {

	MidiManager midi;

	// Use this for initialization
	void Start () {
		midi = GetComponent<MidiManager>();
		midi.OnMidiInput += MidiInput;

		Debug.Log("input device num : " + midi.MidiInGetNumDevs());
		Debug.Log("output device num : " + midi.MidiOutGetNumDevs());

		midi.MidiInOpen(0);
	}

	void OnGUI(){
		if ( GUI.Button(new Rect(20, 20, 100, 50), "send") ) {
			midi.MidiOutSend(1, 0x007f3c91);
		}	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void MidiInput(int msg, int state, int picth, int velocity){
		Debug.Log(msg + " : " + state + " : " + picth + " : " + velocity);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
public class MidiManager : MonoBehaviour {

    public delegate void OnMidiInputHandler(int msg, int state, int picth, int velocity);
    public event OnMidiInputHandler OnMidiInput;

	delegate void MidiCallBack(long handle, int msg, int instance, int param1, int param2);

    const int MMSYSERR_NOERROR = 0;

    const int MMSYSERR_BADDEVICEID = 2;
    const int MMSYSERR_ALLOCATED = 4;
    const int MMSYSERR_NOMEM = 7;
    const int MMSYSERR_INVALPARAM = 11;
    const int MMSYSERR_NODEVICE = 68;

    const int MMSYSERR_INVALHANDLE = 5;
    const int MIDIERR_STILLPLAYING = 65;

    const uint MIDI_MAPPER = 0xffffffff;

    const int CALLBACK_NULL     = 0x00000000;
    const int CALLBACK_FUNCTION = 0x00030000;

    const int MIDI_OPEN  = 961;
    const int MIDI_CLOSE = 962;
    const int MIDI_DATA  = 963;

	long oMidi = 0;
	long iMidi = 0;

	static uint inDviceID = 0;
	static int inMessge = 0;
	static int inState = 0;
	static int inPicth = 0;
	static int inVelocity = 0;
	static bool isReceived = false; 

	enum MIDI_STATE{
		WAIT,
		OPEN,
		RE_OPEN,
		CLOSE,
		END,
	}
	static MIDI_STATE midiInState;
	// public MIDI_STATE midiOutState;

	Coroutine reConnection;

    // Midi API
    [DllImport("winmm.dll")]
    private static extern int midiInGetNumDevs();
    [DllImport("winmm.dll")]
    private static extern int midiOutGetNumDevs();
	[DllImport("winmm.dll", SetLastError=true)]
	private static extern int midiInOpen(ref long lphMidiIn, uint uDeviceID, MidiCallBack callback, int instance, int flags);
    [DllImport("winmm.dll")]
    private static extern int midiOutOpen(ref long handle, uint deviceID, MidiCallBack callback, int instance, int flags);  
    [DllImport("winmm.dll")]
    private static extern int midiInStart(long handle);
    [DllImport("winmm.dll")]
    private static extern int midiInStop(long handle);
	[DllImport("winmm.dll")]
    private static extern int midiOutShortMsg(long handle, int message);
    [DllImport("winmm.dll")]
    private static extern int midiInClose(long handle);
	[DllImport("winmm.dll")]
    private static extern int midiOutClose(long handle);
	
    void Start(){
		midiInState = MIDI_STATE.WAIT;
    }

	void Update(){
		if(inMessge == MIDI_CLOSE && midiInState == MIDI_STATE.CLOSE){
			reConnection = StartCoroutine(ReConnect());
			midiInState = MIDI_STATE.RE_OPEN;
		}
		if(isReceived){
			OnMidiInput(inMessge, inState, inPicth, inVelocity);
			isReceived = false;
		}
	}

	public int MidiInGetNumDevs(){
		return midiInGetNumDevs();
	}
	public int MidiOutGetNumDevs(){
		return midiOutGetNumDevs();
	}


	public void MidiInOpen(uint deviceID){

		inDviceID = deviceID;
		if(midiInState == MIDI_STATE.OPEN){
			return;
		}

		var res = midiInOpen(ref iMidi, deviceID, InCallBack, 0, CALLBACK_FUNCTION);
		res = midiInStart(iMidi);

		if(res != MMSYSERR_NOERROR && midiInState != MIDI_STATE.RE_OPEN){
			Debug.LogWarning("MIDI OPEN ERROR : code = " + res);
			midiInState = MIDI_STATE.RE_OPEN;
			reConnection = StartCoroutine(ReConnect());
			return;
		}

	}
	public void MidiOutOpen(uint deviceID){
		var res = midiOutOpen(ref oMidi, 1, OutCallBack, 0, CALLBACK_FUNCTION);
	}

	public void MidiOutSend(uint deviceID, int msg){
		var res = midiOutOpen(ref oMidi, deviceID, OutCallBack, 0, CALLBACK_FUNCTION);
		res = midiOutShortMsg(oMidi, msg);
		res = midiOutClose( oMidi);
	}

	void OnDisable(){
	 	var res = 0;
		midiInState = MIDI_STATE.END;
		res = midiInStop(iMidi);
		res = midiInClose(iMidi);
		res = midiOutClose(oMidi);
 	}

	IEnumerator ReConnect(){
		while(true){
			Debug.LogWarning("input midi no-connection. re-connection.");
			MidiInOpen(inDviceID);
			if(inMessge == MIDI_OPEN){
				yield break;
			}
			yield return new WaitForSeconds(1f);
		}
	}

	void InCallBack(long handle, int msg, int instance, int param1, int param2){
        inMessge = msg;
		inState = param1 & 0xff;
        inPicth = param1 >> 8 & 0xff;
        inVelocity = param1 >> 16 & 0xff;
		
		if(msg == MIDI_OPEN){
			midiInState = MIDI_STATE.OPEN;
		}
		else if(msg == MIDI_CLOSE){
			midiInState = MIDI_STATE.CLOSE;
		}
		else if(msg == MIDI_DATA){
			isReceived = true;
		}
    }
	
	void OutCallBack(long handle, int msg, int instance, int param1, int param2){
        // int state = param1 & 0xff;
        // int data1 = param1 >> 8 & 0xff;
        // int data2 = param1 >> 16 & 0xff;

		// Debug.Log("OUT : " + msg + "ch : " + state + ", pic : " + data1 + ", velo : " + data2);
    }
}

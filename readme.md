
# UnityでMIDI送受信を行うライブラリ

## 動作環境 
- Windows10
- Unity 2017


## Usage

#### Install
1. UnityPackageををダブルクリックしてインポートする.


#### Test

*MIDIを送受信テストシーン*

1. testシーンを開く.
2. Arduinoなどのシリアル通信デバイスを接続.
3. シーンを実行する.
4. Consoleに受信したメッセージが表示される.
5. Buttonを押すとMIDI Message(0x007f3c91)を送信

#### Method

*接続(受信)*

`MidiManager.MidiInOpen(uint deviceID);`

*受信イベント*

`MidiManager.OnMidiInput(int msg, int state, int picth, int velocity);`	

*送信*
	
`MidiManager.MidiOutSend(uint deviceID, int msg);`

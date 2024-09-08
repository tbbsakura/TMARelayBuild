// ver 1.002
// Copyright (c) 2023 Sakura(さくら) / tbbsakura
// MIT License. See "LICENSE" file.

using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using uOSC;
using System.IO;
using UnityEngine.UIElements;
using System;

namespace UnityEngine.UI
{

    [AddComponentMenu("OSC/SakuraVRMMuscle2OSC", 34)]
    public class SakuraVRMMuscle2OSC : MonoBehaviour
    {
/* 	old definition, not used
        private static readonly String[] _muscleNames = {
            "Left Shoulder Down-Up",//37
            "Left Shoulder Front-Back",//38
            "Left Arm Down-Up",//39
            "Left Arm Front-Back",//40
            "Left Arm Twist In-Out",//41
            "Left Forearm Stretch",//42
            "Left Forearm Twist In-Out",//43
            "Left Hand Down-Up",//44
            "Left Hand In-Out",//45

            "Right Shoulder Down-Up", //46 
            "Right Shoulder Front-Back", //47
            "Right Arm Down-Up", //48
            "Right Arm Front-Back", //49
            "Right Arm Twist In-Out", //50
            "Right Forearm Stretch", //51 
            "Right Forearm Twist In-Out", //52 
            "Right Hand Down-Up", //53 
            "Right Hand In-Out", //54 
        };
*/

        private static readonly String[] _muscleOSCParamDef = {
			"/avatar/parameters/TMARelay_SpineFrontBack", //0 Body
			"/avatar/parameters/TMARelay_SpineLeftRight", //1 Body
			"/avatar/parameters/TMARelay_SpineTwistLeftRight", //2  Body
			"/avatar/parameters/TMARelay_ChestFrontBack", //3  Body
			"/avatar/parameters/TMARelay_ChestLeftRight", //4  Body
			"/avatar/parameters/TMARelay_ChestTwistLeftRight", //5 Body
			"/avatar/parameters/TMARelay_UpperChestFrontBack", //6 Body(Ignore)
			"/avatar/parameters/TMARelay_UpperChestLeftRight", //7 Body(Ignore)
			"/avatar/parameters/TMARelay_UpperChestTwistLeftRight", //8 Body(Ignore)

			"/avatar/parameters/TMARelay_NeckNodDownUp", //9 HeadNeck
			"/avatar/parameters/TMARelay_NeckTiltLeftRight", //10 HeadNeck
			"/avatar/parameters/TMARelay_NeckTurnLeftRight", //11 HeadNeck
			"/avatar/parameters/TMARelay_HeadNodDownUp", //12 HeadNeck
			"/avatar/parameters/TMARelay_HeadTiltLeftRight", //13 HeadNeck
			"/avatar/parameters/TMARelay_HeadTurnLeftRight", //14 HeadNeck

			"/avatar/parameters/TMARelay_LeftEyeDownUp", //15 HeadNeck
			"/avatar/parameters/TMARelay_LeftEyeInOut", //16 HeadNeck
			"/avatar/parameters/TMARelay_RightEyeDownUp", //17 HeadNeck
			"/avatar/parameters/TMARelay_RightEyeInOut", //18 HeadNeck
			"/avatar/parameters/TMARelay_JawClose", //19 HeadNeck(Ignore)
			"/avatar/parameters/TMARelay_JawLeftRight", //20 HeadNeck(Ignore)

			//------- amimation template being prepared now (8 muscles? 7?)
			"/avatar/parameters/TMARelay_LeftUpperLegFrontBack", //21 LeftLeg
			"/avatar/parameters/TMARelay_LeftUpperLegInOut", //22 LeftLeg
			"/avatar/parameters/TMARelay_LeftUpperLegTwistInOut", //23 LeftLeg
			"/avatar/parameters/TMARelay_LeftLowerLegStretch", //24 LeftLeg
			"/avatar/parameters/TMARelay_LeftLowerLegTwistInOut", //25 LeftLeg
			"/avatar/parameters/TMARelay_LeftFootUpDown", //26 LeftLeg
			"/avatar/parameters/TMARelay_LeftFootTwistInOut", //27 LeftLeg
			"/avatar/parameters/TMARelay_LeftToesUpDown", //28 LeftLeg

			//------- amimation template being prepared now (8 muscles? 7?)
			"/avatar/parameters/TMARelay_RightUpperLegFrontBack", //29 RightLeg
			"/avatar/parameters/TMARelay_RightUpperLegInOut", //30 RightLeg
			"/avatar/parameters/TMARelay_RightUpperLegTwistInOut", //31 RightLeg
			"/avatar/parameters/TMARelay_RightLowerLegStretch", //32 RightLeg
			"/avatar/parameters/TMARelay_RightLowerLegTwistInOut", //33 RightLeg
			"/avatar/parameters/TMARelay_RightFootUpDown", //34 RightLeg
			"/avatar/parameters/TMARelay_RightFootTwistInOut", //35 RightLeg
			"/avatar/parameters/TMARelay_RightToesUpDown", //36 RightLeg

			//------- animation template prepared (5 muscles)
			"/avatar/parameters/TMARelay_ShoulderL_DownUp", //37 (Ignore)
			"/avatar/parameters/TMARelay_ShoulderL_FrontBack", //38 (Ignore)
			"/avatar/parameters/TMARelay_UpperArmL_DownUp", //39
			"/avatar/parameters/TMARelay_UpperArmL_FrontBack", //40
			"/avatar/parameters/TMARelay_UpperArmL_Twist", //41
			"/avatar/parameters/TMARelay_LowerArmL_Stretch", //42
			"/avatar/parameters/TMARelay_LowerArmL_Twist", //43
			"/avatar/parameters/TMARelay_HandL_DownUp", //44 (Ignore)
			"/avatar/parameters/TMARelay_HandL_InOut", //45 (Ignore)

			//------- animation template prepared (5 muscles)
			"/avatar/parameters/TMARelay_ShoulderR_DownUp", //46 (Ignore)
			"/avatar/parameters/TMARelay_ShoulderR_FrontBack", //47 (Ignore)
			"/avatar/parameters/TMARelay_UpperArmR_DownUp", //48 
			"/avatar/parameters/TMARelay_UpperArmR_FrontBack", //49
			"/avatar/parameters/TMARelay_UpperArmR_Twist", //50 
			"/avatar/parameters/TMARelay_LowerArmR_Stretch", //51
			"/avatar/parameters/TMARelay_LowerArmR_Twist", //52 
			"/avatar/parameters/TMARelay_HandR_DownUp", //53 (Ignore)
			"/avatar/parameters/TMARelay_HandR_InOut", //54 (Ignore)

			"/avatar/parameters/TMARelay_LeftThumb1Stretched", //55 LeftHand
			"/avatar/parameters/TMARelay_LeftThumbSpread", //56 LeftHand
			"/avatar/parameters/TMARelay_LeftThumb2Stretched", //57 LeftHand
			"/avatar/parameters/TMARelay_LeftThumb3Stretched", //58 LeftHand
			"/avatar/parameters/TMARelay_LeftIndex1Stretched", //59 LeftHand
			"/avatar/parameters/TMARelay_LeftIndexSpread", //60 LeftHand
			"/avatar/parameters/TMARelay_LeftIndex2Stretched", //61 LeftHand
			"/avatar/parameters/TMARelay_LeftIndex3Stretched", //62 LeftHand
			"/avatar/parameters/TMARelay_LeftMiddle1Stretched", //63 LeftHand
			"/avatar/parameters/TMARelay_LeftMiddleSpread", //64 LeftHand
			"/avatar/parameters/TMARelay_LeftMiddle2Stretched", //65 LeftHand
			"/avatar/parameters/TMARelay_LeftMiddle3Stretched", //66 LeftHand
			"/avatar/parameters/TMARelay_LeftRing1Stretched", //67 LeftHand
			"/avatar/parameters/TMARelay_LeftRingSpread", //68 LeftHand
			"/avatar/parameters/TMARelay_LeftRing2Stretched", //69 LeftHand
			"/avatar/parameters/TMARelay_LeftRing3Stretched", //70 LeftHand
			"/avatar/parameters/TMARelay_LeftLittle1Stretched", //71 LeftHand
			"/avatar/parameters/TMARelay_LeftLittleSpread", //72 LeftHand
			"/avatar/parameters/TMARelay_LeftLittle2Stretched", //73 LeftHand
			"/avatar/parameters/TMARelay_LeftLittle3Stretched", //74 LeftHand

			"/avatar/parameters/TMARelay_RightThumb1Stretched", //75 RightHand
			"/avatar/parameters/TMARelay_RightThumbSpread", //76 RightHand
			"/avatar/parameters/TMARelay_RightThumb2Stretched", //77 RightHand
			"/avatar/parameters/TMARelay_RightThumb3Stretched", //78 RightHand
			"/avatar/parameters/TMARelay_RightIndex1Stretched", //79v
			"/avatar/parameters/TMARelay_RightIndexSpread", //80 RightHand
			"/avatar/parameters/TMARelay_RightIndex2Stretched", //81 RightHand
			"/avatar/parameters/TMARelay_RightIndex3Stretched", //82v
			"/avatar/parameters/TMARelay_RightMiddle1Stretched", //83 RightHand
			"/avatar/parameters/TMARelay_RightMiddleSpread", //84 RightHand
			"/avatar/parameters/TMARelay_RightMiddle2Stretched", //85 RightHand
			"/avatar/parameters/TMARelay_RightMiddle3Stretched", //86 RightHand
			"/avatar/parameters/TMARelay_RightRing1Stretched", //87 RightHand
			"/avatar/parameters/TMARelay_RightRingSpread", //88 RightHand
			"/avatar/parameters/TMARelay_RightRing2Stretched", //89 RightHand
			"/avatar/parameters/TMARelay_RightRing3Stretched", //90 RightHand
			"/avatar/parameters/TMARelay_RightLittle1Stretched", //91 RightHand
			"/avatar/parameters/TMARelay_RightLittleSpread", //92 RightHand
			"/avatar/parameters/TMARelay_RightLittle2Stretched", //93 RightHand
			"/avatar/parameters/TMARelay_RightLittle3Stretched", //94 RightHand
			
        };
        private static String[] _muscleOSCParam = new String[95];
        private int _customParams = 0;

        [Tooltip("アバター、IPアドレス、ポートを変更する時はStartのチェックを2つともオフにしてください")]
        [SerializeField] private bool    _startRightArm = false;
        [Tooltip("アバター、IPアドレス、ポートを変更する時はStartのチェックを2つともオフにしてください")]
        [SerializeField] private bool    _startLeftArm = false;
        [Tooltip("ExpertMode用")]
        [SerializeField] private bool    _startExpertMode = false;

        [Tooltip("VRMアバターを入れます。変更時はStartのチェックを2つともオフにしてください")]
        public Animator _animationTarget;

        private String    _ipAddress = "127.0.0.1";
        private const int _portDestination = 9000;
        private const int _portListen = 39544;

        private HumanPose _targetHumanPose;
        private HumanPoseHandler _handler;
        private InputField _inputFieldIP;
        private InputField _inputFieldDestPort;
        private InputField _inputFieldListenPort;

        private Toggle _toggleServer;
        private Toggle _toggleServerCutEye;

        private Toggle _toggleLeftArm = null;
        private Toggle _toggleRightArm = null;
        private Toggle _toggleExpertMode = null;
        private Text _textExpertMode;
        private Button _buttonModeNormal;
        private GameObject _expertUI;
        private Text _topText;

        // Start is called before the first frame update
        void Start()
        {
            _muscleOSCParam = new string[_muscleOSCParamDef.Length];

            var server =  GameObject.Find("ExternalReceiver").GetComponent<uOscServer>();
            server.port = _portListen;

            _inputFieldIP = GameObject.Find("InputField_IP").GetComponent<InputField>();
            _inputFieldIP.text = _ipAddress;
            _inputFieldDestPort = GameObject.Find("InputField_Port").GetComponent<InputField>();
            _inputFieldDestPort.text = _portDestination.ToString();

            _inputFieldListenPort = GameObject.Find("InputField_ListenPort").GetComponent<InputField>();
            _inputFieldListenPort.text = _portListen.ToString();

            _toggleLeftArm = GameObject.Find("ToggleLeft").GetComponent<Toggle>();
            _toggleLeftArm.isOn = _startLeftArm;
            _toggleRightArm = GameObject.Find("ToggleRight").GetComponent<Toggle>();
            _toggleRightArm.isOn = _startRightArm;

            _toggleServer = GameObject.Find("ToggleServer").GetComponent<Toggle>();
            _toggleServer.isOn = true; // ExternalReceiverが Start してしまうので

            _toggleServerCutEye = GameObject.Find("ToggleServerCutEye").GetComponent<Toggle>();
            _toggleServerCutEye.isOn = true; 

            _toggleExpertMode = GameObject.Find("ToggleExpertMode").GetComponent<Toggle>();
            _textExpertMode = GameObject.Find("TextExpertMode").GetComponent<Text>();
            _buttonModeNormal= GameObject.Find("ButtonModeNormal").GetComponent<Button>();

            _expertUI = GameObject.Find("ExpertUI");
            _expertUI.SetActive(false);
            _topText = GameObject.Find("TopText").GetComponent<Text>();


        }

        private void ResetCustExParamArray()
        {
            for ( int i = 0; i < _muscleOSCParam.Length ; i++ ) 
            {
                _muscleOSCParam[i] = "";
            }
            _customParams = 0;
        }

        public int ReadCustomExParamList(string path)
        {   // txt ファイルを

            int countExParam = 0;
            ResetCustExParamArray();
            string data = File.ReadAllText(path);
            string[] strLines = data.Split(char.Parse("\n"));
            foreach (string strOneLine in strLines ) 
            {
                string[] parts = strOneLine.Split(char.Parse("\t"));
                if ( parts.Length < 2 ) continue;

                string[] parts2 = parts[1].Split(char.Parse("\""));
                if ( parts2.Length < 2 ) continue;
                
                string strIndex = parts[0];
                string strAddress = parts2[1];
                if ( strAddress.Length < 2 ) continue;
                
                int idx;
                if (!int.TryParse(strIndex, out idx)) continue;
                if ( idx < 0 || idx >= _muscleOSCParamDef.Length ) continue;
                _muscleOSCParam[idx] = strAddress;
                countExParam++;
                Debug.Log("Added: " + idx + "/" + strAddress);
            }
            _customParams = countExParam;
            return countExParam;
        }

        public void EnterExpertMode()
        {
            if ( _customParams > 0 && _expertUI != null ) {
                SetClientTogglesOff();

                _toggleRightArm.gameObject.SetActive(false);
                _toggleLeftArm.gameObject.SetActive(false);

                _expertUI.SetActive(true);

                _toggleExpertMode = GameObject.Find("ToggleExpertMode").GetComponent<Toggle>();
                _textExpertMode = GameObject.Find("TextExpertMode").GetComponent<Text>();
                _textExpertMode.text = _customParams.ToString() + " ExParams defined.";
                _buttonModeNormal= GameObject.Find("ButtonModeNormal").GetComponent<Button>();
                _topText.text = "Expert mode (experimental)";
            }
        }

        public void ExitExpertMode()
        {
            ResetCustExParamArray();
            SetClientTogglesOff();
            _toggleExpertMode.isOn = false;
            _expertUI.SetActive(false);

            _toggleRightArm.gameObject.SetActive(true);
            _toggleLeftArm.gameObject.SetActive(true);
            _topText.text = "Normal mode";
        }

        bool IsValidIpAddr( string ipString ) {
            IPAddress address;
            return (IPAddress.TryParse(ipString, out address));
        }

        int GetValidPortFromStr( string portstr )
        {
            int port;
            if (!int.TryParse(portstr, out port)) {
                return -1;
            }
            if (port <= 0 || port > 65535 ) {
                return -1;
            }
            return port;
        }

        public void SetClientTogglesOff()
        {
            _startRightArm = false;
            _startLeftArm = false;
            _startExpertMode = false;

            if ( _toggleLeftArm != null ) _toggleLeftArm.isOn = false;
            if ( _toggleRightArm != null ) _toggleRightArm.isOn = false;
			if ( _toggleExpertMode != null ) _toggleExpertMode.isOn = false;
        }

        public void OnServerCutEyeToggleChanged(Boolean value) 
        {
            bool isOn = _toggleServerCutEye.isOn;
            var exr = GameObject.Find("ExternalReceiver").GetComponent<EVMC4U.ExternalReceiver>();
            if ( exr == null ) return;

            exr.CutBoneNeck = false;
            exr.CutBoneHead = false;
            exr.CutBoneLeftEye = true;
            exr.CutBoneRightEye = true;
            exr.CutBoneJaw = false;

            exr.CutBoneHips = false;
            exr.CutBoneSpine = false;
            exr.CutBoneChest = false;
            exr.CutBoneUpperChest = false;

            exr.CutBoneLeftUpperLeg = false;
            exr.CutBoneLeftLowerLeg = false;
            exr.CutBoneLeftFoot = false;
            exr.CutBoneLeftToes = false;

            exr.CutBoneRightUpperLeg = false;
            exr.CutBoneRightLowerLeg = false;
            exr.CutBoneRightFoot = false;
            exr.CutBoneRightToes = false;

            exr.CutBonesEnable = isOn;                
        }

        public void OnServerToggleChanged(Boolean value) 
        {
            var uoscServer =  GameObject.Find("ExternalReceiver").GetComponent<uOscServer>();
            if ( _toggleServer.isOn == false ) {
                uoscServer.StopServer();
                _topText.text = "OSC server stopped.";
            }
            else 
            {
                int port = GetValidPortFromStr(_inputFieldListenPort.text);
                if ( port > 0 ) 
                {            
                    uoscServer.port = port;
                    uoscServer.StartServer();
                    if ( _topText != null ) _topText.text = "OSC Server started.";

                }
                else {
                    _toggleServer.isOn = false;
                    _topText.text = "Invalid server port.";
                }
            }
        }

        bool InitClient()
        {
            if ( _animationTarget == null ) 
            {
                SetClientTogglesOff();
                return false;
            }

            if (_handler == null ) {
                _handler = new HumanPoseHandler( _animationTarget.avatar, _animationTarget.transform);
                if ( _handler == null ) {
                    _topText.text = "HumanPoseHandler preparation failed.";
                    return false;
                }
            }
            uOscClient client = GetComponent<uOscClient>();  
            if (client == null ) {
                _topText.text = "OSC Client preparation failed.";
                return false;
            }

            int destport = GetValidPortFromStr(_inputFieldDestPort.text);
            if (destport != -1 ) 
            {
                client.port = destport;
            }
            else 
            {
                _topText.text = "Invalid Dest Port.";
                return false;
            }
            if ( IsValidIpAddr(_inputFieldIP.text)) {
                client.address = _inputFieldIP.text;
            }
            else {
                _topText.text = "Invalid IP Address";
                return false;
            }
            _topText.text = "Client started.";
            return true;
        }

        public void OnToggleChangedExpertMode(bool value)
        {
            if ( _startExpertMode == true && _toggleExpertMode.isOn == false ) 
            {
                _topText.text = "Client stopped.";
            } 
            else if (  _startExpertMode == false && _toggleExpertMode.isOn == true ) 
            {
                if (InitClient() == false ) 
                {
                    _toggleExpertMode.isOn = false;
                    return;
                }
            }
            _startExpertMode = _toggleExpertMode.isOn;
        }

        public void OnToggleChanged(bool value)
        {
            // start -> stop 
            if ( (_startRightArm == true || _startLeftArm == true  ) && _toggleLeftArm.isOn == false && _toggleRightArm.isOn ==false ) 
            {
                _topText.text = "Client stopped.";
                _startLeftArm = false;
                _startRightArm = false;
                return;

            }

            _startLeftArm = _toggleLeftArm.isOn;
            _startRightArm = _toggleRightArm.isOn;
            if ( _startRightArm == false && _startLeftArm == false ) {
                return;
            }

            if (InitClient() == false ) 
            {
                SetClientTogglesOff();
                return;
            }
        }


        // Update is called once per frame
        void Update()
        {
            if ( _customParams > 0 && _expertUI != null && _expertUI.activeInHierarchy == false ) 
            {
                EnterExpertMode();
            }

            if ( _startRightArm == false && _startLeftArm == false && _startExpertMode == false ) 
            {
                if ( _handler != null ) _handler = null;
                return;
            }

            uOscClient client = GetComponent<uOscClient>();  
            if ( _handler != null && client != null ) {
                _handler.GetHumanPose(ref _targetHumanPose);   
                int musclesLen = _targetHumanPose.muscles.Length; // 一応上限見ておく
                if (_startExpertMode == true ) 
                {
                    for (int i = 0; i < _muscleOSCParam.Length && i < musclesLen; i++) { // 使う範囲かつ上限以内
                        if ( String.IsNullOrEmpty( _muscleOSCParam[i] ) == false ) 
                        {
                            float mus = _targetHumanPose.muscles[i];
                            client.Send(_muscleOSCParam[i], mus);
                        }
                    }
                }
                else // Normal Mode
                {
                    if ( _startLeftArm == true )
                    {
                        // 37, 38 ... LeftShoulder
                        for (int i = 39; i <= 43 && i < musclesLen && i < _muscleOSCParamDef.Length; i++) { // 使う範囲かつ上限以内
                            float mus = _targetHumanPose.muscles[i];
                            client.Send(_muscleOSCParamDef[i], mus);
                        }
                        // 44,45 LeftHand( Wrist )
                    }
                    if ( _startRightArm == true ) {
                        // 46, 47 ... RightShoulder
                        for (int i = 48; i <= 52 && i < musclesLen && i < _muscleOSCParamDef.Length; i++) { // 使う範囲かつ上限以内
                            float mus = _targetHumanPose.muscles[i];
                            client.Send(_muscleOSCParamDef[i], mus);
                        }
                        // 53,54 RightHand( Wrist )
                    }
                }
            }
        }
    }
}

// Copyright (c) 2023-2024 Sakura(さくら) / tbbsakura
// MIT License. See "LICENSE" file.

#define TMARELAY_KANTANPACK 
#define FINGER_MODE_4BIT
//#define FINGER_MODE_2BIT

using System.Net;
using UnityEngine;
using uOSC;
using System.IO;
using System;
using SFB;
using EVMC4U;

using SakuraScript.Utils;

#if TMARELAY_KANTANPACK
using HardCoded.VRigUnity;
#endif

namespace UnityEngine.UI
{
    [System.Serializable]
    public class TMARelaySetting {
        public string _defaultVRM;
    };

    [AddComponentMenu("OSC/SakuraVRMMuscle2OSC", 34)]
    public class SakuraVRMMuscle2OSC : MonoBehaviour
    {
        private static readonly String[] _muscle2bitOSCParam = {
            "/avatar/parameters/TMARelay_LowerArmLstRst_2x4",
            "/avatar/parameters/TMARelay_UpperArmLdufbRdufb_2x4",
            "/avatar/parameters/TMARelay_UpperArmTwPinky_LR_2x4",
            "/avatar/parameters/TMARelay_RingMiddle_LR_2x4",
            "/avatar/parameters/TMARelay_IndexThumb_LR_2x4",
            "/avatar/parameters/TMARelay_HandUdIo_LR_2x4",
            "/avatar/parameters/TMARelay_ShoulderLdufbRdufb_2x4",
        };

        private static readonly String[] _muscle4bitOSCParam = {
            "/avatar/parameters/LowerArmLR_Stretch",
            "/avatar/parameters/LowerArmLR_Twist",
            "/avatar/parameters/UpperArmLR_DownUp",
            "/avatar/parameters/UpperArmLR_FrontBack",
            "/avatar/parameters/UpperArmLR_Twist",

            "/avatar/parameters/TMAR_LR_THUMB",
            "/avatar/parameters/TMAR_LR_INDEX",
            "/avatar/parameters/TMAR_LR_MIDDLE",
            "/avatar/parameters/TMAR_LR_RING",
            "/avatar/parameters/TMAR_LR_LITTLE",

            "/avatar/parameters/HandLR_InOut",
            "/avatar/parameters/HandLR_DownUp",
         };

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

        [Tooltip("アバター、IPアドレス、ポートを変更する時はStartのチェックを全てオフにしてください")]
        [SerializeField] private bool    _startRightArm = false;
        [Tooltip("アバター、IPアドレス、ポートを変更する時はStartのチェックを全てオフにしてください")]
        [SerializeField] private bool    _startLeftArm = false;
#if FINGER_MODE_4BIT || FINGER_MODE_2BIT
        [Tooltip("アバター、IPアドレス、ポートを変更する時はStartのチェックを全てオフにしてください")]
        [SerializeField] private bool    _startFinger48 = false;
#endif
        [Tooltip("ExpertMode用")]
        [SerializeField] private bool    _startExpertMode = false;

        [SerializeField, Tooltip("VRMアバターを入れます。変更時はStartのチェックを2つともオフにしてください")]
        Animator _animationTarget;

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
        private Toggle _toggleFinger48Mode = null;
        private Text _textExpertMode;
        private Button _buttonModeNormal;
        private GameObject _expertUI;
        private Text _topText;

        TMARelay_FileDragAndDrop _loader; 
        TMARelaySetting _setting = new TMARelaySetting();

#if TMARELAY_KANTANPACK
        private HardCoded.VRigUnity.CameraButton _vruCameraButton;
#endif
        // Start is called before the first frame update
        void Start()
        {
            var server =  GameObject.Find("ExternalReceiver").GetComponent<uOscServer>();
            server.port = _portListen;
            _loader = GetComponent<TMARelay_FileDragAndDrop>();

            _inputFieldIP = GameObject.Find("InputField_IP").GetComponent<InputField>();
            _inputFieldIP.text = _ipAddress;
            _inputFieldDestPort = GameObject.Find("InputField_Port").GetComponent<InputField>();
            _inputFieldDestPort.text = _portDestination.ToString();

#if !TMARELAY_KANTANPACK
            _inputFieldListenPort = GameObject.Find("InputField_ListenPort").GetComponent<InputField>();
            _inputFieldListenPort.text = _portListen.ToString();

            _toggleServer = GameObject.Find("ToggleServer").GetComponent<Toggle>();
            _toggleServer.isOn = true; // ExternalReceiverが Start してしまうので

            _toggleServerCutEye = GameObject.Find("ToggleServerCutEye").GetComponent<Toggle>();
            _toggleServerCutEye.isOn = true; 

            _toggleExpertMode = GameObject.Find("ToggleExpertMode").GetComponent<Toggle>();
            _textExpertMode = GameObject.Find("TextExpertMode").GetComponent<Text>();
            _buttonModeNormal= GameObject.Find("ButtonModeNormal").GetComponent<Button>();

            _expertUI = GameObject.Find("ExpertUI");
            _expertUI.SetActive(false);
#endif
            _toggleLeftArm = GameObject.Find("ToggleLeft").GetComponent<Toggle>();
            _toggleLeftArm.isOn = _startLeftArm;
            _toggleRightArm = GameObject.Find("ToggleRight").GetComponent<Toggle>();
            _toggleRightArm.isOn = _startRightArm;
            _toggleFinger48Mode = GameObject.Find("ToggleFinger48").GetComponent<Toggle>();
            _toggleFinger48Mode.isOn = _startFinger48;

            _topText = GameObject.Find("TopText").GetComponent<Text>();

            // 設定ファイル
            string pathSetting = GetMainSettingFilePath();
            if (System.IO.File.Exists(pathSetting) ) {
                SakuraSetting<TMARelaySetting> loader = new SakuraSetting<TMARelaySetting>();
                if ( loader.LoadFromFile(pathSetting) ) _setting = loader.Data;
            }

#if TMARELAY_KANTANPACK
            var cbgo = GameObject.Find("Camera Button");
             _vruCameraButton = cbgo.GetComponent<HardCoded.VRigUnity.CameraButton>();
            _topText.text = "設定ボタンの中で使用するカメラを指定してから\r\nカメラ開始を押してください";
#endif
#if !TMARELAY_KANTANPACK
            // デフォルトモデルのロード
            Debug.Log($"Loading {DefaultVRMPath}");
            _loader.OpenVRM(DefaultVRMPath);
#endif
        }

        private void OnDestroy() {
            SakuraSetting<TMARelaySetting> saver = new SakuraSetting<TMARelaySetting>();
            saver.Data = _setting;
            saver.SaveToFile(GetMainSettingFilePath());
        }

        string GetMainSettingFilePath()
        {
            string path = GetJsonDirectory();
            return  path + "\\TMARelay.setting.json";
        }

        string GetJsonDirectory()
        {
#if UNITY_EDITOR
            string path = "Assets\\SakuraShop_tbb\\TMARelay\\setting";
#else
            string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');//EXEを実行したカレントディレクトリ (ショートカット等でカレントディレクトリが変わるのでこの方式で)
#endif
            return path;
        }

        public void OnVRMLoadButton()
        {
            var extensions = new[] { new ExtensionFilter("VRM Files", "vrm" ), };
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (paths.Length > 0 && paths[0].Length > 0) {
                _loader.OpenVRM(paths[0]);
            }
        }

        private string DefaultVRMPath {
            get {
                if (_setting._defaultVRM.Length > 0 && System.IO.File.Exists(_setting._defaultVRM)){
                    return _setting._defaultVRM;
                }
#if UNITY_EDITOR
                const char separatorChar = '/';
                string modelFilepath = "Assets/SakuraShop_tbb/VRM_CC0/HairSample_Male.vrm"; //CC0 model
                modelFilepath = modelFilepath.Replace( separatorChar, System.IO.Path.DirectorySeparatorChar );
#else
                string modelFilepath = "HairSample_Male.vrm"; //CC0 model
#endif
                if ( System.IO.File.Exists(modelFilepath) ) return modelFilepath;
                return "";
            }
        }

        // VRMファイル読み込み後の処理
        public void OnVRMLoaded(Animator animator)
        {
            // animationtarget, HumanPoseHandler 変数更新
            _animationTarget = animator;
            _setting._defaultVRM = _loader.LastLoadedFile; // 最後に読めたファイルを次回読むファイルにする
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
                _toggleFinger48Mode.gameObject.SetActive(false);

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
            _toggleFinger48Mode.gameObject.SetActive(true);
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
            _startFinger48 = false;
            _startExpertMode = false;

            if ( _toggleLeftArm != null ) _toggleLeftArm.isOn = false;
            if ( _toggleRightArm != null ) _toggleRightArm.isOn = false;
            if ( _toggleFinger48Mode != null ) _toggleFinger48Mode.isOn = false;
			if ( _toggleExpertMode != null ) _toggleExpertMode.isOn = false;
        }

#if !TMARELAY_KANTANPACK
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
#endif
        bool InitClient()
        {
            if ( _animationTarget == null ) 
            {
                SetClientTogglesOff();
                Debug.Log("InitClient() failed: _animationTarget == null");
				var goer = GameObject.Find("/ExternalReceiver");
				if ( goer == null ) return false;
                var exrec = goer.GetComponent<ExternalReceiver>();
				if ( exrec == null ) return false;
                if ( exrec.Model == null ) return false;
                var ani = exrec.Model.GetComponent<Animator>();
                if ( ani == null ) return false;
                _animationTarget = ani;
            }

            if (_handler == null ) {
                _handler = new HumanPoseHandler( _animationTarget.avatar, _animationTarget.transform);
                if ( _handler == null ) {
                    _topText.text = "HumanPoseHandler初期化失敗.";
                    Debug.Log(_topText.text);
                    return false;
                }
            }
            uOscClient client = GetComponent<uOscClient>();  
            if (client == null ) {
                _topText.text = "OSCクライアント初期化失敗";
                Debug.Log(_topText.text);
                return false;
            }

            int destport = GetValidPortFromStr(_inputFieldDestPort.text);
            if (destport != -1 ) 
            {
                client.port = destport;
            }
            else 
            {
                _topText.text = "ポートが不正です";
                Debug.Log(_topText.text);
                return false;
            }
            if ( IsValidIpAddr(_inputFieldIP.text)) {
                client.address = _inputFieldIP.text;
                Debug.Log(_topText.text);
            }
            else {
                _topText.text = "IPアドレスが不正です";
                Debug.Log(_topText.text);
                return false;
            }
#if TMARELAY_KANTANPACK
            _topText.text = "VRChatに情報送信中\r\n左右逆の場合は、設定-カメラ-水平反転";
#else
            _topText.text = "VRChatに情報送信中";
#endif
            return true;
        }

        public void OnToggleChangedExpertMode(bool value)
        {
            if ( _startExpertMode == true && _toggleExpertMode.isOn == false ) 
            {
                _topText.text = "---";
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
            Debug.Log($"OnToggleChanged {value}");
            if ( _toggleFinger48Mode.isOn == false ) {
                _toggleLeftArm.gameObject.SetActive(true);
                _toggleRightArm.gameObject.SetActive(true);
            }
            else {
                _startLeftArm = _toggleLeftArm.isOn = false;
                _startRightArm = _toggleRightArm.isOn = false;
                _toggleLeftArm.gameObject.SetActive(false);
                _toggleRightArm.gameObject.SetActive(false);
            }
            if ( (_startRightArm || _startLeftArm  || _startFinger48 ) && _toggleLeftArm.isOn == false && _toggleRightArm.isOn ==false && _toggleFinger48Mode.isOn ==false )
            {
                _topText.text = "送信停止";
                _startLeftArm = false;
                _startRightArm = false;
                _startFinger48 = false;
                return;

            }

            _startLeftArm = _toggleLeftArm.isOn;
            _startRightArm = _toggleRightArm.isOn;
            _startFinger48 = _toggleFinger48Mode.isOn;
            if ( _startRightArm == false && _startLeftArm == false && _startFinger48 == false ) {
                return;
            }

            if (InitClient() == false ) 
            {
                SetClientTogglesOff();
                return;
            }
        }

        int EncodeFloatTo2BitInt( float float1 ) {
            if ( float1 > 0.5f ) return 3;
            else if ( float1 >= 0.0 && float1 < 0.5 ) return 2;
            else if ( float1 >= -0.5 && float1 < 0.0 ) return 1;
            return 0; // if ( float1 < -0.5 ) return 0;
        }

        int EncodeFloatTo3BitInt( float float1 ) {
            if ( float1 >= 0.75 ) return 7;
            else if ( float1 >= 0.5 && float1 < 0.75 ) return 6;
            else if ( float1 >= 0.25 && float1 < 0.5 ) return 5;
            else if ( float1 >= 0.0 && float1 < 0.25 ) return 4;
            else if ( float1 >= -0.25 && float1 < 0.0 ) return 3;
            else if ( float1 >= -0.5 && float1 < 0.25 ) return 2;
            else if ( float1 >= -0.75 && float1 < -0.5 ) return 1;
            return 0; // if ( float1 < -0.75 ) return 0;
        }


        int EncodeFloatTo4BitInt( float float1 ) {
            if (float1 >= 1.0 ) return 15;
            else if ( float1 >= 0.866667 ) return 14;
            else if ( float1 >= 0.733333 ) return 13;
            else if ( float1 >= 0.6 ) return 12;
            else if ( float1 >= 0.466667 ) return 11;
            else if ( float1 >= 0.33333 ) return 10;
            else if ( float1 >= 0.2 ) return 9;
            else if ( float1 >= 0.066667 ) return 8;
            else if ( float1 >= -0.066667 ) return 7;
            else if ( float1 >= -0.2 ) return 6;
            else if ( float1 >= -0.33333 ) return 5;
            else if ( float1 >= -0.466667 ) return 4;
            else if ( float1 >= -0.6 ) return 3;
            else if ( float1 >= -0.733333 ) return 2;
            else if ( float1 >= -0.866667 ) return 1;
            return 0; 
        }

        int Encode4FloatsToInt( float float1, float float2, float float3, float float4 ) {
            return  (EncodeFloatTo2BitInt(float1) << 6) + 
                        (EncodeFloatTo2BitInt(float2) << 4) + 
                        (EncodeFloatTo2BitInt(float3) << 2) + 
                        EncodeFloatTo2BitInt(float4);
        }

        int Encode2FloatsToInt( float float1, float float2 ) {
            return  (EncodeFloatTo4BitInt(float1) << 4) + EncodeFloatTo4BitInt(float2);
        }

        // Update is called once per frame
        void Update()
        {
#if TMARELAY_KANTANPACK
            bool vruEnabled = _vruCameraButton.IsCameraShowing;
            if ( vruEnabled ) { // Finger Mode専用
                if (_toggleFinger48Mode.isOn == false ) {
                    _toggleFinger48Mode.isOn = true;
                    _startFinger48 = true;
                    if (InitClient() == false ) 
                    {
                        SetClientTogglesOff();
                    }
                }
                /*
                if ( !_toggleLeftArm.isOn || ! _toggleRightArm.isOn ) {
                    _toggleLeftArm.isOn = true;
                    _toggleRightArm.isOn = true;
                    _startLeftArm = true;
                    _startRightArm = true;
                    if (InitClient() == false ) 
                    {
                        SetClientTogglesOff();
                    }
                } */               
            }
            else {
                if ( _toggleLeftArm.isOn || _toggleRightArm.isOn || _toggleFinger48Mode.isOn ) {
                    _toggleLeftArm.isOn = false;
                    _toggleRightArm.isOn = false;
                    _startLeftArm = false;
                    _startRightArm = false;
                    _toggleFinger48Mode.isOn = false;
                    _startFinger48 = false;
                    SetClientTogglesOff();
                    _topText.text = "送信停止";
                }
            }
#endif
            if ( _customParams > 0 && _expertUI != null && _expertUI.activeInHierarchy == false ) 
            {
                EnterExpertMode();
            }

            if ( _startRightArm == false && _startLeftArm == false && _startExpertMode == false && _startFinger48 == false) 
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
//#if FINGER_MODE_4BIT || FINGER_MODE_2BIT
                else if (_startFinger48==true)
                {
                    int int1 = 0;
#if FINGER_MODE_4BIT  // 4bit
                    //Debug.Log("FINGER_MODE_4BIT");
                    int1 = Encode2FloatsToInt(
                        _targetHumanPose.muscles[42],  // TMARelay_LowerArmL_Stretch
                        _targetHumanPose.muscles[51]); // TMARelay_LowerArmR_Stretch 
                    client.Send(_muscle4bitOSCParam[0], int1 ); // TMARelay_LowerArmLR_Stretch

                    int1 = Encode2FloatsToInt(
                        _targetHumanPose.muscles[43], // TMARelay_LowerArmL_Twist
                        _targetHumanPose.muscles[52]);// TMARelay_LowerArmR_Twist
                    client.Send(_muscle4bitOSCParam[1], int1 ); // TMARelay_LowerArmLR_Twist

                    int1 = Encode2FloatsToInt(
                        _targetHumanPose.muscles[39],// Upper Left Down Up 
                        _targetHumanPose.muscles[48]); // Upper Right Down Up 
                    client.Send(_muscle4bitOSCParam[2], int1 ); //  TMARelay_UpperArmLR_DownUp

                    int1 = Encode2FloatsToInt(
                        _targetHumanPose.muscles[40], // Upper Left FrontBack
                        _targetHumanPose.muscles[49]);// Upper Right FrontBack
                    client.Send(_muscle4bitOSCParam[3], int1 );// "/avatar/parameters/TMARelay_UpperArmLR_FrontBack",

                    int1 = Encode2FloatsToInt(
                        _targetHumanPose.muscles[41],// Upper Left Twist
                        _targetHumanPose.muscles[50]); // Upper Right Twist
                    client.Send(_muscle4bitOSCParam[4], int1 );// "/avatar/parameters/TMARelay_UpperArmLR_Twist",

                    int1 = Encode2FloatsToInt(
                        (_targetHumanPose.muscles[55]+_targetHumanPose.muscles[57]+_targetHumanPose.muscles[58])/3.0f, // Left Thumb
                        (_targetHumanPose.muscles[75]+_targetHumanPose.muscles[77]+_targetHumanPose.muscles[78])/3.0f); // Right thumb
                    client.Send(_muscle4bitOSCParam[5], int1 );// "/avatar/parameters/TMAR_LR_THUMB",

                    int1 = Encode2FloatsToInt(
                        (_targetHumanPose.muscles[59]+_targetHumanPose.muscles[61]+_targetHumanPose.muscles[62])/3.0f, // Left Index
                        (_targetHumanPose.muscles[79]+_targetHumanPose.muscles[81]+_targetHumanPose.muscles[82])/3.0f);  // Right index
                    client.Send(_muscle4bitOSCParam[6], int1 );// "/avatar/parameters/TMAR_LR_INDEX",

                    int1 = Encode2FloatsToInt(
                        (_targetHumanPose.muscles[63]+_targetHumanPose.muscles[65]+_targetHumanPose.muscles[66])/3.0f, // Left Middle
                        (_targetHumanPose.muscles[83]+_targetHumanPose.muscles[85]+_targetHumanPose.muscles[86])/3.0f); // Right Middle
                    client.Send(_muscle4bitOSCParam[7], int1 );// "/avatar/parameters/TMAR_LR_MIDDLE",

                    int1 = Encode2FloatsToInt(
                        (_targetHumanPose.muscles[67]+_targetHumanPose.muscles[69]+_targetHumanPose.muscles[70])/3.0f, // Left Ring
                        (_targetHumanPose.muscles[87]+_targetHumanPose.muscles[89]+_targetHumanPose.muscles[90])/3.0f); // Right Ring
                    client.Send(_muscle4bitOSCParam[8], int1 );// "/avatar/parameters/TMAR_LR_RING",
                    int1 = Encode2FloatsToInt(
                        (_targetHumanPose.muscles[71]+_targetHumanPose.muscles[73]+_targetHumanPose.muscles[74])/3.0f, // Left Pinky
                        (_targetHumanPose.muscles[91]+_targetHumanPose.muscles[93]+_targetHumanPose.muscles[94])/3.0f); // Right Pinky
                    client.Send(_muscle4bitOSCParam[9], int1 );// "/avatar/parameters/TMAR_LR_LITTLE",

                    int1 = Encode2FloatsToInt(
                        _targetHumanPose.muscles[45],// Left Hand InOut
                        _targetHumanPose.muscles[54]); // Right Hand InOut
                    client.Send(_muscle4bitOSCParam[10], int1 );//  "/avatar/parameters/HandLR_InOut",
                    int1 = Encode2FloatsToInt(
                        _targetHumanPose.muscles[44],// Left Hand DownUp
                        _targetHumanPose.muscles[53]); // Right Hand DownUp
                    client.Send(_muscle4bitOSCParam[11], int1 );//  "/avatar/parameters/HandLR_DownUp",

#else
//if FINGER_MODE_2BIT
                    Debug.Log("FINGER_MODE_2BIT");
                    int1 = Encode4FloatsToInt(
                        _targetHumanPose.muscles[42],// TMARelay_LowerArmL_Stretch
                        _targetHumanPose.muscles[43], // TMARelay_LowerArmL_Twist
                        _targetHumanPose.muscles[51], // TMARelay_LowerArmR_Stretch
                        _targetHumanPose.muscles[52]);// TMARelay_LowerArmR_Twist
                    client.Send(_muscle2bitOSCParam[0], int1 ); // "/avatar/parameters/TMARelay_LowerArmLstRst_2x4",

                    int1 = Encode4FloatsToInt(
                        _targetHumanPose.muscles[39],// Upper Left Down Up 
                        _targetHumanPose.muscles[40], // Upper Left FrontBack
                        _targetHumanPose.muscles[48], // Upper Right Down Up 
                        _targetHumanPose.muscles[49]);// Upper Right FrontBack
                    client.Send(_muscle2bitOSCParam[1], int1 );// "/avatar/parameters/TMARelay_UpperArmLdufbRdufb_2x4",

                    int1 = Encode4FloatsToInt(_targetHumanPose.muscles[41],// Upper Left Twist
                        (_targetHumanPose.muscles[71]+_targetHumanPose.muscles[73]+_targetHumanPose.muscles[74])/3.0f, // Left Pinky
                        _targetHumanPose.muscles[51], // Upper Right Twist
                        (_targetHumanPose.muscles[91]+_targetHumanPose.muscles[93]+_targetHumanPose.muscles[94])/3.0f); // // Right Pinky
                    client.Send(_muscle2bitOSCParam[2], int1 ); // "/avatar/parameters/TMARelay_UpperArmTwPinky_LR_2x4",

                    int1 = Encode4FloatsToInt(
                        (_targetHumanPose.muscles[67]+_targetHumanPose.muscles[69]+_targetHumanPose.muscles[70])/3.0f, // Left Ring
                        (_targetHumanPose.muscles[63]+_targetHumanPose.muscles[65]+_targetHumanPose.muscles[66])/3.0f, // Left Middle
                        (_targetHumanPose.muscles[87]+_targetHumanPose.muscles[89]+_targetHumanPose.muscles[90])/3.0f, // Right Ring
                        (_targetHumanPose.muscles[83]+_targetHumanPose.muscles[85]+_targetHumanPose.muscles[86])/3.0f); // // Right Middle
                    client.Send(_muscle2bitOSCParam[3], int1 ); // "/avatar/parameters/TMARelay_RingMiddle_LR_2x4",

                    int1 = Encode4FloatsToInt(
                        (_targetHumanPose.muscles[59]+_targetHumanPose.muscles[61]+_targetHumanPose.muscles[62])/3.0f, // Left Index
                        (_targetHumanPose.muscles[55]+_targetHumanPose.muscles[57]+_targetHumanPose.muscles[58])/3.0f, // Left Thumb
                        (_targetHumanPose.muscles[79]+_targetHumanPose.muscles[81]+_targetHumanPose.muscles[82])/3.0f, // Right index
                        (_targetHumanPose.muscles[75]+_targetHumanPose.muscles[77]+_targetHumanPose.muscles[78])/3.0f); // // Right thumb
                    client.Send(_muscle2bitOSCParam[4], int1 ); // "/avatar/parameters/TMARelay_IndexThumb_LR_2x4",

                    int1 = Encode4FloatsToInt(_targetHumanPose.muscles[44],// Hand Left Up Down
                        _targetHumanPose.muscles[45], // Hand Left In Out
                        _targetHumanPose.muscles[53], // Hand Right Up Down
                        _targetHumanPose.muscles[54]);// Hand Right In Out
                    client.Send(_muscle2bitOSCParam[5], int1 );// "/avatar/parameters/TMARelay_HandUdIo_LR_2x4",

                    int1 = Encode4FloatsToInt(_targetHumanPose.muscles[37],// Shoulder Left Down up
                        _targetHumanPose.muscles[38], // Shoulder Left front back
                        _targetHumanPose.muscles[46], // Shoulder Right Down up
                        _targetHumanPose.muscles[47]);// Shoulder Right front back
                    client.Send(_muscle2bitOSCParam[6], int1 );// "/avatar/parameters/TMARelay_ShoulderLdufbRdufb_2x4",
#endif
                }
//#endif
                else // Normal Mode
                {
                    if ( _startLeftArm == true )
                    {
                        // 37, 38 ... LeftShoulder
                        for (int i = 39; i <= 43 && i < musclesLen && i < _muscleOSCParamDef.Length; i++) { // 使う範囲かつ上限以内
                            float mus = _targetHumanPose.muscles[i];
                            client.Send(_muscleOSCParamDef[i], mus);
                        }
                        // finger test
                        for (int i = 55; i <= 74 && i < musclesLen && i < _muscleOSCParamDef.Length; i++) { // 使う範囲かつ上限以内
                            float mus = _targetHumanPose.muscles[i];
                            client.Send(_muscleOSCParamDef[i], mus);
                        }
                    }
                    if ( _startRightArm == true ) {
                        // 46, 47 ... RightShoulder
                        for (int i = 48; i <= 52 && i < musclesLen && i < _muscleOSCParamDef.Length; i++) { // 使う範囲かつ上限以内
                            float mus = _targetHumanPose.muscles[i];
                            client.Send(_muscleOSCParamDef[i], mus);
                        }
                        // finger test
                        for (int i = 75; i <= 94 && i < musclesLen && i < _muscleOSCParamDef.Length; i++) { // 使う範囲かつ上限以内
                            float mus = _targetHumanPose.muscles[i];
                            client.Send(_muscleOSCParamDef[i], mus);
                        }
                    }
                }
            }
        }
    }
}

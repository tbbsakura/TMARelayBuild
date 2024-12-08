using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using B83.Win32;

using System.IO;
using System.Threading.Tasks;
using VRM;

using UniGLTF;
using UniHumanoid;
using VRMShaders;
using System.Security.Cryptography;

public class TMARelay_FileDragAndDrop : MonoBehaviour
{
    private bool m_loadAsync = true;
    private bool m_loading = false;
    private Text m_topText;

    private string m_lastLoadedFile;
    public string LastLoadedFile => m_lastLoadedFile;

    EVMC4U.ExternalReceiver m_exrec;
    RuntimeGltfInstance _lastLoaded = null;
    SakuraVRMMuscle2OSC _muscle2OSC;

    private void Awake() {
        m_exrec = GameObject.Find("ExternalReceiver").GetComponent<EVMC4U.ExternalReceiver>();
        _muscle2OSC = GetComponent<SakuraVRMMuscle2OSC>();
    }

    void OnEnable ()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
        m_topText = GameObject.Find("TopText").GetComponent<Text>();
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    public void OpenVRM(string path)
    {
        if ( m_loading ) return; // ignore
        LoadModel(path);
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        if ( m_loading ) return; // ignore
        LoadModel(aFiles[0].ToString()); // load only 1st file
    }
    
    void OnLoaded(RuntimeGltfInstance loaded, string path)
    {
        if ( loaded == null || loaded.Root == null )  return;
        if ( _lastLoaded != null ) Destroy(_lastLoaded.Root);
        _lastLoaded = loaded;
        loaded.Root.transform.SetParent(transform, false);// このcomponentがattachされているオブジェクトを親に設定する
        loaded.Root.transform.position.Set(-1,1,3); // 位置調整
        m_topText.text = "Showing Meshes...";
        foreach (var spring in loaded.Root.GetComponentsInChildren<VRMSpringBone>())
        {
            spring.Setup();
            spring.gameObject.SetActive(false);
        }

        loaded.ShowMeshes(); // メッシュ表示
        var lah = loaded.GetComponent<VRMLookAtHead>();
        if (lah !=null)
        {
            lah.UpdateType = UpdateType.LateUpdate;
        }

        UniHumanoid.HumanPoseTransfer target = loaded.Root.AddComponent<UniHumanoid.HumanPoseTransfer>();
        if ( target != null ) 
        {
            Animator animator = target.GetComponent<Animator>();
            if (animator != null)
            {
                if ( m_exrec != null && target != null ) m_exrec.Model = loaded.Root;
                m_lastLoadedFile = path; // load と show が成功してから更新する
                _muscle2OSC?.OnVRMLoaded(animator);
            }
        }
        m_topText.text = "VRM loaded";
    }

    private async void LoadModel(string path)
    {
        var muscle2OSC = GameObject.Find("Muscle2OSC").GetComponent<SakuraVRMMuscle2OSC>();
        var ext = Path.GetExtension(path).ToLower();
        if ( ext == ".vrm" ) 
        {
            if ( m_topText != null ) m_topText.text = "Loading VRM...";
            using ( var data = new GlbFileParser(path).Parse() ) 
            {
                muscle2OSC.SetClientTogglesOff();
                var vrm = new VRMData(data);
                using ( var context = new VRMImporterContext(vrm) ) 
                {
                    var loaded = default(RuntimeGltfInstance);
                    m_loading = true;
                    if (m_loadAsync)
                    {
                        try {
                            loaded = await context.LoadAsync(new VRMShaders.RuntimeOnlyAwaitCaller());
                        } catch(System.Exception e) {
                            System.Type exceptionType = e.GetType();
                            m_topText.text = "Load failed. : " + e.Message;
                            m_loading = false;
                            return;
                        }
                    }
                    else
                    {
                        try {
                            loaded = context.Load();
                        } catch(System.AggregateException ae1) { // 2層Aggregateが帰ってきたりするので
                            foreach ( var eInner in ae1.InnerExceptions ) 
                            {
                                if (eInner.GetType() == typeof(System.AggregateException) ) 
                                {
                                    System.AggregateException ae2 = (System.AggregateException)eInner;
                                    foreach ( var eInner2 in ae2.InnerExceptions ) 
                                    {
                                        m_topText.text = "Load failed. : " + eInner2.Message;
                                        m_loading = false;
                                        return;
                                    }
                                }
                                else {
                                    m_topText.text = "Load failed. : " + eInner.Message;
                                    m_loading = false;
                                    return;
                                }
                            }
                            m_loading = false;
                            return;
                        } catch (System.Exception e) {
                            m_topText.text = "Load failed. : " + e.Message;
                            m_loading = false;
                            return;
                        }
                    }
                    OnLoaded(loaded, path);
                }
            }
        }
        else if ( ext == ".txt" ) // ExParam definition file?
        {
            m_topText.text = "Loading TXT...";
            if ( muscle2OSC.ReadCustomExParamList(path) > 0 )
            {
                muscle2OSC.EnterExpertMode();
            }
        }
        else {
            m_topText.text = "Dropped is not .vrm file.";
        }
        m_loading = false;
    }
}


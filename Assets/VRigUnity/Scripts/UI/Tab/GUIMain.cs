// Original File VRigUnity
// modified by tbbsakura
using System.IO;
using UniGLTF;
using UnityEngine;
using UnityEngine.UI;
using VRM;

// tbbsakra BEGIN
using EVMC4U;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniVRM10;
// tbbsakra END

namespace HardCoded.VRigUnity {
	/// <summary>
	/// Main script of the UI classes
	/// </summary>
	public class GUIMain : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private RectTransform windowTransform;
		[SerializeField] private TrackingResizableBox trackingBox;
		[SerializeField] private CanvasScaler[] canvasScalers;
		private CustomizableCanvas customizableCanvas;
		private OrbitalCamera orbitalCamera;
		
		public Vector3 ModelTransform { get; set; }
		public TrackingResizableBox TrackingBox => trackingBox;
		public CustomizableCanvas CustomizableCanvas => customizableCanvas;

	    SynchronizationContext synchronizationContext; // tbbsakura

		void Awake() {
			customizableCanvas = FindObjectOfType<CustomizableCanvas>();
			orbitalCamera = FindObjectOfType<OrbitalCamera>();
		}

		void Start() {
	        synchronizationContext = SynchronizationContext.Current; // tbbsakura

			// Configure scene with settings
			LoadVrmModel(Settings.ModelFile);
			LoadCustomImage(Settings.ImageFile);
			SetShowBackgroundImage(Settings.ShowCustomBackground);
			
			UpdateCanvasScale(SettingsUtil.GetUIScaleValue(Settings.GuiScale));
			Settings.GuiScaleListener += (value) => {
				UpdateCanvasScale(SettingsUtil.GetUIScaleValue(value));
			};
		}

		private void UpdateCanvasScale(float scaleFactor) {
			// Update position of UI windows
			float positionMultiplier = canvasScalers[0].scaleFactor / scaleFactor;
			foreach (Transform child in windowTransform) {
				RectTransform rect = child.GetComponent<RectTransform>();
				rect.anchoredPosition *= positionMultiplier;
			}

			foreach (CanvasScaler canvas in canvasScalers) {
				canvas.scaleFactor = scaleFactor;
			}
		}

		public void ResetModel() {
			Settings.ModelFile = "";
			SolutionUtils.GetSolution().Model.ResetVRMModel();
		}

		public void LoadVrmModel(string path) {
			Logger.Log($"Load VRM Model: '{path}'");
			if (path == "") {
				return;
			}

			if (!File.Exists(path)) {
				Logger.Log($"Failed to load vrm model '{path}' : file not found");
				return;
			}

			FileInfo fi = new(path);
			if (fi.Exists && fi.Length > 100_000_000) {
				WarningDialog.Instance.Open(Lang.Format(Lang.WarningDialogLargeModelSize, "100MB"), delegate {
					LoadVrmModelInternal(path);
				});
			} else {
				LoadVrmModelInternal(path);
			}
		}

		private void LoadVrmModelInternal(string path) {
			Logger.Log($"Load VRM Model Internal: '{path}'");
			LoadModelSakura(path);
			return;

			var data = new GlbFileParser(path).Parse();
			var vrm = new VRMData(data);
			using (var context = new VRMImporterContext(vrm)) {
				var loaded = context.Load();
				loaded.EnableUpdateWhenOffscreen();
				loaded.ShowMeshes();
				
				Settings.ModelFile = path;
				SolutionUtils.GetSolution().Model.SetVRMModel(loaded.gameObject);
			}
		}

/// <summary>
/// tbbsakur added BEGIN
/// </summary>
	    private bool m_loadAsync = true;
	    private bool m_loading = false;
		RuntimeGltfInstance _lastLoaded = null;
		GameObject _lastLoaded10 = null;
	    private string m_lastLoadedFile;
//	    public string LastLoadedFile => m_lastLoadedFile;

		ExternalReceiver GetExrec() {
			var goer = GameObject.Find("/ExternalReceiver");
			if ( goer == null ) return null;
			return goer.GetComponent<ExternalReceiver>();
		}


		void SetExrecAnimator(GameObject loaded_gameObject )
		{
			var exrec = GetExrec();
			if ( exrec != null ) {
				exrec.Model = loaded_gameObject;
			}
		}

		void DestroyLastLoaded()
		{
			if ( _lastLoaded != null ) {
				Destroy(_lastLoaded.Root);
				_lastLoaded = null;
			}
			if ( _lastLoaded10 != null) {
				// VRM 10 のunloadをここで
				Destroy(_lastLoaded10);          
				_lastLoaded10 = null;  
			}
			
		}

		void OnLoaded10(GameObject loaded, string path)
		{
			if ( loaded == null )  return;
			DestroyLastLoaded();
			_lastLoaded10 = loaded;
			loaded.transform.SetParent(transform, false);// このcomponentがattachされているオブジェクトを親に設定する
			loaded.transform.position.Set(-1,1,3); // 位置調整

			foreach (var spring in loaded.GetComponentsInChildren<VRMSpringBone>())
			{
				spring.Setup();
				spring.gameObject.SetActive(false);
			}

			var meshes = loaded.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (var m in meshes) { 
				m.updateWhenOffscreen = true;
			}

			Animator animator = loaded.GetComponent<Animator>();

			if (animator != null)
			{
				var exrec = GetExrec();
				if ( exrec != null ) exrec.Model = loaded;
				m_lastLoadedFile = path; // load と show が成功してから更新する
			}

			Settings.ModelFile = path;
			SolutionUtils.GetSolution().Model.SetVRMModel(loaded);
		}

		void OnLoaded(RuntimeGltfInstance loaded, string path)
		{
			if ( loaded == null || loaded.Root == null )  return;
			DestroyLastLoaded();
			_lastLoaded = loaded;
			loaded.Root.transform.SetParent(transform, false);// このcomponentがattachされているオブジェクトを親に設定する
			loaded.Root.transform.position.Set(-1,1,3); // 位置調整
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
					var exrec = GetExrec();
					if ( exrec != null && target != null ) exrec.Model = loaded.Root;
					m_lastLoadedFile = path; // load と show が成功してから更新する
				}
			}
			Settings.ModelFile = path;
			SolutionUtils.GetSolution().Model.SetVRMModel(loaded.gameObject);
		}

		private async void LoadModelSakura(string path)
		{
			Logger.Log($"Load VRM Model: '{path}'");
			bool try10 = false;
			// VRM0
			try
			{
				using ( var data = new GlbFileParser(path).Parse() ) 
				{
					var vrm = new VRMData(data);
					using ( var context = new VRMImporterContext(vrm) ) 
					{
						var loaded = default(RuntimeGltfInstance);

						if (m_loadAsync)
						{
							try {
								loaded = await context.LoadAsync(new VRMShaders.RuntimeOnlyAwaitCaller());
							} catch(System.Exception e) {
								System.Type exceptionType = e.GetType();
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
											m_loading = false;
											return;
										}
									}
									else {
										m_loading = false;
										return;
									}
								}
								m_loading = false;
								return;
							} catch (System.Exception e) {
								m_loading = false;
								return;
							}
						}
						loaded.EnableUpdateWhenOffscreen();
						OnLoaded(loaded, path);
					}
				}
			}
			catch (NotVrm0Exception e)
			{
				Debug.Log("NotVrm0Exception: Try to load VRM1");
				try10 = true;
				//continue loading
			}

			// VRM1 : MCP2VMCP のソース例を真似
			if ( try10 ) {
				try
				{
					string vrmfilepath = path;
					byte[] VRMdataRaw = File.ReadAllBytes(path);
					GlbLowLevelParser glbLowLevelParser = new GlbLowLevelParser(null, VRMdataRaw);
					GltfData gltfData = glbLowLevelParser.Parse();

					synchronizationContext.Post(async (_) =>
					{
						var instance = await Vrm10.LoadBytesAsync(VRMdataRaw, canLoadVrm0X: false, showMeshes: true);
						await Task.Delay(100); //VRM1不具合対策

						OnLoaded10(instance.gameObject, path);
					}, null);

					gltfData?.Dispose();
				}
				catch (Exception e)
				{
					Debug.Log("VRM1 Exception: " +  e.Message  );
				}
			}
			Debug.Log("Load Model done");
		}
		
/// <summary>
/// tbbsakur added END
/// </summary>

		public void LoadCustomImage(string path) {
			if (path == "") {
				return;
			}

			if (!File.Exists(path)) {
				Logger.Log($"Failed to load background image '{path}'");
				return;
			}

			Settings.ImageFile = path;
			Texture2D tex = new(2, 2);
			tex.LoadImage(File.ReadAllBytes(path));

			customizableCanvas.SetBackgroundImage(tex);
		}
		
		public void SetShowBackgroundImage(bool show) {
			Settings.ShowCustomBackground = show;
			customizableCanvas.ShowBackground(show);
		}

		public void SetBackgroundColor(Color color) {
			customizableCanvas.SetBackgroundColor(color);
		}

		public void ResetCamera() {
			orbitalCamera.ResetCamera();
		}

		public void SetShowCamera(bool show) {
			customizableCanvas.ShowWebcam(show);
		}

		public void SetShowBackgroundColor(bool show) {
			Settings.ShowCustomBackgroundColor = show;
		}
	}
}

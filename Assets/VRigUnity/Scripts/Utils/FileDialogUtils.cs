﻿using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace HardCoded.VRigUnity {
	public class FileDialogUtils {
		public struct CustomExtensionFilter {
			public string Name;
			public string[] Extensions;

			public CustomExtensionFilter(string filterName, params string[] filterExtensions) {
				Name = filterName;
				Extensions = filterExtensions;
			}
		}

		public static void OpenFilePanel(MonoBehaviour behaviour, string title, string file, CustomExtensionFilter[] extensions, bool multiSelect, Action<string[]> callback) {
			try {
				callback.Invoke(SFB_OpenFilePanel(title, file, extensions, multiSelect));
			} catch (DllNotFoundException) {
				behaviour.StartCoroutine(ShowLoadDialog(title, file, extensions, multiSelect, callback));
			}
		}

		private static string[] SFB_OpenFilePanel(string title, string file, CustomExtensionFilter[] extensions, bool multiSelect) {
			string directory = "";
			if (File.Exists(file)) {
				directory = Path.GetDirectoryName(file);
			}
	
			SFB.ExtensionFilter[] filters = new SFB.ExtensionFilter[extensions.Length];
			for (int i = 0; i < extensions.Length; i++) {
				filters[i] = new SFB.ExtensionFilter(extensions[i].Name, extensions[i].Extensions);
			}

			return SFB.StandaloneFileBrowser.OpenFilePanel(title, directory, filters, multiSelect);
		}

		private static bool IsWaiting;
		private static IEnumerator ShowLoadDialog(string title, string file, CustomExtensionFilter[] extensions, bool multiSelect, Action<string[]> callback) {
			if (!IsWaiting) {
				// Make sure we are doing one file browser at a time
				IsWaiting = true;

				try {
					string directory = null;
					if (File.Exists(file)) {
						directory = Path.GetDirectoryName(file);
					}

					SimpleFileBrowser.FileBrowser.Filter[] filters = new SimpleFileBrowser.FileBrowser.Filter[extensions.Length];
					for (int i = 0; i < extensions.Length; i++) {
						filters[i] = new SimpleFileBrowser.FileBrowser.Filter(extensions[i].Name, extensions[i].Extensions);
					}
					SimpleFileBrowser.FileBrowser.SetFilters(true, filters);


					yield return SimpleFileBrowser.FileBrowser.WaitForLoadDialog(
						SimpleFileBrowser.FileBrowser.PickMode.Files, multiSelect, null, directory, title, "Select"
					);

					if (SimpleFileBrowser.FileBrowser.Success) {
						callback.Invoke(SimpleFileBrowser.FileBrowser.Result);
					}
				} finally {
					IsWaiting = false;
				}
			}
		}

		public static void SaveFilePanel(MonoBehaviour behaviour, string title, string file, string extension, Action<string[]> callback) {
			SaveFilePanel(behaviour, title, file, new CustomExtensionFilter("*." + extension, extension), callback);
		}


		public static void SaveFilePanel(MonoBehaviour behaviour, string title, string file, CustomExtensionFilter extension, Action<string[]> callback) {
			try {
				callback.Invoke(SFB_SaveFilePanel(title, file, extension));
			} catch (DllNotFoundException) {
				behaviour.StartCoroutine(ShowSaveDialog(title, file, extension, callback));
			}
		}

		private static string[] SFB_SaveFilePanel(string title, string file, CustomExtensionFilter extension) {
			string directory = "";
			if (File.Exists(file)) {
				directory = Path.GetDirectoryName(file);
			}
			
			var extensions = new [] {
				new SFB.ExtensionFilter(extension.Name, extension.Extensions)
			};
			// Known Issue: SaveFilePanel() method from SFB does not read title param.
			string result = SFB.StandaloneFileBrowser.SaveFilePanel(title, directory, "", extensions);
			if (result == string.Empty) {
				return new string[0];
			} else if (!result.EndsWith("." + extension.Extensions[0])) {
				result += "." + extension.Extensions[0];
			}
			return new string[] { result };
		}

		private static IEnumerator ShowSaveDialog(string title, string file, CustomExtensionFilter extension, Action<string[]> callback) {
			if (!IsWaiting) {
				// Make sure we are doing one file browser at a time
				IsWaiting = true;

				try {
					string directory = null;
					if (File.Exists(file)) {
						directory = Path.GetDirectoryName(file);
					}
					
					var filters = new [] {
						new SimpleFileBrowser.FileBrowser.Filter(extension.Name, extension.Extensions)
					};
					SimpleFileBrowser.FileBrowser.SetFilters(true, filters);
					SimpleFileBrowser.FileBrowser.SetDefaultFilter(extension.Extensions[0]);

					yield return SimpleFileBrowser.FileBrowser.WaitForSaveDialog(
						SimpleFileBrowser.FileBrowser.PickMode.Files, false, null, directory, title, "Save"
					);

					if (SimpleFileBrowser.FileBrowser.Success) {
						callback.Invoke(SimpleFileBrowser.FileBrowser.Result);
					}
				} finally {
					IsWaiting = false;
				}
			}
		}
	}
}

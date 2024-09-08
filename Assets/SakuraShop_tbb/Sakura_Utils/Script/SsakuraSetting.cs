// Copyright (c) 2023-2024 Sakura(さくら) / tbbsakura
// MIT License. See "LICENSE" file.

using UnityEngine;
using System.IO;
using SFB;
using System;
using JetBrains.Annotations;
using TMPro;

namespace SakuraScript.Utils 
{
    /// <summary>
    /// 特定のクラスTをjsonに保存/jsonから読み込みする
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SakuraSetting<T>  
    {
        protected T _data;
        public T Data { get => _data; set => _data = value; }
        /// <summary>
        /// ファイル選択ダイアログを出してからロードする(nullは返さない、必ずインスタンスを返す)
        /// </summary>
        public bool LoadFromFile()  // no path specified
        {
            var extensions = new[] {
                new ExtensionFilter("Json Files", "json" ),
            };

            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (paths.Length > 0 && paths[0].Length > 0)
            {
                return LoadFromFile(paths[0]);
            }
            return false;
        }

        public virtual bool LoadFromFile(string path)
        {
            string  json = LoadJsonFromFile(path);
            if (json == "") return false;
            _data = JsonUtility.FromJson<T>(json);
            return true;
        }

        public void SaveToFile()  // no path specified
        {
            var json = JsonUtility.ToJson(_data, true);
            SaveJsonToFile(json);
        }

        public void SaveToFile(string path)
        {
            var json = JsonUtility.ToJson(_data, true);
            StreamWriter sw = new StreamWriter(path,false); 
            sw.Write(json);
            sw.Flush();
            sw.Close();
        }

        // static json load/save functions
        public static string LoadJsonFromFile(string path)
        {
            StreamReader sr = new StreamReader(path, false);
            string json = "";
            while(!sr.EndOfStream) {
                json += sr.ReadLine ();
            }
            sr.Close();
            return json;
        }

        static public void SaveJsonToFile(string json, string nameDefault = "default.json" )
        {
            var extensions = new[] {
                new ExtensionFilter("Json Files", "json" ),
            };

            var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", nameDefault, extensions);

            if (path.Length > 0)
            {
                StreamWriter sw = new StreamWriter(path,false); 
                sw.Write(json);
                sw.Flush();
                sw.Close();
            }
        }
    }
}

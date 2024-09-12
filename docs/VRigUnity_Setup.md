# VRigUnity を自分のプロジェクト(Unity2022) Build できるようにする手順メモ
1. VRigUnity を git clone で持ってくる。

2. (Assets/download_lib.sh が Linux でもうまく動かなかったので)
shに書いてあるKariaro さんの mediapipe plugin の github (https://github.com/Kariaro/MediaPipeUnityPlugin/releases)
行って com.github.homuler.mediapipe-0.10.1-macos11.tgz をを手動でダウンロードして 
tgz 内の package/Runtime/Plugins/ の中身をプロジェクトの 
Packages/com.github.homuler.mediapipe/Runtime/Plugins に入れる。他は解凍しない。
（既に Plugins フォルダはあるが、中身が入ってないので、それだけ入れる感じ）
homuler さんとこ(https://github.com/homuler/MediaPipeUnityPlugin/releases) のでうまく行くのかは不明 

3. Project に対応したバージョンのUnity入れる。

4. プロジェクトを開く。空のシーンになっているので scenes/ の中のシーンファイル１つだけあるWorkspace.unityを開きなおす。

5. ヒエラルキー solutionの下にDebugModelとNew UIを移動してから solutionをProject 適当なところにD&Dしてprefab保存。
(親子にしておかないと、一部Inspector設定が外部扱いとなって保存されないので親子にする。)

6. 作った prefab を右クリックして Export package。全部入れても良いと思うが
個人的には、uOSCや EVMCP4U のような移行先にもある奴はバージョン違いを半端に上書きすると怖いのでひとまず除外してやっている。
(入れてつっこんでエラー出たら修正するか、入れないでおいて足りなかったら入れる等修正するかの差)

7. Unity2022のほうでこれだけインポートした場合、Packages がないからエラーになるので
Packages/com.github.homuler.mediapipe/ もコピーしておく。

8. Animation Rigging が入ってないのでエラーになる。Unity Package Manager でいれる。多少のバージョン違いはご愛敬。

9. Newtonsoftもないので、いれる。 github URL指定：com.unity.nuget.newtonsoft-json
(Unity2020, Unity2021では何もせずとも最初からNewtonsoft.Jsonが使えるらしいので、パッケージリストにはいない。2022から Jsonまわりの公式が別のパッケージになったので、入れないと無い)

10. Solution prefabとCustmizable Canvas の Prefabをシーン直下に置いて走らせれば通る。エラーがいくつか出る。
install.iss というテキストファイルを読んでバージョンチェックする部分が、ファイルがないとエラーになるのでコードごと消す。

11. Test でエラー出るのは消す

# TMARelay Build
- TMARelay を改変したり、中身を把握して違うソフト等で活用したい方向けに、ビルド方法を説明するものです。
- ユーザー向け設定を済ませていることが前提です。

## 1. Unity project設定
Unity HUBから新規のunityprojectを作り（VRCSDK不要なのでVCCは使わない）に 以下のものを入れる

- [EVMCP4U](https://github.com/gpsnmeajp/EasyVirtualMotionCaptureForUnity) v5_0bを使っています(TMARelay v0.4.0からv5系/VRM1対応になっています)
- [StandaloneFileBrowser](https://github.com/gkngkc/UnityStandaloneFileBrowser) (TMARelay v0.2.2以降)

その後、このリポジトリのファイルをプロジェクトに追加。

シーンファイル Assets/ExternalReceiver を開いてAssets/SakuraShop_tbb/TMARelay/VRM_prefab/VRMScene にある Muscle2OSC をヒエラルキー直下に置く。
または シーンファイル TMARelay.scene を開く。

あとは File - Buildsetting で設定してビルドすれば TMARelay.exe ができます。

## 2. スクリプトについて
Assets/SakuraShop_tbb/TMARelay/Script 内にあります。

TMARelay は MIT Licence で、他のMIT Licenseのものも色々使っているので、再配布等したい場合はAssets/SakuraShop_tbb/TMARelay/LICENCES 内を確認してください。

SakuraVRMMuscle2OSC.cs が、マッスル値を読み込んでOSC送信するスクリプト、
FileDragAndDrop/TMARelay_FileDragAndDrop.cs が、ドラッグ＆ドロップ処理のスクリプトです。

### 3. WebCamKit (WebCamかんたんパック)のビルドについて
WebCamKit ブランチをビルドすればWebCamかんたんパックとして頒布している、VRigUnity統合版がビルドできます。
v0.5.0Wからは VRigUnityも除外せず含めていますが、オリジナルの VRigUnity が除外している MediaPipe のリソースは含めていません。

Kariaro さんの mediapipe plugin の github (https://github.com/Kariaro/MediaPipeUnityPlugin/releases)
行って com.github.homuler.mediapipe-0.10.1-macos11.tgz をを手動でダウンロードして 
tgz 内の package/Runtime/Plugins/ の中身をプロジェクトの 
Packages/com.github.homuler.mediapipe/Runtime/Plugins に入れてください。
他は解凍しなくてOKです（既に Plugins フォルダはあるが、中身が入ってないので、それだけ入れる感じ）



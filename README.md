# TMARelay Build
- TMARelay を改変したり、中身を把握して違うソフト等で活用したい方向けに、ビルド方法を説明するものです。
- ユーザー向け設定を済ませていることが前提です。
- 2024/12/9時点の最新は、WebCamKit ブランチに統合されています。v0.6.0W以降を参照してください。mainは v0.4.0時点(tagあり)、その後の更新はBinEncodeブランチにありますが、WebCamKit側に主要機能を実装してそちらが主になっています。

## 1. Unity project設定
Unity HUBから新規のunityprojectを作り（VRCSDK不要なのでVCCは使わない）に 以下のものを入れる

- [EVMCP4U](https://github.com/gpsnmeajp/EasyVirtualMotionCaptureForUnity) v5_0bを使っています(TMARelay v0.4.0からv5系/VRM1対応になっています)
- [StandaloneFileBrowser](https://github.com/gkngkc/UnityStandaloneFileBrowser) (TMARelay v0.2.2以降)

## 2. VRigUnity, MediaPipePlugin設定
WebCamKit ブランチv0.5.0Wからは VRigUnityも除外せず含めていますが、オリジナルの VRigUnity が除外している MediaPipe のリソースは含めていません。
Kariaro さんの mediapipe plugin の github (https://github.com/Kariaro/MediaPipeUnityPlugin/releases)
行って com.github.homuler.mediapipe-0.10.1-macos11.tgz をを手動でダウンロードして 
tgz 内の package/Runtime/Plugins/ の中身をプロジェクトの 
Packages/com.github.homuler.mediapipe/Runtime/Plugins に入れてください。
他は解凍しなくてOKです（既に Plugins フォルダはあるが、中身が入ってないので、それだけ入れる感じ）

## 3. build
その後、このリポジトリのファイルをプロジェクトに追加。
シーンファイル TMARelay.WebCamKit.scene.unity を開く。
あとは File - Buildsetting で設定してビルドすれば TMARelay_WebCamKit.exe ができます。

## 4. スクリプトについて
Assets/SakuraShop_tbb/TMARelay/Script 内にあります。

TMARelay は MIT Licence で、他のMIT Licenseのものも色々使っているので、再配布等したい場合はAssets/SakuraShop_tbb/TMARelay/LICENCES 内を確認してください。

SakuraVRMMuscle2OSC.cs が、マッスル値を読み込んでOSC送信するスクリプト、
FileDragAndDrop/TMARelay_FileDragAndDrop.cs が、ドラッグ＆ドロップ処理のスクリプトです。
v0.6.0W時点で、VRigUnityのコードの微修正もかなり含まれています（なるべくコメント等でどこが該当箇所かわかるようにしています）




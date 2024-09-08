# TMARelay Build
- TMARelay を改変したり、中身を把握して違うソフト等で活用したい方向けに、ビルド方法を説明するものです。
- ユーザー向け設定を済ませていることが前提です。

## 1. Unity project設定
Unity HUBから新規のunityprojectを作り（VRCSDK不要なのでVCCは使わない）に 以下のものを入れる

- [EVMCP4U](https://github.com/gpsnmeajp/EasyVirtualMotionCaptureForUnity) v4系/VRM0用で開発しています
- [StandaloneFileBrowser](https://github.com/gkngkc/UnityStandaloneFileBrowser) (TMARelay v0.2.2以降)

その後 TMARelay_VRM_(バージョン番号).unitypackage を入れる。(またはこのリポジトリをcloneする）

シーンファイル Assets/ExternalReceiver を開いてAssets/SakuraShop_tbb/TMARelay/VRM_prefab/VRMScene にある Muscle2OSC をヒエラルキー直下に置く。
または シーンファイル TMARelay.scene を開く。

あとは File - Buildsetting で設定してビルドすれば TMARelay.exe ができます。

## 2. スクリプトについて
Assets/SakuraShop_tbb/TMARelay/Script 内にあります。

TMARelay は MIT Licence で、他のMIT Licenseのものも色々使っているので、再配布等したい場合はAssets/SakuraShop_tbb/TMARelay/LICENCES 内を確認してください。

SakuraVRMMuscle2OSC.cs が、マッスル値を読み込んでOSC送信するスクリプト、
FileDragAndDrop/TMARelay_FileDragAndDrop.cs が、ドラッグ＆ドロップ処理のスクリプトです。

Unity上のPlay ではドラッグ＆ドロップができないので、代わりにスクリプト内で指定したファイルを読めるようになっています。
33行目あたりのLoadModel のコメントを解除して、任意のファイルパスを指定してください。
(v0.2.2に向けて修正作業中)


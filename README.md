# IroSphere Legacy



■概要

画像の色を参照し、ＨＳＬ色空間に配置します。
詳しくは動画をご覧ください

Youtube版
https://youtu.be/jrCr4j_qdPo

ニコニコ版
https://www.nicovideo.jp/watch/sm42022218



■簡単な使い方

・Projectウィンドウの「Scenes > SampleScene」を開きます。

・画面上のPlayボタンでゲーム実行します

・左の写真をクリックすると、対応した箇所の座標にノードが作成されます



■ゲーム実行時の操作方法

ノードの作成　→　左クリック

スフィアの回転　→　WASD

スフィアの移動　→　十字キー  

スフィアの拡縮　→　QE / マウスホイール

ヘルプの表示　→　H

色情報の表示　→　I

画像の非表示　→　スペース

グリッドの非表示　→　G

ノードのランダム生成　→　R

ノードを一つ削除　→　Z

ノードを全て削除　→　C

サブスフィアと入れ替え　→　F

セーブ　→　K

ロード　→　L（SphereManagerのSaveDataスロットにセーブデータをドラッグ＆ドロップした状態で）



■画像を差し替えたい時


・画像を差し替えたいときは、Hierarchyの「SphereManager」ノードのPictureに画像をドラッグ・アンド・ドロップしてください。

　画像はProjectウィンドウのPictureフォルダにサンプル画像が何枚か入っています。

・自分の画像を使いたい時は、ゲームを終了状態で、好きな画像をエクスプローラーからProjectウィンドウの「Picture」フォルダにドラッグ・アンド・ドロップしてインポートし、
　その画像のInspectorで「TextureType」 を「Sprite(2D and UI)」に変更します。あとは同じ操作です。

 
 
■パラメーター調整方法

「Hierarchy」の、「SphereManger」の「Param」にScriptableObjectがささっています。このファイルを編集する事で調整可能です。 
ゲーム実行中編集不可のパラメーターは、実行中グレーアウトします
 
  ■ライセンスについて
  
CC0です。ご自由にどうぞ



■更新履歴

2023-04-15

　・PictureにRGB画像補正機能追加


2023-04-05

　・SphereManagerにSaturation（彩度）パラメーターの追加

　・レンダーパイプラインをURPに変更

　・ランダム取得する際、画像の外を１ピクセルだけ拾ってしまうバグがあったので修正

　・（追記）画像読み取りが上下反転していたのを修正


2023-04-02

　・コントローラー入力がシビアに反応しすぎていたので遊び範囲を広げました


2023-04-02

　・Mac版対応の為、文字コードをUTF-8に、改行コードをLFに


2023-04-02

　・「I」キーのインフォーメーション表示で、画像の外の色も拾える様に修正

　・右クリックで、マウスカーソルのある位置のカラー情報をクリップボードにコピー出来るように

　・クリックで追加出来るノードの最大数を1000に

　・初期配置ノードの最低数に0を指定出来る様に（初期配置ノードを非表示に出来る様に）


2023-04-01

　・配布開始

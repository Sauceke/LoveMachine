[English](README.md) | [日本語](README-jp.md)

# BepInEx LoveMachine

[![QA][CI Badge]](#) [![Download][Downloads Badge]][インストーラ]
[![Patreon][Patreon Badge]][Patreon]

[![インストーラ][Download Button]][インストーラ] &nbsp; [![動画(R18)][Demo Button]][Demo video]

LoveMachine はアダルトゲームのキャラクターと連動してアダルトグッズを動かすプラグインです。

[一部の自律型デバイス](#サポートしているデバイス)について、以下のゲームに対応しています。

| ゲームタイトル               | 開発者             | VR 対応        |
| :--------------------------- | :----------------- | :------------- |
| [カスタムオーダーメイド 3D2] | Kiss               | ⭕             |
| [聖騎士リッカの物語]         | もぐらソフト       | ❌             |
| [ハニーセレクト 2]           | ILLUSION           | ⭕             |
| [放課後輪姦中毒]             | みこにそみ         | ⭕※[AGHVR]のみ |
| [淫魔界 2: カムラン]         | 淡泊室             | ❌             |
| [インサルトオーダー]         | みこにそみ         | ⭕※[IOVR]のみ  |
| [コイカツ！]                 | ILLUSION           | ⭕             |
| [コイカツ！パーティ]         | ILLUSION           | ⭕             |
| [コイカツ！サンシャイン]     | ILLUSION           | ⭕             |
| [恋来い温泉物語]             | アプリコットハート | ⭕             |
| [Last Evil]                  | Flametorch         | ❌             |
| [大江戸とりがー!!]           | CQC Software       | ❌             |
| [Our Apartment]              | Momoiro Software   | ❌             |
| [プレイホーム]               | ILLUSION           | ⭕             |
| [ROOM ガール]                | ILLUSION           | ❌             |
| [セクロスフィア]             | ILLUSION           | ❌             |
| [Succubus Cafe]              | MIGI STUDIO        | ❌             |
| [VR カノジョ]                | ILLUSION           | ⭕             |

## サポートしているデバイス

LoveMachine はアダルトグッズとの連動を可能にする[Buttplug.io]プロジェクトに頼って作られています。執
筆時点で Buttplug.io は 200 以上のデバイスをサポートしています。

このプラグインは**上下運動**、**振動**、**回転**、**締め付け**機能に対応しています。

以下は実際に MOD でテストしたデバイスの一覧です：

電動オナホ

- [The Handy]
- [Kiiroo KEON]
- OSR2

振動機能付きオナホ

- [Lovense Gush]
- [Lovense Max 2]
- [Lovense Diamo]
- [Lovense Domi 2]
- [Lovense Calor]
- The Xbox gamepad

回転機能付きオナホ

- Vorze A10 Cyclone

## インストール

[インストーラ]をダウンロードして実行してください。もし「Windows があなたの PC を保護しました」という
メッセージが出たら、詳細情報 > 実行 をクリックしてください。

[Intiface Central]もインストールする必要があります。

## 使い方

1. Intiface Central を起動します。
2. プレイボタンをクリックしてください。
3. 使いたいデバイスを起動して接続してください。
4. ゲーム開始！

⚠ 一部のゲームでは、BepInEx コンソールが VR ゲームのプレイ中に開いている場合、停止ボタンが機能しない
場合があります。これはゲームウィンドウよりも BepInEx コンソールが前面に出ているため、BepInEx の画面
の動作が優先されてしまうために起こります。このような場合はコンソールを無効にすることをお勧めします。

## 動作方法・制限

- LoveMachine は、女性キャラの特定のボーン(手、股間、胸、口)の動きを分析して、各アニメーションループ
  の開始時にオナホを作動させるタイミングを正確に決定します。
- 上下運動(およびバイブの振動)は、キャリブレーション中に記録された、男性キャラの金玉袋に最も近いボー
  ンの動きに合わせて作動します。(玉舐めアニメーション中は同期できなくなりますが、それ以外の場合は上
  手く機能します)
- 全身のボーン位置によって動作を決定しているので、画面に映り切らないサイズや比率のキャラクターには対
  応できません。

## プラグイン設定

⚠ IL2CPP を利用しているゲーム(RoomGirl, 聖騎士リッカの物語) は、現時点では ConifigurationManager と
互換性がありません。代わりに設定ファイルを設定してください。

プラグイン設定 > LoveMachine から以下のパラメータを設定できます。

### アニメーション設定(『コイカツ！』と『コイカツ！サンシャイン』のみ対応)

- **Simplify animations:** このオプションを有効にすると、モーションブレンド(キャラの複数のモーション
  の合成機能)をオフにします。モーションブレンドは動作タイミングが検出しにくくなるため、特に
  SideLoader のアニメーションで真の没入感を得たい場合には、この設定が不可欠です。デフォルトではオフ
  になっています。他の MOD と干渉する可能性があります。

### デバイス一覧

ここには Intiface で接続できるすべてのデバイスが表示されます。

- **Connect:** Intiface サーバーと接続・再接続します。
- **Scan** デバイスをスキャンします。

一般的なデバイス設定 (全てのデバイスに共通):

- **Group Role:** 複数の女性キャラが映るシーンで、どの女の子とデバイスを同期させるかを選択します。こ
  れは複数のキャラが映らないシーンにも影響します。例えば、ある女の子キャラクタ A とデバイスを同期さ
  せているとき、別の女の子キャラクター B だけが映るシーンになった場合、デバイスは動作しません。
- **Body Part:** デバイスが追跡する部位のボーンを選択します。デフォルトはオート(プレイヤーの金玉に最
  も近いボーン)です。TJ/FJ を再現する際には、2 台のデバイスで交互に動作させることが可能です。『コイ
  カツ！』『コイカツ！サンシャイン』ではフェラチオや手コキも検出することができます。
- **Latency (milliseconds):** アダルトグッズの動作遅延は通常無視できるほど小さいですが、ディスプレイ
  とデバイスの動きに大きなズレがある場合には、このパラメータを調整することでズレを補正することができ
  ます。最適な調整値は利用する環境によって異なるので、実際に試しながら少しづつ調整してください。
- **Updates per second:** デバイスに動作コマンドを送信する頻度を指定します。BLE デバイスは通常、1 秒
  間に約 10~20 個のコマンドを処理できます。

オナホ設定:

- **Max Strokes (per minute):** 100%の長さで上下させることが可能な最高速度です。
- **Stroke Zone / Slow:** 動きをスローにしたときの上下運動の長さです。0%が最小、100%が最大です。
- **Smooth Stroking:** 上下運動の動きからロボっぽさを減らしますが、全てのオナホが対応しているわけで
  はありません。Handy と OSR2 デバイスで上手く機能することが分かっています。デフォルトではオフになっ
  ています。

バイブ設定:

- **Intensity Range:** このデバイスで可能な最小~最大の振動の強さを設定します。0%=振動無し、100%=振動
  最大。
- **Vibration Pattern:** 振動のパターンを指定します。利用可能なパターンは Sin 波、三角波、鋸波、パル
  ス波、変化なし、カスタムパターンです。
- **Custum Pattern:** Vibration Pattern が Custom に設定されている場合に使用できますスライダーを使用
  して、振動強度のカーブを設定することができます。

圧力設定:

- **Pressure Range:** このデバイスが許容する最小および最大の圧力を設定します。単位は(%)です。
- **Pressure Update Interval (seconds):** デバイスが圧力を変更するのに掛かる時間です。単位は(秒)。デ
  フォルトは 5 秒です。

以下の設定も行うといいかもしれません:

- **Save device assignments:** 有効にすると、すべてのデバイス「3P の役割」と「身体の部位属性」が共有
  できます。デフォルトでは無効になっています。

### Intiface 設定

- **WebSocket host:** 実行中の Intiface ホストの URL です。リモートマシン上で実行されていない限り
  、`ws://127.0.0.1`でなければなりません。
- **WebSocket port:** Intiface が解放しているポートです。通常は`12345`です。

### 停止ボタン設定

万が一何かが起こった時、または激しいセックスが始まってしまった時、あなたを守るために停止ボタンをご用
意しました。デフォルトではスペースキーを押すことで全てのデバイスが直ちに停止します。

- **Emergency Stop Key Binding:** 停止ボタンを作動させるためのキー割り当てを設定します。(デフォルト
  ではスペースキー)
- **Resume Key Binding:** 停止を解除するためのキー割り当てを設定します。(デフォルトでは F8)

### オナホ設定

- **Stroke Length Realism:** 上下運動の幅をアニメーションとどの程度一致させるかを設定します。0%では
  デバイスが利用可能な長さをフル活用します。100%ではゲーム内のアニメーションの長さにできるだけ合わせ
  て動きます。
- **Hard Sex Intensity:** 激しいセックスアニメーション中にデバイスが動く強さを調整します。100%にする
  と 0%のときより 2 倍早く動きます。LoveMachine の使用により発生するいかなる損害についても、開発者は
  一切の責任を負いかねます。
- **Orgasm Depth:** 絶頂時のデバイスの動きの深さを設定します。
- **Orgasm Shaking Frequency:** 絶頂中に 1 秒間で何回上下運動を行うかを設定します。

### 回転設定

- **Rotation Speed Ratio:** 回転の速度を設定します。0%は無回転、100%全速回転です。デフォルトは 50%で
  す。
- **Rotation Direction Change Chance:** 回転方向が変化する確率を設定します。デフォルトは 30%です。

### 圧力設定

- **Enable Pressure Control:** このデバイスの圧力機能を使用するかどうかを設定します。デフォルトでは
  無効になっています。
- **Pressure Mode:** 圧力の設定方法を決定します。
  - **Cycle:** 一定時間かけて徐々に圧力を高め、最後に開放します。
  - **Stroke Length:** 圧力をかける面積です。圧力が強くなります。
  - **Stroke Speed:** 圧力がかかる速さです。圧力が強くなります。
- **Pressure Cycle Length (seconds):** 圧力設定が Cycle に設定されている場合、圧力が強まってから解放
  されるまでの時間間隔を設定します。単位は(秒)です。

## ご協力してくれる方へ

対応してほしいゲームタイトルがあれば是非ご連絡ください。新しいゲームタイトルに対応するのは比較的簡単
で、ほとんどコーディングをしなくてもよいのです。参考として PlayHome の実装をご覧ください。また、対応
してほしいデバイスの PR も大歓迎です。

この MOD は無料ですが、寄付を受け付けています。私の開発を応援してくださる方は、[こちら][Patreon]から
よろしくお願いします。

### 開発者

Sauceke   •   nhydock   •   hogefugamoga   •   RPKU ・ andama777(日本語訳)

### パトロン

[ManlyMarco]   •   Aftercurve   •   AkronusWings   •   Ambicatus   •   Andrew Hall   •  
AstralClock   •   Atlantic Dragon   •   Average MBT viewer   •   Benos Hentai   •   boaz   •  
Bri   •   butz   •   cat tail   •   CBN ヴい    •   Ceruleon   •   CROM   •   Daniel   •  
EPTG   •   er er   •   Flan   •   funnychicken   •   Gabbelgu   •   gmolnmol   •   gold25   •   GOU
YOSIHIRO   •   Greg   •   hiro   •   Ior1yagami   •   Junk   •   Kai Yami   •   KTKT   •  
kuni   •   Laneo   •   mokochurin   •   Nemi   •   Nephilim Bacon   •   nppon   •   PhazR   •  
Phil   •   prepare55   •   purena   •   real name   •   rolandmitch   •   RP 君    •  
SavagePastry   •   Sean McKagan   •   Shakes   •   Taibe   •   tanu   •   TO   •   Tom   •  
TrashTaste   •   ttrs   •   tutinoko   •   unitora   •   uruurian   •   Wel Adunno   •   yamada
tarou   •   Zesty Cucumber   •   Zijian Wang   •   Zomba Mann   •   しゃどみん    •   シルバー
   •   ふ    •   りょすけ みのかわ    •   一太 川崎    •   優希 岩永    •   哲慶 宗    •   国崎往人
   •   将也 三田    •   洋 冨岡    •   猛 羽場    •   终晓    •   郁弥 中村    •   闇《YAMI》   •  
高島　渉

## 謝辞

LoveMachine は[BepInEx]プラグインフレームワークと、そしてもちろん[Buttplug.io]プロジェクトのお陰で作
られています。

<!-- badges -->

[CI Badge]: https://github.com/Sauceke/LoveMachine/actions/workflows/qa.yml/badge.svg
[Downloads Badge]: https://img.shields.io/github/downloads/Sauceke/LoveMachine/total
[Patreon Badge]: https://shields.io/badge/patreon-grey?logo=patreon
[Download Button]:
  https://img.shields.io/badge/%E2%87%93%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%A9-blue?style=for-the-badge
[Demo Button]:
  https://img.shields.io/badge/%E2%96%B6_%E5%8B%95%E7%94%BB(R18)-pink?style=for-the-badge

<!-- own links -->

[インストーラ]:
  https://github.com/Sauceke/LoveMachine/releases/latest/download/LoveMachineInstaller.exe
[LoveMachine.Experiments]: https://sauceke.github.io/LoveMachine.Experiments
[Hotdog]: https://sauceke.github.io/hotdog
[Patreon]: https://www.patreon.com/sauceke
[Demo video]: https://www.erome.com/a/f2XKHJ1I

<!-- sponsored game links -->

[カスタムオーダーメイド3D2]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ011538.html/?locale=en_US
[聖騎士リッカの物語]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ363824.html/?locale=en_US
[ハニーセレクト2]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ013722.html/?locale=en_US
[放課後輪姦中毒]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ189924.html/?locale=en_US
[淫魔界2: カムラン]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ321625.html/?locale=en_US
[インサルトオーダー]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ220246.html/?locale=en_US
[ROOMガール]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015465.html/?locale=en_US
[コイカツ！サンシャイン]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015724.html/?locale=en_US
[セクロスフィア]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015728.html/?locale=en_US
[プレイホーム]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015713.html/?locale=en_US
[コイカツ！]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015719.html/?locale=en_US
[恋来い温泉物語]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ01000460.html/?locale=en_US
[大江戸とりがー!!]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ439205.html/?locale=en_US

<!-- sponsored sex toy links -->

[The Handy]:
  https://www.thehandy.com/?ref=saucekebenfield&utm_source=saucekebenfield&utm_medium=affiliate&utm_campaign=The+Handy+Affiliate+program
[Kiiroo KEON]: https://feelrobotics.go2cloud.org/aff_c?offer_id=4&aff_id=1125&url_id=203
[Lovense Calor]: https://www.lovense.com/r/vu65q6
[Lovense Gush]: https://www.lovense.com/r/f7lki7
[Lovense Max 2]: https://www.lovense.com/r/k8bbja
[Lovense Diamo]: https://www.lovense.com/r/54xpc7
[Lovense Domi 2]: https://www.lovense.com/r/77i51d

<!-- other links -->

[ManlyMarco]: https://github.com/ManlyMarco
[Buttplug.io]: https://github.com/buttplugio/buttplug
[Intiface Central]: https://intiface.com/central
[BepInEx]: https://github.com/BepInEx
[AGHVR]: https://github.com/Eusth/AGHVR
[IOVR]: https://github.com/Eusth/IOVR
[Our Apartment]: https://www.patreon.com/momoirosoftware
[コイカツ！パーティ]: https://store.steampowered.com/app/1073440/__Koikatsu_Party/
[VRカノジョ]: http://www.illusion.jp/preview/vrkanojo/index_en.php
[Last Evil]: https://store.steampowered.com/app/823910/last_evil/
[Succubus Cafe]: https://store.steampowered.com/app/1520500/Succubus_Cafe/

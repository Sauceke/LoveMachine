# BepInEx LoveMachine
[![.NET][CI Badge]](#)
[![Download][Downloads Badge]][インストーラ]
[![Patreon][Patreon Badge]][Patreon]

[![インストーラ][Download Button]][インストーラ] &nbsp;
[![動画(R18)][Demo Button]][Demo video]

LoveMachineはアダルトゲームのキャラクターと連動してアダルトグッズを動かすプラグインです。 

[一部の自律型デバイス](#サポートしているデバイス)について、以下のゲームに対応しています。
| ゲームタイトル | 開発者 | VR対応 |
|:----|:----|:----|
| [カスタムオーダーメイド3D2] | Kiss | ⭕ |
| [聖騎士リッカの物語] | もぐらソフト | ❌ |
| [ハニーセレクト2] | ILLUSION | ⭕ |
| [放課後輪姦中毒] | みこにそみ | ⭕※[AGHVR]のみ |
| [淫魔界2: カムラン] | 淡泊室 | ❌ |
| [インサルトオーダー] | みこにそみ | ⭕※[IOVR]のみ |
| [コイカツ！] | ILLUSION | ⭕ |
| [コイカツ！パーティ] | ILLUSION | ⭕ |
| [コイカツ！サンシャイン] | ILLUSION | ⭕ |
| [恋来い温泉物語] | アプリコットハート | ⭕ |
| [Last Evil] | Flametorch | ❌ |
| [Our Apartment] | Momoiro Software | ❌ |
| [プレイホーム] | ILLUSION | ⭕ |
| [ROOMガール] | ILLUSION | ❌ |
| [セクロスフィア] | ILLUSION | ❌ |
| [Succubus Cafe] | MIGI STUDIO | ❌ |
| [VRカノジョ] | ILLUSION | ⭕ |

## サポートしているデバイス
LoveMachineはアダルトグッズとの連動を可能にする[Buttplug.io]プロジェクトに頼って作られています。執筆時点でButtplug.ioは200以上のデバイスをサポートしています。

このプラグインは**上下運動**、**振動**、**回転**、**締め付け**機能に対応しています。

以下は実際にMODでテストしたデバイスの一覧です：

電動オナホ
- [The Handy]
- OSR2
- KIIROO KEON

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
[インストーラ]をダウンロードして実行してください。もし「Windows があなたの PC を保護しました」というメッセージが出たら、詳細情報 > 実行 をクリックしてください。

[Intiface Central]もインストールする必要があります。

## 使い方
1. Intiface Centralを起動します。
2. プレイボタンをクリックしてください。
3. 使いたいデバイスを起動して接続してください。
4. ゲーム開始！

⚠ 一部のゲームでは、BepInExコンソールがVRゲームのプレイ中に開いている場合、停止ボタンが機能しない場合があります。これはゲームウィンドウよりもBepInExコンソールが前面に出ているため、BepInExの画面の動作が優先されてしまうために起こります。このような場合はコンソールを無効にすることをお勧めします。

## 動作方法・制限
- LoveMachineは、女性キャラの特定のボーン(手、股間、胸、口)の動きを分析して、各アニメーションループの開始時にオナホを作動させるタイミングを正確に決定します。
- 上下運動(およびバイブの振動)は、キャリブレーション中に記録された、男性キャラの金玉袋に最も近いボーンの動きに合わせて作動します。(玉舐めアニメーション中は同期できなくなりますが、それ以外の場合は上手く機能します)
- 全身のボーン位置によって動作を決定しているので、画面に映り切らないサイズや比率のキャラクターには対応できません。

## プラグイン設定
⚠ IL2CPP を利用しているゲーム(RoomGirl, 聖騎士リッカの物語) は、現時点ではConifigurationManagerと互換性がありません。代わりに設定ファイルを設定してください。

プラグイン設定 > LoveMachine から以下のパラメータを設定できます。

### アニメーション設定(『コイカツ！』と『コイカツ！サンシャイン』のみ対応)
- **アニメーションの軽量化:** このオプションを有効にすると、モーションブレンド(キャラの複数のモーションの合成機能)をオフにします。モーションブレンドは動作タイミングが検出しにくくなるため、特にSideLoaderのアニメーションで真の没入感を得たい場合には、この設定が不可欠です。デフォルトではオフになっています。他のMODと干渉する可能性があります。

### デバイス一覧
ここにはIntifaceで接続できるすべてのデバイスが表示されます。
- **Connect:** Intifaceサーバーと接続・再接続します。
- **Scan** デバイスをスキャンします。

一般的なデバイス設定 (全てのデバイスに共通):
- **Group Role:** 複数の女性キャラが映るシーンで、どの女の子とデバイスを同期させるかを選択します。これは複数のキャラが映らないシーンにも影響します。例えば、ある女の子キャラクタAとデバイスを同期させているとき、別の女の子キャラクターBだけが映るシーンになった場合、デバイスは動作しません。
- **Body Part:** デバイスが追跡する部位のボーンを選択します。デフォルトはオート(プレイヤーの金玉に最も近いボーン)です。TJ/FJを再現する際には、2台のデバイスで交互に動作させることが可能です。『コイカツ！』『コイカツ！サンシャイン』ではフェラチオや手コキも検出することができます。
- **Latency (milliseconds):** アダルトグッズの動作遅延は通常無視できるほど小さいですが、ディスプレイとデバイスの動きに大きなズレがある場合には、このパラメータを調整することでズレを補正することができます。最適な調整値は利用する環境によって異なるので、実際に試しながら少しづつ調整してください。
- **Updates per second:** デバイスに動作コマンドを送信する頻度を指定します。BLEデバイスは通常、1秒間に約10~20個のコマンドを処理できます。

オナホ設定:
- **Max Strokes (per minute):** 100%の長さで上下させることが可能な最高速度です。
- **Stroke Zone / Slow:** 動きをスローにしたときの上下運動の長さです。0%が最小、100%が最大です。
- **Smooth Stroking:** 上下運動の動きからロボっぽさを減らしますが、全てのオナホが対応しているわけではありません。HandyとOSR2デバイスで上手く機能することが分かっています。デフォルトではオフになっています。

バイブ設定:
- **Intensity Range:** このデバイスで可能な最小~最大の振動の強さを設定します。0%=振動無し、100%=振動最大。
- **Vibration Pattern:** 振動のパターンを指定します。利用可能なパターンはSin波、三角波、鋸波、パルス波、変化なし、カスタムパターンです。
- **Custum Pattern:** Vibration PatternがCustomに設定されている場合に使用できますスライダーを使用して、振動強度のカーブを設定することができます。

圧力設定:
- **Pressure Range:** このデバイスが許容する最小および最大の圧力を設定します。単位は(%)です。
- **Pressure Update Interval (seconds):** デバイスが圧力を変更するのに掛かる時間です。単位は(秒)。デフォルトは5秒です。

以下の設定も行うといいかもしれません:
- **Save device assignments:** 有効にすると、すべてのデバイス「3Pの役割」と「身体の部位属性」が共有できます。デフォルトでは無効になっています。

### Intiface設定
- **Intiface CLI location:** Intiface CLIの実行ファイルがあるパスです。LoveMachineはゲームが起動すると、このプログラムを起動しようとします。
- **WebSocket host:** 実行中のIntifaceホストのURLです。リモートマシン上で実行されていない限り、`ws://localhost`でなければなりません。
- **WebSocket port:** Intifaceが解放しているポートです。通常は`12345`です。

### 停止ボタン設定
万が一何かが起こった時、または激しいセックスが始まってしまった時、あなたを守るために停止ボタンをご用意しました。デフォルトではスペースキーを押すことで全てのデバイスが直ちに停止します。

- **Emergency Stop Key Binding:** 停止ボタンを作動させるためのキー割り当てを設定します。(デフォルトではスペースキー)
- **Resume Key Binding:** 停止を解除するためのキー割り当てを設定します。(デフォルトではF8)

### オナホ設定
- **Stroke Length Realism:** 上下運動の幅をアニメーションとどの程度一致させるかを設定します。0%ではデバイスが利用可能な長さをフル活用します。100%ではゲーム内のアニメーションの長さにできるだけ合わせて動きます。

- **Hard Sex Intensity:** 激しいセックスアニメーション中にデバイスが動く強さを調整します。100%にすると0%のときより2倍早く動きます(少なくともHandyでは)。
LoveMachineの使用により発生するいかなる損害についても、開発者は一切の責任を負いかねます。

- **Orgasm Depth:** 絶頂時のデバイスの動きの深さを設定します。
- **Orgasm Shaking Frequency:** 絶頂中に1秒間で何回上下運動を行うかを設定します。

### 回転設定
- **Rotation Speed Ratio:** 回転の速度を設定します。0%は無回転、100%全速回転です。デフォルトは50%です。
- **Rotation Direction Change Chance:** 回転方向が変化する確率を設定します。デフォルトは30%です。

### 圧力設定
- **Enable Pressure Control:** このデバイスの圧力機能を使用するかどうかを設定します。デフォルトでは無効になっています。
- **Pressure Mode:** 圧力の設定方法を決定します。
  - **Cycle:** 一定時間かけて徐々に圧力を高め、最後に開放します。
  - **Stroke Length:** 圧力をかける面積です。圧力が強くなります。
  - **Stroke Speed:** 圧力がかかる速さです。圧力が強くなります。
- **Pressure Cycle Length (seconds):** 圧力設定がCycleに設定されている場合、圧力が強まってから解放されるまでの時間間隔を設定します。単位は(秒)です。

## ご協力してくれる方へ
対応してほしいゲームタイトルがあれば是非ご連絡ください。新しいゲームタイトルに対応するのは比較的簡単で、ほとんどコーディングをしなくてもよいのです。参考としてPlayHomeの実装をご覧ください。また、対応してほしいデバイスのPRも大歓迎です。

このMODは無料ですが、寄付を受け付けています。私の開発を応援してくださる方は、[こちら]からよろしくお願いします。
### 開発者
Sauceke   •   nhydock   •   hogefugamoga   •   RPKU   ・ andama777(日本語訳)

### パトロン
[ManlyMarco]   •   Aftercurve   •   AkronusWings   •   Ambicatus   •   Andrew Hall   •   AstralClock   •   Benos Hentai   •   boaz   •   Bri   •   cat tail   •   CBN ヴい   •   Ceruleon   •   CROM   •   Daniel   •   EPTG   •   er er   •   Flan   •   funnychicken   •   Gabbelgu   •   gold25   •   GOU YOSIHIRO   •   Greg   •   hiro   •   Ior1yagami   •   Kai Yami   •   KTKT   •   kuni   •   Laneo   •   mokochurin   •   Nemi   •   nppon   •   PhazR   •   Phil   •   prepare55   •   real name   •   rolandmitch   •   RP君   •   SavagePastry   •   Shakes   •   Taibe   •   tanu   •   TO   •   Tom   •   TrashTaste   •   ttrs   •   tutinoko   •   unitora   •   uruurian   •   Wel Adunno   •   yamada tarou   •   Zesty Cucumber   •   Zijian Wang   •   しゃどみん   •   シルバー   •   ふ   •   一太 川崎   •   優希 岩永   •   哲慶 宗   •   国崎往人   •   将也 三田   •   洋 冨岡   •   猛 羽場   •   终晓   •   郁弥 中村   •   闇《YAMI》   •   高島　渉

## 謝辞
LoveMachineは[BepInEx]プラグインフレームワークと、そしてもちろん[Buttplug.io]プロジェクトのお陰で作られています。

<!-- badges -->
[CI Badge]: https://github.com/Sauceke/LoveMachine/actions/workflows/commit.yml/badge.svg
[Downloads Badge]: https://img.shields.io/github/downloads/Sauceke/LoveMachine/total
[Patreon Badge]: https://shields.io/badge/patreon-grey?logo=patreon
[Download Button]: https://img.shields.io/badge/%E2%87%93%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%A9-blue?style=for-the-badge
[Demo Button]: https://img.shields.io/badge/%E2%96%B6_%E5%8B%95%E7%94%BB(R18)-pink?style=for-the-badge

<!-- own links -->
[インストーラ]: https://github.com/Sauceke/LoveMachine/releases/latest/download/LoveMachineInstaller.exe
[LoveMachine.Experiments]: https://sauceke.github.io/LoveMachine.Experiments
[Hotdog]: https://sauceke.github.io/hotdog
[Patreon]: https://www.patreon.com/sauceke
[Demo video]: https://www.erome.com/a/DhT7BF4B

<!-- sponsored game links -->
[カスタムオーダーメイド3D2]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ011538.html/?locale=en_US
[聖騎士リッカの物語]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ363824.html/?locale=en_US
[ハニーセレクト2]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ013722.html/?locale=en_US
[放課後輪姦中毒]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ189924.html/?locale=en_US
[淫魔界2: カムラン]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ321625.html/?locale=en_US
[インサルトオーダー]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ220246.html/?locale=en_US
[ROOMガール]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015465.html/?locale=en_US
[コイカツ！サンシャイン]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015724.html/?locale=en_US
[セクロスフィア]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015728.html/?locale=en_US
[プレイホーム]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015713.html/?locale=en_US
[コイカツ！]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015719.html/?locale=en_US
[恋来い温泉物語]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ01000460.html/?locale=en_US

<!-- sponsored sex toy links -->
[The Handy]: https://www.thehandy.com/?ref=saucekebenfield&utm_source=saucekebenfield&utm_medium=affiliate&utm_campaign=The+Handy+Affiliate+program
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

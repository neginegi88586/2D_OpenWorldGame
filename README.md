# ～ファミコン時代のムリゲー感が漂う2Dオープンワールドゲーム～


## このリポジトリへの参加方法

### 1. リポジトリのコピーとFork元の登録
1. GitHubリポジトリのWebページで「Fork」ボタンを押す。
2. 入力画面はそのままにして、「Create fork」ボタンを押す。
3. GitHubアカウントの設定にて、「Developer Settings」ボタンを押す。
4. 「Personal access tokens」メニューをクリックし、その中の「Tokens (classic)」ボタンを押す。
5. 初回はページ中央、それ以外はその少し右上に「Generate new token」メニューがあり、その中から「(classic)」とつくものを選ぶ。
6. 「Expiration」の選択肢から「No Expiration」を選択し、「repo」と書かれたチェックボックスにチェックを付ける。
7.  ページ最下部の「Generate token」ボタンを押して出てきた画面にある黄緑色のテキストボックスにコード(以降はこれを「個人アクセストークン」と呼ぶ)があるので、メモしておく。
8. リポジトリ内のプロジェクトを保存したいフォルダーでGitBashを立ち上げる。
9. GitBashで以下の5行のコードを実行する。
```Bash:Bash
git clone https://github.com/<あなたのGitHubユーザー名>/2D_OpenWorldGame.git
cd 2D_OpenWorldGame
git remote add root_branch https://github.com/neginegi88586/2D_OpenWorldGame.git
git remote set-url origin https://<あなたのGitHubユーザー名>:<個人アクセストークン>@github.com/<あなたのGitHubユーザー名>/2D_OpenWorldGame.git
git remote -v
```
10. 5行目を実行したときに出てきたログで、Fork元リポジトリをFork先リモートリポジトリに登録できたか確認する。

### 2. ローカルブランチの新規作成
1. GitBashで以下の2行のコードを実行する。
```Bash:Bash
git checkout -b <新規ブランチ名>
git branch
```
2. 最後の行を実行したときに出てきたログで、作成したブランチ名が存在するか確認する。
     
### 3. Fork元の変更取得・適用
1. GitBashで以下の2行のコードを実行する。

```Bash:Bash
git fetch root_branch
git merge root_branch/<設定済みのブランチ名>
```

### 4. 作成した変更をFork先リモートリポジトリにpush
1. ブランチ作成後は初回のみ、GitBashで以下の3行のコードを実行する。
```Bash:Bash
git add -A
git commit -m "任意の変更点タイトル"
git push origin <設定済みのブランチ名>
```
2. ブランチ作成後の初回以外、GitBashで以下の3行のコードを実行する。
```Bash:Bash
git add -A
git commit -m "任意の変更点タイトル"
git push
```

## シナリオ / ストーリー分岐 制作ガイド (Scenario Writer 向け)

ゲーム内の会話・分岐ロジックは独自 Story System (ScriptableObject ベース) で管理します。プログラムを直接触らなくても、アセットを作るだけで分岐付き会話が構築できます。

### 概要
| 概念 | 説明 |
|------|------|
| StoryGraph | 会話グラフ全体。開始ノード `startNode` を持ち、複数 `StoryNode` を列挙 |
| StoryNode | 1つの台詞ブロック（本文と選択肢） |
| Choice | 選択肢。条件(任意)と遷移先、実行アクション(任意) |
| Condition | 変数ストアを参照し「表示/非表示」を制御する条件 |
| Action | 選択後に変数を書き換えたりフラグを立てる処理 |

### 1. StoryGraph と StoryNode の作り方
1. メニュー `Create > Story > StoryGraph` でグラフを作成 (例: `MainStoryGraph`)
2. メニュー `Story/Story Graph Window` を開く
3. 上部で作成した Graph を指定
4. 「ノード追加」ボタンで `.asset` を作成 (保存ダイアログが開くので `Assets/Story/Nodes/` などに保存推奨)
5. 左一覧からノードを選択し、右ペインで以下を編集
	- ID: 英数字でユニーク (自動生成名でOKだが重複しないこと)
	- 本文: セリフ本文(仮で直接入力。後でローカライズキーにも変更可)
	- 選択肢: ボタンテキストと遷移先を設定
6. 最初のノードを Start に設定（ボタン or `StartNode` フィールド）

### 2. 選択肢で条件分岐を付ける
1. フォルダで `Create > Story > Conditions > IntCompare` などを選択し条件アセットを作る
2. Choice の「条件追加」でそのアセットを指定
3. 例: `trust >= 2` のような比較を設定すると必要値未満では選択肢が表示されません

### 3. フラグ/ステータス操作 (アクション)
1. フォルダで `Create > Story > Actions > SetInt` や `SetBool` を作成
2. Choice の「アクション追加」で指定
3. 選択肢が選ばれた瞬間に値が書き換わります

### 4. プレイ中に表示する (テスト手順)
1. 空の Canvas (UI) をシーンに用意し、縦レイアウトで `BodyText` (UGUI Text) と ボタン置き場 `ChoicesRoot` (空の GameObject) を用意
2. `StoryUI` コンポーネントをアタッチ
	- Graph フィールドに `StoryGraph` を設定
	- Body Text / Choices Root / Choice Button Prefab を割り当て
3. ボタンプレハブ: `Button` + 子に `Text` を持つ一般的なUGUIボタンでOK
4. 再生すると最初のノード本文と選択肢が表示され、クリックで遷移します

#### (任意) 新 Input System 対応
本プロジェクトは `com.unity.inputsystem@1.7.0` を導入済みです。必要に応じて:
1. `Edit > Project Settings > Player > Active Input Handling` を `Both` もしくは `Input System Package (New)` に変更
2. これにより:
	- マウス: そのままクリック
	- キーボード: 矢印キー/Tab でボタンフォーカス移動, Enter / Space で決定
	- ゲームパッド: 十字キー/スティックで移動, A(または決定ボタン) で決定
3. StoryUI は最初の選択肢に自動フォーカスします
 4. 既存シーンで旧 `StandaloneInputModule` による例外が出る場合: メニュー `ストーリー/既存シーンのEventSystemを新InputSystem対応に変換` を実行

#### (任意) ボタン配色を変更したい
`StorySampleCreator` が生成するボタンは濃紺背景+白文字です。独自テーマにしたい場合:
1. サンプル生成後にシーン内の `ChoiceButton` をプレハブ化 (Project にドラッグ)
2. そのプレハブの `Image` 色と `Button > Colors` を調整 (Normal/Highlighted/Pressed)
3. `StoryUI.choiceButtonPrefab` にそのプレハブを再割り当て
4. 既存の自動生成を使いたくない場合は `StoryUI` に生成済みボタンを差し替えるだけでもOK

#### フォント (GenEiAntiqueNv5-M への切替)
サンプル生成/差し替えは以下の優先順位でフォントをロードします:
1. `GenEiAntiqueNv5-M SDF.asset`
2. `GenEiAntiqueNv5-M.ttf`
3. `NotoSansJP-Regular SDF.asset`
4. `NotoSansJP-Regular.ttf`

既存シーンの Text を一括で GenEiAntique に変えたい場合: メニュー `ストーリー/現在シーンのUIフォントを GenEiAntique に差し替え` を実行。

#### 文字がギザギザ / かくかくする場合
1. TextMesh Pro (TMP) を使用しているか確認 (サンプル生成は自動で TMP にしています)
2. TMP フォントアセット (SDF) の Sampling Point Size / Atlas Padding を適正値に調整
3. Canvas の `UI Scale Mode` が `Scale With Screen Size` で、参照解像度と実行解像度が極端に乖離していないか確認
4. フォントを TTF 直読みしている場合は SDF アセットを作成し差し替え
5. 低 DPI 拡縮によるぼやけ → GameView のズーム (Scale) を 1.0x に近付け確認


### 5. 名前や本文を後で差し替えたい (ローカライズ)
現状は文字列を直接入力しています。後で Unity Localization Package を導入する場合:
1. `bodyKey` や `textKey` を テーブルキーに変更 (本文はキー名のまま残す)
2. 取得時にテーブルから解決するラッパを `StoryUI` 内で差し替え

### 6. よくある注意点
| 事象 | 対策 |
|------|------|
| ノードが進まない | 選択肢に遷移先 `next` が未設定か、条件が全て偽 |
| 選択肢が出ない | Condition の値が足りない / Bool が未初期化 |
| StartNode 未設定 | Graph Window 上部で設定 |
| ID 重複 | Graph Window の「ID整列/重複チェック」で検出 |

### 7. 後で追加予定 (拡張アイデア)
- ノード間リンクを線で可視化するグラフビュー
- 条件式を複合で書ける式エディタ
- CSV / Google スプレッドシートから自動インポート
- ログ出力（選択履歴トレース）

### 8. シナリオ提出フロー (ライター向け)
1. 既存 Graph にノード追加 or 新規 Graph 作成 (命名規則例: `Chapter1_Main`)
2. 追加/変更した `.asset` を Git でステージ (コード変更不要)
3. README 冒頭の Git 運用手順どおりにブランチで push / PR 作成

### 9. 変数運用のシンプル規約提案
| 用途 | キー例 | 型 |
|------|--------|----|
| 信頼度 | `trust_<npcName>` | int |
| クエスト受注 | `quest_<id>_accepted` | bool |
| クエスト達成 | `quest_<id>_cleared` | bool |
| 特殊イベントフラグ | `flag_<event>` | bool |

---
質問や拡張要望があれば Issue / PR コメントへ。さらに高度な可視化エディタが必要になったら相談してください。


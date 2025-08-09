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

#～ファミコン時代のムリゲー感が漂う2Dオープンワールドゲーム～


## このリポジトリへの参加方法

1\. リポジトリのコピーとFork元の登録

	1. GitHubリポジトリのWebページで「Fork」ボタンを押す。

	2. 入力画面はそのままにして、「Create fork」ボタンを押す。

    2. リポジトリ内のプロジェクトを保存したいフォルダーでGitBashを立ち上げる。

	3. GitBashで以下の4行のコードを実行する。

```Bash:Bash

git clone https://github.com/<あなたのGitHubユーザー名>/2D\_OpenWorldGame.git

cd 2D\_OpenWorldGame

git remote add root\_branch https://github.com/neginegi88586/2D\_OpenWorldGame.git

git remote -v

```

    4. 最後の行を実行したときに出てきたログで、Fork元リポジトリをFork先リモートリポジトリに登録できたか確認する。

2\. ローカルブランチの新規作成

	1. GitBashで以下の2行のコードを実行する。

```Bash:Bash

git checkout -b <新規ブランチ名>

git branch

```

    2. 最後の行を実行したときに出てきたログで、作成したブランチ名が存在するか確認する。

3\. Fork元の変更取得・適用

    1. GitBashで以下の2行のコードを実行する。

```Bash:Bash

git fetch root\_branch

git merge root\_branch/<設定済みのブランチ名>

```

4\. 作成した変更をFork先リモートリポジトリにpush

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


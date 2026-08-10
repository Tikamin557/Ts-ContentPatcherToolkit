============================================================
LANGUAGE / 言語について
============================================================

This README is provided in both English and Japanese.
Japanese users can skip the English section by scrolling down.
A full Japanese version of the README is available in the second
half of this file.

このREADMEは英語と日本語の両方で記載されています。
日本語ユーザーの方は、英語部分をスクロールして飛ばしてください。
後半に日本語で書かれた説明があります。


============================================================
T's Content Patcher Toolkit
Version 1.1.0
============================================================

A Windows toolkit designed to make working with Content Patcher
mod files easier.

T's Content Patcher Toolkit provides useful tools for Content
Patcher modding.

More tools may be added in future versions.


------------------------------------------------------------
FEATURES
------------------------------------------------------------

[JSON Indentation]

Clean up the indentation of Content Patcher JSON files without
unnecessarily changing how the file is written.

Unlike a general JSON formatter, this tool does not rearrange your
existing line breaks. It only adjusts the indentation at the
beginning of each line.

For example, if you intentionally write something on a single line
like this:

    "Position": { "X": 10, "Y": 20 }

it will remain on a single line.

Likewise, if you intentionally write the same structure across
multiple lines:

    "Position": {
        "X": 10,
        "Y": 20
    }

it will remain across multiple lines.

This means you can keep the layout you chose for your Content Patcher
files while using the tool to clean up inconsistent or broken
indentation.

The contents of each line are preserved.

Multiline values are also kept on separate lines.

For example:

"Target": "Maps/Example_A,
           Maps/Example_B,
           Maps/Example_C"

The line breaks are preserved, while the continued lines are aligned
with the beginning of the value.

It also supports JSON files containing comments, which are commonly
used in Content Patcher packs.


[Single File]

Select a JSON file and clean up its indentation.

You can also drag and drop a JSON file directly onto the application
window.


[Batch Processing]

Process multiple JSON files in a folder at once.

You can optionally include JSON files inside subfolders, making this
useful for larger Content Patcher packs.


[Backup]

You can enable automatic backup creation before files are modified.

Keeping this enabled is recommended when processing important mod
files.


[Indentation Size]

You can choose the number of spaces used for each indentation level:

    2 spaces
    3 spaces
    4 spaces
    8 spaces


[Languages]

The application supports:

    English
    Japanese

The selected language is saved and restored the next time the
application is started.


------------------------------------------------------------
INSTALLATION
------------------------------------------------------------

No installation is required.

1. Download the ZIP file.
2. Extract it to any folder.
3. Run TsCPToolKit.exe.

The application is distributed as a standalone Windows executable.


------------------------------------------------------------
HOW TO USE
------------------------------------------------------------

[Process a Single JSON File]

1. Open the "Single JSON" tab.

2. Select a JSON file using the browse button, or drag and drop a
   JSON file onto the application.

3. Choose the indentation size.

4. Choose whether to create a backup.

5. Run the indentation process.


[Process a Folder]

1. Open the "Batch" tab.

2. Select the folder containing your JSON files, or drag and drop
   the folder onto the application.

3. Enable or disable processing of subfolders as needed.

4. Choose the indentation size.

5. Choose whether to create backups.

6. Run the batch process.


------------------------------------------------------------
IMPORTANT NOTES
------------------------------------------------------------

This tool is intended to help organize Content Patcher JSON files.

It does not validate or repair incorrect Content Patcher patches or
JSON syntax.

It also does not intentionally rewrite your JSON into a different
style.

Existing line breaks and one-line structures are preserved, with the
tool focusing on the indentation at the beginning of each line.

When working with important files, keeping the backup option enabled
is recommended.


------------------------------------------------------------
APPLICATION SETTINGS
------------------------------------------------------------

Application settings are saved in:

    Documents
    └─ T's Content Patcher Toolkit
       └─ config.json

Deleting config.json will reset the saved application settings.


------------------------------------------------------------
VERSION
------------------------------------------------------------

T's Content Patcher Toolkit
Version 1.1.0



============================================================
日本語
============================================================

T's Content Patcher Toolkit は、Content PatcherのMod制作を
補助するためのWindows用ツールです。

Content PatcherのModファイルを扱いやすくするための機能を
提供します。

今後のバージョンでは、さらに別の機能を追加する可能性があります。


------------------------------------------------------------
機能
------------------------------------------------------------

【JSONのインデント整理】

Content PatcherのJSONファイルの書き方を必要以上に変更せず、
インデントを整理します。

一般的なJSONフォーマッターとは異なり、この機能では既存の
改行位置を変更しません。
各行の先頭にあるインデントだけを整理します。

例えば、意図的に次のように1行で記述している部分は、

    "Position": { "X": 10, "Y": 20 }

整理後も1行のまま維持されます。

同様に、次のように複数行で記述している部分は、

    "Position": {
        "X": 10,
        "Y": 20
    }

整理後も複数行のまま維持されます。

そのため、Content Patcherのファイルを自分で読みやすいように
記述したレイアウトはそのまま維持しながら、崩れたインデントや
不揃いなインデントだけを整理できます。

各行の内容もそのまま維持されます。

複数行に分けて記述している値も、そのまま維持されます。

例えば、

"Target": "Maps/Example_A,
           Maps/Example_B,
           Maps/Example_C"

のように記述している場合、改行はそのまま維持しながら、
2行目以降を値の開始位置に揃えて整理します。

Content Patcherのファイルでよく使用される、
コメントを含むJSONにも対応しています。


【単一ファイル】

JSONファイルを1つ選択してインデントを整理できます。

JSONファイルをアプリのウィンドウへ直接ドラッグ＆ドロップ
することもできます。


【フォルダ一括処理】

フォルダ内にある複数のJSONファイルをまとめて処理できます。

サブフォルダ内のJSONファイルも処理対象にできるため、
ファイル数の多いContent Patcher Modにも利用できます。


【バックアップ】

ファイルを変更する前に、自動でバックアップを作成する
設定があります。

大切なModファイルを処理する場合は、バックアップを
有効にしておくことをおすすめします。


【インデント幅】

1段あたりのスペース数を次の中から選択できます。

    2スペース
    3スペース
    4スペース
    8スペース


【言語】

アプリは次の言語に対応しています。

    English
    日本語

選択した言語は保存され、次回起動時にも引き継がれます。


------------------------------------------------------------
インストール
------------------------------------------------------------

インストール作業は必要ありません。

1. ZIPファイルをダウンロードします。

2. 任意の場所へ解凍します。

3. TsCPToolKit.exe を起動します。

単体で起動できるWindowsアプリとして配布されています。


------------------------------------------------------------
使い方
------------------------------------------------------------

【JSONファイルを1つ処理する】

1. 「単一JSON」タブを開きます。

2. 参照ボタンからJSONファイルを選択するか、
   アプリへJSONファイルをドラッグ＆ドロップします。

3. インデント幅を選択します。

4. バックアップを作成するか選択します。

5. インデント整理を実行します。


【フォルダをまとめて処理する】

1. 「フォルダ一括」タブを開きます。

2. JSONファイルが入っているフォルダを選択するか、
   アプリへフォルダをドラッグ＆ドロップします。

3. 必要に応じてサブフォルダも処理対象にします。

4. インデント幅を選択します。

5. バックアップを作成するか選択します。

6. 一括処理を実行します。


------------------------------------------------------------
注意事項
------------------------------------------------------------

このツールはContent PatcherのJSONファイルを整理しやすくする
ためのものです。

間違ったContent PatcherのパッチやJSONの構文を検証・修正する
機能ではありません。

また、JSON全体を別の書式へ書き換えることを目的としていません。

既存の改行位置や1行で記述した部分などは維持し、
各行の先頭にあるインデントを中心に整理します。

大切なファイルを処理する場合は、バックアップを
有効にしておくことをおすすめします。


------------------------------------------------------------
アプリ設定
------------------------------------------------------------

アプリの設定は次の場所に保存されます。

    ドキュメント
    └─ T's Content Patcher Toolkit
       └─ config.json

config.json を削除すると、保存されているアプリ設定を
初期状態に戻すことができます。


------------------------------------------------------------
バージョン
------------------------------------------------------------

T's Content Patcher Toolkit
Version 1.1.0
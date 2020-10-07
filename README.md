# TrpgDiceBot
## これはなに？
<p>Discord のダイス Bot です。</p>
<p>ローカル環境で動作させることを前提で作っているので、DL して起動させる必要があります (多重起動するとどうなるのかはまだ未検証です)。</p>

## DL するもの

<p>現在準備中</p>

## 使い方
<ul>
<li>
先頭に "&" が必要です。
例：”& 1d10”, & “1d6-1d3” など
また、"&p <数字>" でCoCでよくやる1d100が振れます。成功か失敗かも表示されます。
</li>

<li>"&p" でとりあえず 1d100 を振ります</li>
<li>万が一、送信可能な文字数を超えた場合は、送る予定だった文字列がコンソール画面に表示されます。</li>
</ul>
<p>※タイミングにより出目が荒ぶることがありますが、仕様です。<br>
あまりに酷いと感じた場合はお申し付けください。</p>


## 今後更新予定
<ul>
<li>”&p 50+10” などの入力に対応したい。</li>
</ul>

## 更新履歴
### ver.1.02 (2020/09/24)
<ul>
<li>コマンド、&help, %help, %version(ver) を追加</li>
<li>バージョンを Bot 側で表示できるようにした</li>
</ul>

### ver.1.01 (2020/09/11)
<ul>
<li>”&p” で 1d100 を振れるようにした</li>
<li>送信可能な文字数を超えた場合は、コンソール画面に送る予定だった文字列を表示するようにした</li>
</ul>

### ver.1.00 (2020/09/10)
<ul>
<li>とりあえず完成したと判断したもの</li>
</ul>
Owin SelfHostでWebSocketのChat作ってみた。
==========================================

Owin + SelfHost + WebSocketをLinux+monoで動かして見たかったテスト

でも今のところWebSocketがWindowsしか動かない


原因
======

https://github.com/mono/mono/blob/master/mcs/class/System/System.Net/HttpListenerRequest.cs

これのIsWebSocketRequestが未実装

Katanaはこれを使ってWebSocketかどうか判定しているため、WebSocketでないと見做し、通常のHTTPレスポンスを返してしまう。


メモ
====

Katanaのtestをmono3.2で走らせないとまずそうだねぇ。
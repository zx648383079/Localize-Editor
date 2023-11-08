# Localize Editor
 suport json、xlf、resw、resx、po、mo etc


## 已实现功能

1. json 文件的读写
2. xlf 文件的读写
3. resx 文件的读写
4. po 文件的读写
5. mo 文件的读写
6. PHP 文件读写
7. 部分字幕、歌词（srt,lrc）文件读写
8. 支持mysql数据库读写
9. 翻译API 配置及调用，单个翻译，支持主流翻译API：Bing、Google、DeepL、百度、有道、阿里巴巴
10. 支持内嵌浏览器免费翻译


## 待实现功能

1. 全部翻译
2. 从文件夹中提取需要翻译的文本
3. 有些文件的语言是写在文件路径里的，例如：message.en.json, en.json, en/message.json  这类语言是否需要指定提取？
4. 有些文件是只有 id 和 对应的 翻译, 是否需要 自动适配并保留显示到下一个语言包中，

例如： en.json, 'id' => 'it is wrong'; 

zh.json 'id' => '出错了', 

需要显示成 
|源: en| 翻译: zh| Id|
|:---|:---|:---|
|it is wrong|出错了|id|
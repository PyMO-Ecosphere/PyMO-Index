# PyMO-Index

PyMO游戏索引，用于索引所有的PyMO游戏。

## 索引源表`sources.json`

用于记录索引源，指向多个游戏索引表。该文件为一个数组，每个数组项表示一个索引源。

其中每个对象的字段代表的含义如下：

字段名称 | 数据类型 | 可空 | 含义
------ | -------- | --- | ------
name   | string   | 否  | 索引源名称
id     | string   | 否  | 索引源唯一标识符
url    | string   | 是  | 索引源json链接
screenshot_baseurl | string | 是 | 索引源游戏截图链接前缀
local_path | string | 是 | 索引源json本地路径，当可以在本地找到索引源时优先使用本地索引源以进行调试
desc   | string   | 否  | 对索引源的说明

## 游戏索引表`gamedb.json`

该文件为一个数组，数组内的每个对象都对应一个游戏的元数据。

其中每个对象的字段代表的含义如下：

字段名称       | 数据类型       | 可空 | 含义
------------- | ------------ | --- | ------
author        | string       | 是   | 移植者
baidu_folder  | string       | 否   | 游戏唯一标识符
contains_h    | bool         | 否   | 是否包含R18内容
download_link | string       | 是   | 下载链接（指向一个网页）
download_pass | string       | 是   | 上述下载链接的提取码
folder        | string       | 是   | 仅用于兼容官方的游戏索引
game_id       | int          | 是   | 仅用于兼容官方的游戏索引
introduction  | string       | 是   | 游戏简介
platforms     | string array | 否，且不可为空列表   | 下载包中包含的平台，可能的值为`android`、`symbian`、`psp`、`3ds`、`wii`
publish_date  | date         | 是   | 发布日期
publish_site  | string       | 是   | 发布帖
screenshots   | string array | 否，但可为空列表   | 游戏截图列表
title         | string       | 否   | 游戏标题
unzip_pass    | string       | 是   | 解压密码

## 各脚本工具

脚本工具使用F#语言编写，可以安装.NET SDK后使用`dotnet fsi <fsx文件>`命令来执行这些脚本。

### `pymo-index-utils.fsx`

该文件用于解析和处理游戏索引，可利用该脚本编写处理游戏索引的程序。

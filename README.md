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

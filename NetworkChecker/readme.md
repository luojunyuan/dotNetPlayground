### .Net でネットワークをチェックする三つの方法

1. 命名空间 `System.Net.NetworkInformation.NetworkInterface`，这里面提供的方法耗时长，且总是返回True。

2. WIN32 API, 笔记本下测试，仅在关闭wifi开关后，才返回False

3. COM 组件，比较精准的获取网络是否能链接互联网，且反应最快消耗小。

命名空间 `System.Net.NetworkInformation.NetworkInterface` 下还有一个网路断开的事件，仅在wifi开关切换时触发事件。

### .Net でネットワークをチェックする三つの方法

1. 命名空间 `System.Net.NetworkInformation.NetworkInterface`，这里面提供的方法耗时长，且总是返回True。

2. WIN32 API, ~~笔记本下测试，仅在关闭wifi开关后，才返回False~~

    * InternetGetConnectedState() (文档不推荐)
    * INetworkListManager.GetConnectivity() 

3. COM 组件，比较精准的获取网络是否能链接互联网，且反应最快消耗小。(但是引入COM组件 .Net Core却不受支持) (注：NetListMgr在win32 api中也提供了)

4. 通用的ping的方法，效果比第一种差一点

命名空间 `System.Net.NetworkInformation.NetworkInterface` 下还有一个网路断开的事件，仅在wifi开关切换时触发事件。

链接互联网时输出

```cmd
System.Net.NetworkInformation module     (True, 135ms)
WIN32 Api                                (True, 12ms)
COM Component                            (True, 8ms)
Ping                                     (True, 84ms)
```

断开wifi后输出

```cmd
System.Net.NetworkInformation module     (True, 130ms)
WIN32 Api                                (False, 11ms)
COM Component                            (False, 8ms)
Ping                                     (False, 82ms)
```

关闭Wifi开关后

```cmd
System.Net.NetworkInformation module     (True, 133ms)
WIN32 Api                                (False, 11ms)
COM Component                            (False, 8ms)
Ping                                     (False, 87ms)
```

Ping 无法连接到网络时都会引发异常

`引发的异常:“System.Net.Sockets.SocketException”(位于 System.Net.NameResolution.dll 中)`

`引发的异常:“System.Net.NetworkInformation.PingException”(位于 System.Net.Ping.dll 中)`

> 本次测试 `System.Net.NetworkInformation` 下的组件始终没有变化

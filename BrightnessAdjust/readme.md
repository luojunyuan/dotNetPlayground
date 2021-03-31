### CSharp 三种方式修改windows屏幕亮度

---

1. 使用 Dxva2.dll, 详细看 class AdjustScreenByDxva2()

Dxva的效果是调整显示器内置亮度，一般是支持显示器，所以笔记本可能就不支持这种方法。

2. 使用 gdi32.dll，详情看 class AdjustScreenByGdi32()

使用这个方法修改亮度实际上是在调整伽马值，虽然调整伽马会导致色彩偏，但确实是降低了亮度。这似乎属于英特尔驱动色彩管理，理应基本所有电脑都支持这种方法把。

我在dotnet5 下VS提示我需要引入 System.Drawing.Common 5.0.x 这个包

3. 使用WMI，System.Management 模块

这种方法调整亮度，和笔记本、win平板上自带的亮度调整是一摸一样的，相当于改系统设置里的亮度，调整亮度时左上角会有亮度弹窗。所以只有显示器的主机设备应该是不支持的。

这种方法 + 第二种方法可以让屏幕亮度做到很低。

同上，需要引入 System.Management

### 注，须知：

虽然林佬推荐首选Dxva2，但是使用Dxva2可能对杂牌显示器带来危害（比如低端凡硕），乱调整可能导致显示器直接不发光，怎么救都救不回来。只能用代码再改回来。

``` csharp
// XXX: Do not use Dxva2, may cause damage to the monitor. `cur` gets 100 first time, set `min`, `max` 
// between 22-85 is fine for one of my monitor, out of the range it's not glow anymore.
```

当wpf程序代码访问WMI模块后，触控会升级到和uwp相似的效果，这个时候那些Scroll 滑条都无法触摸操作了。

### Reference
> [WPF 修改屏幕亮度 (lindexi.com)](https://blog.lindexi.com/post/WPF-%E4%BF%AE%E6%94%B9%E5%B1%8F%E5%B9%95%E4%BA%AE%E5%BA%A6.html)
> 
> wmi 部分代码参考出处我忘记哪个仓库了

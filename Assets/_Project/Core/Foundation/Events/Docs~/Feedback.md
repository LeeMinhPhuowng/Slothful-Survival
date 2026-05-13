**1. Namespace.**
Thiếu namespace: `Core.Foundation.Events`

**2. Reentrancy protection.**
`_isRunning` là cơ chế tốt, nhưng đang chặn recursive calls một cách lặng lẽ, nên bổ sung log (dùng hệ thống Logger của Nhi).

**3. Exception handling.**
```
try
{
    OnRaised?.Invoke();
}
finally
{
    _isRunning = false;
}
```

Lời gọi Invoke sẽ nuốt exception stacktrace. Cần chuyển sang sử dụng method GetInvocationList() và xử lý tuần tự từng listener (handle exception, log, ...).

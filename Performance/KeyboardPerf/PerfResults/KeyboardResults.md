## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

## **Velaptor Stats (Baseline)**
**Velaptor Version:** v1.0.0-preview.30  
**Description**: Baseline before changes for v1.0.0-preview.31

| Method                                  | Mean     | Error     | StdDev    | Median   | Gen0   | Gen1   | Allocated |
|---------------------------------------- |---------:|----------:|----------:|---------:|-------:|-------:|----------:|
| KeyboardState.IsKeyDown                 | 1.965 us | 0.0495 us | 0.1459 us | 2.002 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsKeyUp                   | 1.971 us | 0.0389 us | 0.0785 us | 1.984 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.SetKeyState               | 2.042 us | 0.0397 us | 0.0641 us | 2.054 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.KeyToChar                 | 1.980 us | 0.0582 us | 0.1661 us | 2.019 us | 0.6733 | 0.0153 |   8.25 KB |
| KeyboardState.GetDownKeys               | 2.136 us | 0.0427 us | 0.1162 us | 2.190 us | 0.6714 | 0.0114 |   8.23 KB |
| KeyboardState.AnyAltKeysDown            | 2.054 us | 0.1041 us | 0.3053 us | 2.155 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.AnyCtrlKeysDown           | 2.100 us | 0.0478 us | 0.1341 us | 2.126 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.AnyShiftKeysDown          | 2.571 us | 0.3406 us | 1.0043 us | 2.007 us | 0.6676 | 0.0076 |    8.2 KB |
| KeyboardState.AnyNumpadNumberKeysDown   | 2.061 us | 0.0474 us | 0.1398 us | 2.055 us | 0.6714 | 0.0095 |   8.23 KB |
| KeyboardState.AnyStandardNumberKeysDown | 1.985 us | 0.0714 us | 0.2061 us | 2.036 us | 0.6714 | 0.0114 |   8.23 KB |
| KeyboardState.IsLeftAltKeyDown          | 1.684 us | 0.1207 us | 0.3559 us | 1.887 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsLeftCtrlKeyDown         | 1.969 us | 0.0560 us | 0.1634 us | 1.986 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsLeftShiftKeyDown        | 1.940 us | 0.0394 us | 0.1156 us | 1.974 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsRightAltKeyDown         | 1.894 us | 0.0889 us | 0.2606 us | 1.958 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsRightCtrlKeyDown        | 1.966 us | 0.0477 us | 0.1376 us | 1.991 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsRightShiftKeyDown       | 1.933 us | 0.0551 us | 0.1626 us | 1.974 us | 0.6695 | 0.0095 |    8.2 KB |
| Keyboard.GetState                       | 3.230 us | 0.0642 us | 0.0713 us | 3.255 us | 0.6695 | 0.0095 |    8.2 KB |

---

## **Velaptor Stats (Test 3)**
**Velaptor Version:** v1.0.0-preview.31  
**Description:** After improvements

| Method                                  | Mean       | Error     | StdDev    | Gen0   | Allocated |
|---------------------------------------- |-----------:|----------:|----------:|-------:|----------:|
| KeyboardState.IsKeyDown                 |   2.926 ns | 0.0437 ns | 0.0387 ns |      - |         - |
| KeyboardState.IsKeyUp                   |   2.570 ns | 0.0749 ns | 0.0700 ns |      - |         - |
| KeyboardState.SetKeyState               |   3.417 ns | 0.0633 ns | 0.0592 ns |      - |         - |
| KeyboardState.KeyToChar                 |   7.785 ns | 0.1554 ns | 0.1453 ns |      - |         - |
| KeyboardState.GetDownKeys               | 128.989 ns | 1.9226 ns | 1.7984 ns | 0.0081 |     104 B |
| KeyboardState.AnyAltKeysDown            |   4.244 ns | 0.0506 ns | 0.0474 ns |      - |         - |
| KeyboardState.AnyCtrlKeysDown           |   4.240 ns | 0.0726 ns | 0.0679 ns |      - |         - |
| KeyboardState.AnyShiftKeysDown          |   4.302 ns | 0.0890 ns | 0.0832 ns |      - |         - |
| KeyboardState.AnyNumpadNumberKeysDown   |  17.443 ns | 0.1966 ns | 0.1743 ns |      - |         - |
| KeyboardState.AnyStandardNumberKeysDown |  21.220 ns | 0.2831 ns | 0.2509 ns |      - |         - |
| KeyboardState.IsLeftAltKeyDown          |   2.119 ns | 0.0225 ns | 0.0199 ns |      - |         - |
| KeyboardState.IsLeftCtrlKeyDown         |   2.161 ns | 0.0445 ns | 0.0416 ns |      - |         - |
| KeyboardState.IsLeftShiftKeyDown        |   2.128 ns | 0.0347 ns | 0.0324 ns |      - |         - |
| KeyboardState.IsRightAltKeyDown         |   2.140 ns | 0.0364 ns | 0.0341 ns |      - |         - |
| KeyboardState.IsRightCtrlKeyDown        |   2.210 ns | 0.0478 ns | 0.0447 ns |      - |         - |
| KeyboardState.IsRightShiftKeyDown       |   2.188 ns | 0.0384 ns | 0.0341 ns |      - |         - |
| Keyboard.GetState                       | 591.791 ns | 9.5462 ns | 8.9295 ns | 0.2193 |    2752 B |

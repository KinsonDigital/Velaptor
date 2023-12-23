## **Velaptor Stats (Baseline)**
**Velaptor Version:** v1.0.0-preview.30  
**Description**: Baseline before changes for v1.0.0-preview.31

## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


| Method                                  | Mean     | Error     | StdDev    | Median   | Gen0   | Gen1   | Allocated |
|---------------------------------------- |---------:|----------:|----------:|---------:|-------:|-------:|----------:|
| KeyboardState.IsKeyDown                 | 1.965 us | 0.0495 us | 0.1459 us | 2.002 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.IsKeyUp                   | 1.971 us | 0.0389 us | 0.0785 us | 1.984 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.SetKeyState               | 2.042 us | 0.0397 us | 0.0641 us | 2.054 us | 0.6695 | 0.0095 |    8.2 KB |
| KeyboardState.AnyKeysDown               | 2.164 us | 0.0584 us | 0.1704 us | 2.194 us | 0.6733 | 0.0153 |   8.25 KB |
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
**Description:** Changes from test 2 is that KeyboardKeyGroups uses frozen dictionaries and immutable lists.

## **Stats**:
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
[Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

| Method                                  | Mean       | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
| KeyboardState.IsKeyDown                 |   2.344 ns | 0.0202 ns | 0.0179 ns |      - |      - |         - |
| KeyboardState.IsKeyUp                   |   2.687 ns | 0.0709 ns | 0.0663 ns |      - |      - |         - |
| KeyboardState.SetKeyState               |   3.246 ns | 0.0321 ns | 0.0285 ns |      - |      - |         - |
| KeyboardState.AnyKeysDown               |   49.99 ns |  0.457 ns |  0.428 ns |      - |      - |         - |
| KeyboardState.KeyToChar                 |   7.539 ns | 0.1729 ns | 0.1617 ns |      - |      - |         - |
| KeyboardState.GetDownKeys               |   98.26 ns |  0.790 ns |  0.660 ns | 0.0025 |      - |      32 B |
| KeyboardState.AnyAltKeysDown            |   4.144 ns | 0.0307 ns | 0.0273 ns |      - |      - |         - |
| KeyboardState.AnyCtrlKeysDown           |   4.144 ns | 0.0481 ns | 0.0450 ns |      - |      - |         - |
| KeyboardState.AnyShiftKeysDown          |   4.212 ns | 0.0414 ns | 0.0387 ns |      - |      - |         - |
| KeyboardState.AnyNumpadNumberKeysDown   |  31.248 ns | 0.5581 ns | 0.5220 ns | 0.0051 |      - |      64 B |
| KeyboardState.AnyStandardNumberKeysDown |  33.310 ns | 0.6660 ns | 0.6230 ns | 0.0051 |      - |      64 B |
| KeyboardState.IsLeftAltKeyDown          |   1.658 ns | 0.0223 ns | 0.0208 ns |      - |      - |         - |
| KeyboardState.IsLeftCtrlKeyDown         |   1.703 ns | 0.0428 ns | 0.0400 ns |      - |      - |         - |
| KeyboardState.IsLeftShiftKeyDown        |   1.733 ns | 0.0519 ns | 0.0711 ns |      - |      - |         - |
| KeyboardState.IsRightAltKeyDown         |   1.697 ns | 0.0195 ns | 0.0182 ns |      - |      - |         - |
| KeyboardState.IsRightCtrlKeyDown        |   1.663 ns | 0.0155 ns | 0.0137 ns |      - |      - |         - |
| KeyboardState.IsRightShiftKeyDown       |   1.689 ns | 0.0287 ns | 0.0268 ns |      - |      - |         - |
| Keyboard.GetState                       |   603.3 ns |  11.83 ns |  11.62 ns | 0.2193 |      - |   2.69 KB |

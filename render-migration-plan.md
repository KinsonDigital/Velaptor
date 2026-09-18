# Velaptor OpenGL → WebGPU Migration Plan

## Problem Statement

Velaptor currently uses OpenGL (via Silk.NET) as its sole rendering backend. A prototype WebGPU project (`csharp-webgpu`) has been created to learn WebGPU fundamentals. The goal is to migrate Velaptor's rendering layer from OpenGL to WebGPU while minimizing impact on the public API and considering whether to replace OpenGL entirely or support dual backends.

---

## Phase 0: Analysis Summary

### Current OpenGL Architecture

```
┌───────────────────────────────────────────────────────────────┐
│  PUBLIC API (no GL concepts)                                  │
│  - Window (abstract, inherited by user game class)            │
│  - IBatcher { Begin(), Clear(), End(), ClearColor }           │
│  - ITextureRenderer / IFontRenderer / IShapeRenderer / ILineRenderer │
│  - IContentManager.Load<T>() / IFont / ITexture / IAudio     │
│  - IScene / ISceneManager / IUpdatable / IDrawable            │
│  - IAppInput<KeyboardState> / IAppInput<MouseState>          │
├───────────────────────────────────────────────────────────────┤
│  INTERNAL OPENGL LAYER                                        │
│  GLWindow ─── Silk.NET IWindow (GLFW) — OpenGL context       │
│       ├── IGLInvoker (47 methods: 1:1 GL calls)              │
│       ├── IOpenGLService (bind tracking, debug labeling)      │
│       ├── ShaderProgram (4 shader pairs: tex/font/shape/line)│
│       ├── GpuBufferBase<T> → 4 GPU buffers (VAO/VBO/EBO)     │
│       ├── 4 Renderers (Texture/Font/Shape/Line)              │
│       ├── Batcher → BatchingManager → RenderMediator          │
│       └── Texture/Font loader (TexImage2D upload)             │
└───────────────────────────────────────────────────────────────┘
```

### OpenGL-Specific Code by Layer

| Layer | Files | GL Dependencies | Complexity |
|-------|-------|----------------|------------|
| IGLInvoker + GLInvoker | 2 files | 47 methods, 21 GL enums | High |
| IOpenGLService + OpenGLService | 2 files | 24 methods, OpenGLBufferType | Medium |
| Shader Programs (4 shaders) | 4 files | GLSL 450, CreateShader/LinkProgram | Medium |
| GPU Buffers (4 types) | 5 files (base + 4) | VAO/VBO/EBO, BufferData/SubData | High |
| GPU Data structs (4 types) | 4 files | Vertex attrib layouts matching GLSL | Medium |
| Batch Items (4 types) | 4 files | GPU-agnostic but in GL namespace | Low |
| GLWindow | 1 file | OpenGL context, SwapBuffers, Viewport | High |
| GLFW Interop | 4 files | Monitor/display queries only | Low |
| Texture | 1 file | GenTexture, TexImage2D | Medium |
| GL enums | 1 file | 21+ GL constant enums | Low |
| **Total** | **~26 files** | | |

### csharp-webgpu Prototype Architecture

```
┌───────────────────────────────────────────────────────────────┐
│  PROTOTYPE: Single-Sprite WebGPU Demo                         │
│                                                               │
│  Silk.NET IWindow (GraphicsAPI.None — no GL context)          │
│       │                                                       │
│  GraphicsDevice (Instance → Adapter → Device → Queue)         │
│  GraphicsSurface (window surface + swap chain)                │
│  GraphicsShader (WGSL loader with {HW}/{HH} substitution)     │
│  GraphicsPipeline (pipelines + bind group layouts)            │
│  GraphicsTexture (PNG → QueueWriteTexture → view + sampler)   │
│  Camera2D (uniform buffer → bind group)                       │
│  Frame (per-frame: encoder → render pass → submit → present)  │
│                                                               │
│  Key limitations for Velaptor:                                │
│  - No vertex buffers (hardcoded shader positions)             │
│  - No batching (single draw call)                             │
│  - No indexed drawing (6 vertices, 2 shared)                  │
│  - Single WGSL shader (no shape/line/font shaders)            │
│  - No font rendering pipeline                                 │
└───────────────────────────────────────────────────────────────┘
```

### Public API Compatibility Assessment

| Public API Type | OpenGL Leak? | Needs Change? |
|----------------|--------------|---------------|
| `Window` abstract class | No | No |
| `IBatcher` | No | No |
| `ITextureRenderer` / `IFontRenderer` / etc. | No | No |
| `IContentManager` | No | No |
| `ITexture { uint Id, Width, Height }` | **Yes** — `Id` is GL handle | **Minor** — Id is just opaque handle |
| `IFont { ITexture Atlas }` | Propagates from ITexture | No (if ITexture unchanged) |
| `IUpdatable` / `IDrawable` | No | No |
| `IScene` / `ISceneManager` | No | No |
| `Input` types | No | No |
| Shape/Circle/Line structs | No | No |
| `FrameTime` / `SizeU` | No | No |

**Conclusion: The public API can remain 95%+ unchanged.** The only potential change is `ITexture.Id` which currently exposes an OpenGL texture handle. This is used as an opaque identifier by renderers for texture batching — a WebGPU implementation would use a different identifier type, but as a `uint` it can still hold a WebGPU texture handle or an internal index.

---

## Phase 1: Decision — Single Backend vs. Dual Backend

### Option A: Replace OpenGL Entirely with WebGPU

**Effort**: Medium-High (refactor all internals once)  
**Maintenance**: Low (single backend to maintain)  
**Risk**: Medium (must get WebGPU feature parity on day one of migration)  
**Public API**: No changes needed  
**Binary size**: Smaller (one backend)  
**Compile time**: Faster  
**Ideal for**: Clean break, new renderer from scratch  

**Verdict**: ✅ **RECOMMENDED for initial release.** Removes all GL baggage, simplifies architecture, and avoids the combinatorial complexity of dual-backend testing.

**BUT**: Cannot ship until ALL rendering features (texture, font, shape, line) work on WebGPU.

### Option B: Support Both Backends (OpenGL + WebGPU)

**Effort**: High (design abstract backend interface, implement both)  
**Maintenance**: High (two backends to maintain and test)  
**Risk**: Low (OpenGL always works as fallback)  
**Public API**: No changes needed  
**Binary size**: Larger (two backends)  
**Compile time**: Slower (platform-specific compilation)  
**Ideal for**: Transitional period, or supporting older hardware  

**Verdict**: ❌ **NOT recommended for initial implementation.** The abstraction layer adds significant complexity. However, if the codebase is structured *as if* dual backend is possible (clean interface boundaries), the option remains open.

### Recommendation: Phase the Migration

```
Phase 1: Refactor internal architecture for backend abstraction
  → Create IRenderBackend abstraction
  → Extract GL code behind it (no behavior change)
  → Public API stays identical

Phase 2: Implement WebGPU backend
  → Write WebGPU version of each abstraction
  → Feature-parity with GL backend (texture, font, shape, line)

Phase 3: Switch default to WebGPU, keep GL as fallback
  → Backend selection via config/constructor
  → Retire GL backend after stability period
```

---

## Phase 2: Proposed New Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│  PUBLIC API (UNCHANGED)                                          │
│  Window / IBatcher / IRenderers / IContentManager / ITexture...  │
├──────────────────────────────────────────────────────────────────┤
│  INTERNAL BATCHING LAYER (UNCHANGED)                             │
│  BatchingManager / RenderMediator / Reactable Notifications      │
├──────────────────────────────────────────────────────────────────┤
│  ABSTRACT BACKEND INTERFACE (NEW)                                │
│                                                                  │
│  IGraphicsBackend ─────────────────────────────────────────┐     │
│  ├── CreateTexture(uint width, uint height, byte[] data)   │     │
│  ├── CreateShader(ShaderType type, string[] sources)       │     │
│  ├── BeginFrame() / EndFrame()                             │     │
│  ├── RenderBatch<T>(T[] items) where T : struct            │     │
│  └── Dispose()                                             │     │
│                                                            │     │
│  IRenderer<TBatchItem> ──────────────────────────────┐     │     │
│  ├── BatchType                                       │     │     │
│  ├── Render(TBatchItem[] items)                      │     │     │
│  └── OpenGL version: IGLInvoker + GpuBufferBase      │     │     │
│     WebGPU version: WebGpuBuffer + BindGroups        │     │     │
├──────────────────────────────────────────────────────┴─────┴─────┤
│  OPENGL BACKEND (EXISTING, REFACTORED)                           │
│  GlfwBackend         ─ window, display queries, no GL context    │
│  OpenGLBackend       ─ implements IGraphicsBackend               │
│  ├── OpenGLTexture    ─ wraps GenTexture/TexImage2D              │
│  ├── OpenGLBuffer{4}  ─ wraps VAO/VBO/EBO logic                  │
│  └── OpenGLShaders{4} ─ GLSL 450 shader compilation              │
├──────────────────────────────────────────────────────────────────┤
│  WEBGPU BACKEND (NEW)                                            │
│  WebGPUBackend        ─ implements IGraphicsBackend              │
│  ├── WebGPUDevice      ─ Instance → Adapter → Device → Queue     │
│  ├── WebGPUWindow      ─ Surface + swapchain + Frame pattern     │
│  ├── WebGPUTexture     ─ QueueWriteTexture + view + sampler      │
│  ├── WebGPUBuffer{4}   ─ vertex/index buffers, bind groups       │
│  └── WebGPUShaders{4}  ─ WGSL vertex/fragment shader compilation │
└──────────────────────────────────────────────────────────────────┘
```

### What the Abstraction Covers

| OpenGL Concept | WebGPU Equivalent | Abstraction |
|---------------|-------------------|-------------|
| `GL.CreateTexture()` / `TexImage2D` | `DeviceCreateTexture` / `QueueWriteTexture` | `ITextureHandle` |
| `GL.CreateBuffer()` / `BufferData` | `DeviceCreateBuffer` / `QueueWriteBuffer` | `IGpuBuffer<T>` |
| `GL.CreateShader()` / `CompileShader` | `DeviceCreateShaderModule` (WGSL) | `IShaderModule` |
| `GL.CreateProgram()` / `LinkProgram` | RenderPipeline + BindGroupLayout | `IGraphicsPipeline` |
| `GL.UseProgram()` / `Uniform*` | SetPipeline + SetBindGroup | Internal (backend-specific) |
| `GL.BindTexture()` / `ActiveTexture` | BindGroup (texture + sampler) | Internal (backend-specific) |
| `GL.DrawElements()` | RenderPassEncoder.Draw() / DrawIndexed() | `IBackendRenderer.Render()` |
| `gl.Clear()` / `gl.ClearColor()` | RenderPass color attachment clear | `IBatcher.Clear()` (unchanged) |
| `gl.Viewport()` | Framebuffer resize → reconfigure swap chain | `IBatcher` / window resize event |
| `gl.SwapBuffers()` | SurfacePresent() | `EndFrame()` |

### Key Structural Changes

1. **`GLWindow` → `RenderWindow`**: Rename and refactor. Split GL context creation from window management. The window holds a `IGraphicsBackend` instead of directly calling OpenGL.

2. **`GLInvoker` / `IGLInvoker`**: Behind `IGraphicsBackend`. The 47-method interface becomes internal to the OpenGL backend only.

3. **`GpuBufferBase<T>`**: Refactored. Base class extracted with backend-agnostic batch management. OpenGL version handles VAO/VBO/EBO. WebGPU version handles WebGPU buffers and bind groups.

4. **Shader Programs**: Each `IShaderProgram` is backend-specific. The abstract interface becomes `{ ShaderId, Name, Use() }` → `{ PipelineHandle, Name, Bind() }`.

5. **Texture loading**: `Texture` class replaces `IGLInvoker` calls with `IBackendTextureFactory` (or similar). The public `ITexture.Id` becomes an opaque `uint` handle managed by the backend.

6. **Renderers**: Renderers no longer own `IGLInvoker` + `IOpenGLService` + `IGpuBuffer<T>` + `IShaderProgram`. Instead, they hold `IBackendRenderer<T>` which knows how to flush its batch items to the GPU via the active backend.

7. **Silk.NET Window**: Set `GraphicsAPI.None` to avoid OpenGL context creation when using WebGPU backend. The window still provides GLFW surface for WebGPU surface creation.

---

## Phase 3: Implementation Steps

### Step 1: Extract Backend Interface
**Files to create/modify**: ~10 new files
- Define `IVelaptorBackend` or `IGraphicsBackend` interface
- Move `IGpuBuffer<T>` and `IShaderProgram` into backend-agnostic namespace
- Extract batch-item structs from `Velaptor.OpenGL.Batching` to `Velaptor.Batching`
- Keep existing code working by making OpenGL classes implement new interfaces

### Step 2: Refactor Window Init
**Files to modify**: `GLWindow.cs`, `SilkWindowFactory.cs`, `IoC.cs`
- Add `GraphicsAPI.None` option via Silk.NET constructor for WebGPU mode
- Move GL context creation behind backend interface
- Window becomes backend-aware but not backend-specific

### Step 3: Port WebGPU Prototype to Velaptor Codebase
**Files to create**: `Velaptor/Graphics/Backend/WebGPU/*.cs`
- `WebGPUDevice.cs` — Instance → Adapter → Device → Queue chain (from prototype)
- `WebGPUSurface.cs` — Window surface + swap chain (from prototype)
- `WebGPUBackend.cs` — Implements `IGraphicsBackend`
- `WebGPUShaderModule.cs` — WGSL shader compilation
- `WebGPURenderPipeline.cs` — Pipeline + bind groups
- `WebGPUTexture.cs` — Texture upload, views, samplers

### Step 4: Implement Batched Rendering for WebGPU
**Files to create**: `Velaptor/Graphics/Backend/WebGPU/Buffers/*.cs`
- `WebGPUBufferBase.cs` — Batch buffer management
- `WebGPUTextureBuffer.cs` — Texture batch with per-texture bind groups
- `WebGPUFontBuffer.cs` — Font glyph batch
- `WebGPUShapeBuffer.cs` — Shape batch
- `WebGPULineBuffer.cs` — Line batch

### Step 5: Write WGSL Shaders
**Files to create**: `Velaptor/Graphics/Backend/WebGPU/Shaders/*.wgsl`
- `texture.wgsl` — Sprite/texture rendering (with vertex buffer input)
- `font.wgsl` — Font glyph rendering
- `shape.wgsl` — Rounded rectangle + circle rendering
- `line.wgsl` — Line rendering

### Step 6: Implement Font System for WebGPU
- Font atlas texture creation via WebGPU `QueueWriteTexture`
- No FreeType changes needed (CPU-side rasterization unchanged)
- Glyph metrics computation unchanged
- Kerning/mesh generation unchanged

### Step 7: Wire IoC for Backend Selection
- Add `GraphicsBackend` enum to `AppSettings` or config
- Conditional IoC registration: OpenGL components vs WebGPU components
- Default to OpenGL during transition, WebGPU once stable

### Step 8: Test and Validate
- Visual comparison tests (render same scene via both backends)
- Performance benchmarks (compare batch throughput)
- Integration test with existing game projects

---

## Key Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Shape shader (402 lines GLSL) | High — complex math to port to WGSL | Port incrementally; WGSL supports mathematically equivalent operations |
| Font atlas rendering | Medium — ensures consistent glyph metrics | FreeType atlas generation is CPU-side; only GPU upload changes |
| ImGui support | Medium — currently uses Silk.NET OpenGL ImGui extension | Use `Silk.NET.Windowing` events for ImGui input, software render or Dear ImGui's WebGPU backend |
| wgpu-native binary size | Low — unavoidable extra ~5-10MB | Already referenced in csharp-webgpu prototype |
| Device loss handling | Medium — WebGPU requires explicit recovery | Implement `DeviceLostCallback` with device recreation logic (prototype missing this) |
| Cross-platform differences | Medium — wgpu-native on Win/Linux/macOS | Silk.NET's WGPU native package handles platform binaries |

---

## What the csharp-webgpu Prototype Contributes

| Component | Use in Migration | Gaps to Fill |
|-----------|-----------------|--------------|
| `GraphicsDevice` | Instance→Adapter→Device→Queue chain | Needs error-scope support, device loss |
| `GraphicsSurface` | Window surface + swap chain config | Works as-is |
| `GraphicsPipeline` | Pipeline + bind group layout pattern | Needs vertex buffer support; multiple pipelines |
| `GraphicsShader` | WGSL template substitution | Remove `{HW}/{HH}` hack; add vertex buffer input |
| `GraphicsTexture` | QueueWriteTexture + view + sampler | Works as-is; add staging buffer for dynamic updates |
| `Camera2D` | Uniform buffer + bind group pattern | Reuse for per-frame camera; expand for multiple uniform buffers |
| `Frame` | Per-frame encoder/render-pass/submit pattern | Needs to handle multiple render passes per frame |
| SafeHandle wrappers | All 16 handle types | May need modifications if wgpu-native API changes |
| WGSL shader | Proves WGSL works with Silk.NET WebGPU bindings | Must be completely rewritten for batched rendering |
| Blending/sampler config | Pipeline blend state, sampler descriptor | Works as-is; extend for multiple blend modes |

---

## Summary: Key Numbers

- **Public API files to change**: 0 (ITexture.Id is minor type discussion)
- **Internal files to create**: ~25-30 new files for WebGPU backend
- **Internal files to refactor**: ~15-20 existing GL files (extract interfaces)
- **Total estimated files touched**: ~45-50
- **OpenGL-specific code to retire**: ~26 existing files (after dual-backend stabilizes)
- **Shader count**: 4 GLSL pairs → 4 WGSL files

# RenderMediator Performance Report

## 1. O(n²) Layer Collection with Linear Search

**What's currently happening:**
```csharp
for (var i = 0; i < textureItems.Length; i++)
{
    var textureLayer = textureItems.Span[i].Layer;
    if (this.allLayers.Span.Contains(textureLayer))  // ← O(n) search
    {
        continue;
    }
    this.allLayers.Span[layerIndex] = textureLayer;
    layerIndex++;
}
```

For every item, you're calling `.Contains()` which scans through the entire `allLayers` array (1000 elements) to check if the layer already exists. If you have 500 texture items, that's 500 × 1000 = 500,000 comparisons. Do this for all four item types and you're looking at millions of operations per frame.

**How to fix it:**
Use a `HashSet<int>` for O(1) lookups instead of O(n):

```csharp
// Add as a field
private readonly HashSet<int> usedLayers = new();

// In CoordinateRenders():
this.usedLayers.Clear();

for (var i = 0; i < textureItems.Length; i++)
{
    var textureLayer = textureItems.Span[i].Layer;
    if (this.usedLayers.Add(textureLayer))  // O(1) - returns true if added (wasn't already there)
    {
        this.allLayers.Span[layerIndex] = textureLayer;
        layerIndex++;
    }
}
// Repeat for other item types...
```

---

## 2. Fixed 1000-element Array Always Sorted

**What's currently happening:**
```csharp
this.allLayers.Span.Sort();  // Sorts ALL 1000 elements
```

Even if you only have 10 unique layers, you're sorting all 1000 elements. Most of them are just `int.MaxValue` padding. Sorting 1000 elements when you only need to sort 10 is wasteful.

**How to fix it:**
Track how many layers you actually used and only sort that portion:

```csharp
// After collecting all layers, layerIndex holds the actual count
var actualLayerCount = layerIndex;

// Only sort the layers you actually used
this.allLayers.Span.Slice(0, actualLayerCount).Sort();
```

---

## 3. Redundant Layer Iteration

**What's currently happening:**
```csharp
for (var i = 0; i < this.allLayers.Length; i++)  // Always 1000 iterations
{
    if (this.allLayers.Span[i] == int.MaxValue)
    {
        break;
    }
    // ... process layer
}
```

You're iterating through all 1000 elements even though you know exactly how many layers you have (it's `layerIndex` from the collection phase). Plus, for each layer, you're calling `TotalOnLayer()` and `FirstLayerIndex()` which likely scan through the entire batch again.

**How to fix it:**
```csharp
// Only iterate through actual layers
for (var i = 0; i < actualLayerCount; i++)
{
    var currentLayer = this.allLayers.Span[i];
    // ... rest of the logic
}
```

For the `TotalOnLayer()` and `FirstLayerIndex()` calls, if these methods scan the batch each time, consider caching the layer boundaries during your initial collection pass.

---

## 4. Memory Allocation Per Frame

**What's currently happening:**
```csharp
this.bufferResizeReactable.Push(
    PushNotifications.ResizeBufferId,
    new RequiredBufferCapacityData  // ← New allocation every frame
    {
        TotalTextureItems = (uint)textureItems.Length,
        TotalFontItems = (uint)fontItems.Length,
        TotalShapeItems = (uint)shapeItems.Length,
        TotalLineItems = (uint)lineItems.Length,
    });
```

Even though `RequiredBufferCapacityData` is likely a struct, you're creating a new instance every frame. In a 60 FPS game, that's 60 allocations per second. While small, this adds up and puts pressure on the garbage collector.

**How to fix it:**
Make it a field and reuse it:

```csharp
// Add as a field
private RequiredBufferCapacityData bufferCapacityData;

// In CoordinateRenders():
this.bufferCapacityData.TotalTextureItems = (uint)textureItems.Length;
this.bufferCapacityData.TotalFontItems = (uint)fontItems.Length;
this.bufferCapacityData.TotalShapeItems = (uint)shapeItems.Length;
this.bufferCapacityData.TotalLineItems = (uint)lineItems.Length;

this.bufferResizeReactable.Push(PushNotifications.ResizeBufferId, this.bufferCapacityData);
```

---

## 5. Four Separate Sort Operations

**What's currently happening:**
```csharp
textureItems.Span.Sort(this.textureItemComparer);
fontItems.Span.Sort(this.fontItemComparer);
shapeItems.Span.Sort(this.shapeItemComparer);
lineItems.Span.Sort(this.lineItemComparer);
```

You're sorting each batch type separately before you even know what layers exist. This is necessary for the layer-based rendering to work, but it's worth noting that you're doing 4 full sorts per frame.

**How to optimize:**
This is actually necessary for your architecture, but you can optimize the sort itself. If your comparers are doing expensive operations, cache intermediate values. Also, if items are already mostly sorted from the previous frame, consider using an adaptive sort algorithm.

---

## 6. Repeated Span Access

**What's currently happening:**
```csharp
if (this.allLayers.Span.Contains(textureLayer))  // Span accessed
{
    continue;
}
this.allLayers.Span[layerIndex] = textureLayer;  // Span accessed again
```

Every time you write `this.allLayers.Span`, the runtime has to create a new `Span<T>` wrapper. While this is cheap (it's a ref struct), in tight loops it adds up.

**How to fix it:**
Cache the span in a local variable:

```csharp
var layersSpan = this.allLayers.Span;

for (var i = 0; i < textureItems.Length; i++)
{
    var textureLayer = textureItems.Span[i].Layer;
    if (layersSpan.Contains(textureLayer))  // Use cached span
    {
        continue;
    }
    layersSpan[layerIndex] = textureLayer;
    layerIndex++;
}
```

---

## Summary of Impact

**Highest Priority (fix these first):**
1. **HashSet for layer lookup** - Changes O(n²) to O(n), potentially 1000x faster
2. **Sort only used layers** - Reduces sort time by 99% if you have few layers
3. **Iterate only used layers** - Eliminates 990 unnecessary iterations per frame

**Medium Priority:**
4. **Reuse buffer capacity struct** - Reduces GC pressure
5. **Cache Span references** - Minor optimization in tight loops

**Low Priority:**
6. **Sort optimization** - Only if your comparers are expensive

Fixing items 1-3 alone could give you a 10-100x performance improvement in the `CoordinateRenders()` method.

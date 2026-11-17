# Fixing GameSir Controller Vibration (Rumble) on Linux

GameSir controllers like the G7 SE have a quirk: on Linux, they often connect in a mode where **vibration (rumble) does not work**. This page explains why that happens, and how to fix it permanently with a single kernel setting.

## TL;DR

Add `usbcore.quirks=3537:10a0:r` to your kernel command line, reboot, and plug in the controller. It will now stay in XInput mode (LED above the Home button lit) and vibration works. Full instructions below.

Want to try it first, or undo it later, without rebooting? Steps 1–2 apply and revert the quirk at runtime.

## The Problem

### The controller has three USB modes

The GameSir G7 SE presents itself as one of three different USB devices depending on how the host talks to it:

| USB ID | Mode | What it means |
|---|---|---|
| `3537:10a0` | **XInput** | Xbox mode — vibration works, LED above the Home button is lit |
| `3537:1069` | **Xbox GIP** | Licensed Xbox protocol — vibration works |
| `3537:1082` | **DirectInput fallback** | Generic HID — vibration does **not** work |

The controller's mode is decided by how the host **enumerates** it at plug-in. Windows and Linux do this differently:

- **Windows** asks for a 255-byte configuration descriptor.
- **Linux** asks for a 9-byte configuration descriptor.

The G7 SE's firmware fails Linux's 9-byte request during initialization, so on Linux it falls back to DirectInput mode (`3537:1082`) — no vibration, no LED.

You can see the failure in the kernel log:

```bash
sudo dmesg | grep -iE 'gamesir|can.t set config'
```

You'll see `can't set config #1, error -71`.

### How to tell which mode your controller is in

Plug in the controller and run:

```bash
lsusb | grep -i gamesir
```

- `3537:1082` = DirectInput fallback — **no vibration** (LED off)
- `3537:10a0` = XInput — vibration works (LED above the Home button lit)

The LED above the Home button is the physical indicator: **lit = XInput mode, vibration works**.

### Why it sometimes works after a Windows session

If you dual-boot Windows and Linux, the controller's mode is determined by the last host that enumerated it. Windows requests 255 bytes, so the controller stays in XInput mode. If you boot into Fedora after a Windows session (warm reboot, no power cycle), the controller may still be in XInput mode (LED lit) — and vibration works. If you boot Linux from cold, the controller falls back to DirectInput mode (LED off) — and vibration doesn't.

## The Fix

### The `r` quirk flag

The Linux kernel has a per-device quirk system that changes how the kernel talks to a specific USB device. The `r` flag — `USB_QUIRK_WINDOWS_CONFIG_REQ_SIZE` — makes Linux ask for a **255-byte configuration descriptor** instead of 9 bytes, exactly like Windows does.

From the Linux kernel documentation:

> `r` = USB_QUIRK_WINDOWS_CONFIG_REQ_SIZE — Device fails during initialization when asked for a 9-byte configuration descriptor request. Ask for a 255-byte request instead to mirror Windows' behavior.

### Will this break other USB devices?

No. The quirk is scoped to a single USB identity (`VendorID:ProductID:Flags`). Your entry `3537:10a0:r` only affects a device that enumerates as `3537:10a0` — the GameSir G7 SE's XInput identity. Your keyboards, mice, and other controllers are untouched.

And even on that one device, the flag only changes the size of one configuration-descriptor request (9 bytes → 255 bytes) — nothing else about how USB works changes.

## Implementation

### Step 1: Apply the quirk right away (no reboot)

Before making it permanent, verify the quirk fixes your controller:

1. Check the current quirks file (usually empty):

   ```bash
   cat /sys/module/usbcore/parameters/quirks
   ```

2. Set the quirk (note: this **overwrites** the whole file — if you have other quirks, comma-separate them):

   ```bash
   echo '3537:10a0:r' | sudo tee /sys/module/usbcore/parameters/quirks
   ```

3. Unplug and replug the controller. The quirk applies to devices plugged in **after** it is set, so the order matters.

4. Verify it landed:

   ```bash
   lsusb | grep -i gamesir
   ```

   You should now see `3537:10a0` and the LED above the Home button lit.

### Step 2: Revert the quirk right away (no reboot)

Changed your mind? First, see where the quirk is currently coming from:

```bash
cat /sys/module/usbcore/parameters/quirks      # runtime value (empty = not applied right now)
grep -o 'usbcore.quirks=[^ ]*' /proc/cmdline   # kernel command line (nothing printed = not permanent yet)
```

- Only the first command shows the quirk → it was applied at runtime, and the clear below is all you need.
- The second also shows it → it is baked into the boot config too; also follow the permanent-revert notes at the end of this step.

To clear the runtime quirk:

1. Empty the quirks file (the kernel treats an empty value as "clear the whole list"):

   ```bash
   echo "" | sudo tee /sys/module/usbcore/parameters/quirks
   ```

2. Verify it is empty:

   ```bash
   cat /sys/module/usbcore/parameters/quirks
   ```

3. Unplug and replug the controller. It falls back to `3537:1082` (LED off, no vibration) — exactly how it behaved before the quirk.

Two things worth knowing:

- A controller that is **already plugged in** keeps its current mode. The quirk is only read at plug-in, so replugging is what actually applies (or removes) the behavior.
- The runtime value also clears itself on reboot — sysfs writes never survive a restart.

**If the quirk is also in your kernel command line (Step 3),** clearing the sysfs file only cleans the current session — the quirk comes back on your next reboot, because the kernel re-reads its command line. To revert fully, remove it from the command line as well:

- **Fedora:**

  ```bash
  sudo grubby --update-kernel=ALL --remove-args="usbcore.quirks=3537:10a0:r"
  ```

  Then remove ` usbcore.quirks=3537:10a0:r` from `GRUB_CMDLINE_LINUX` in `/etc/default/grub` (`sudo nano /etc/default/grub`).

- **Ubuntu / Debian / Mint:** remove `usbcore.quirks=3537:10a0:r` from `GRUB_CMDLINE_LINUX_DEFAULT` in `/etc/default/grub`, then run:

  ```bash
  sudo update-grub
  ```

### Step 3: Make it permanent

The kernel command line is the permanent way to set the quirk. The exact variable name differs between distributions:

**Note:** this step does not apply anything to your **running** kernel — it only prepares the boot config. The quirk becomes active at the next boot (and for kernels installed after this). Until then, the sysfs write in Step 1 is what makes the quirk work — so run Step 1 first, then this step.

#### Ubuntu / Debian / Mint

Add the parameter to `GRUB_CMDLINE_LINUX_DEFAULT` in `/etc/default/grub`:

```bash
sudo nano /etc/default/grub
```

Add `usbcore.quirks=3537:10a0:r` inside the quotes:

```
GRUB_CMDLINE_LINUX_DEFAULT="quiet splash usbcore.quirks=3537:10a0:r"
```

Then regenerate GRUB (`update-grub` is Ubuntu/Debian-only — Fedora doesn't have it, and doesn't need it):

```bash
sudo update-grub
```

#### Fedora

Fedora uses `GRUB_CMDLINE_LINUX` (there is no `GRUB_CMDLINE_LINUX_DEFAULT`), and with `GRUB_ENABLE_BLSCFG=true` (Fedora's default), the kernel command line lives in per-kernel entries under `/boot/loader/entries/`. So you need two steps:

1. Add the parameter to `GRUB_CMDLINE_LINUX` in `/etc/default/grub` (for future kernels):

   ```bash
   sudo nano /etc/default/grub
   ```

   Add `usbcore.quirks=3537:10a0:r` inside the quotes:

   ```
   GRUB_CMDLINE_LINUX="rhgb quiet usbcore.quirks=3537:10a0:r"
   ```

2. Apply it to the kernels already installed:

   ```bash
   sudo grubby --update-kernel=ALL --args="usbcore.quirks=3537:10a0:r"
   ```

### Step 4: Reboot and verify

Reboot, then:

```bash
cat /proc/cmdline
```

You should see `usbcore.quirks=3537:10a0:r` in the output.

Plug in the controller and check:

```bash
lsusb | grep -i gamesir
```

`3537:10a0` + LED lit = XInput mode, vibration works.

### Step 5: Test in your game

Launch the game and check rumble.

## Troubleshooting

If the controller still falls back to `1082`:

1. Check the quirk is active:

   ```bash
   cat /proc/cmdline
   ```

2. Check the kernel log:

   ```bash
   sudo dmesg | grep -iE 'gamesir|can.t set config'
   ```

3. Check the LED — lit = XInput mode, vibration works.

## Other GameSir Models

| Controller | USB ID | Quirk |
|---|---|---|
| G7 SE | `3537:10a0` | `3537:10a0:r` |
| G7 Pro 8K PC (newer black units) | `3537:1034` | `3537:1034:r` |

Multiple devices: comma-separate them:

```
usbcore.quirks=3537:10a0:r,3537:1034:r
```

## Credits & References

- **Deadband** (github.com/broroeror/gamesir-linux-tools) — the project that documented the mode behavior and the quirk fix. See `docs/g7se.md` and `MANUAL.md`.
- **Linux kernel documentation** — [kernel.org kernel-parameters](https://www.kernel.org/doc/html/latest/admin-guide/kernel-parameters.html)

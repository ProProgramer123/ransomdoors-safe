# RANS0M

A fan-made recreation of the RANSOM (A-90) entity from the Roblox game
*Doors*, as a Windows desktop app. It randomly pops the entity's face up on
your screen, you need to stop moving your mouse and stay off the keyboard, or it
"infects" your PC: 8 gold coin files get scattered around your user folders,
and you have to drag at least 5 of them onto the ransom window before the timer runs
out. Fail to pay in time and it crashes your computer.

This was built for fun, it's kind of poorly coded.
Right now the only noticable bug is that the ransom window doesn't always stay on top of other windows, but it should be fine for the most part. It can't go on top of fullscreen apps.

## Read this before running it

This app **really** shuts down or crashes your computer if you don't pay
the fake ransom in time. That's not a metaphor, it calls `shutdown /s /t 0`,
or (if elevated) marks itself as a critical process so that closing it takes
Windows down with it. This is intentional, but it means
you should:

- Only run it on a machine you own, save your work first, and expect it to
  actually shut down or crash at some point.
- Not run it on anyone else's computer without them knowing exactly what
  it does and agreeing to it.

It is not malware in the sense of trying to steal anything, hide itself, or
spread, it doesn't touch your files besides dropping/deleting its own
harmless `.gold` marker files, and it's fully open source so you can check
that yourself. See [LICENSE.md](LICENSE.md) for the full terms and
disclaimer.

## Requirements

- Windows (uses Win32 hooks, `shutdown.exe`, the registry, etc. This
  won't run anywhere else)
- [.NET 10 SDK](https://dotnet.microsoft.com/) or newer
- Visual Studio 2022+ (optional, for the WinForms designer) or just the
  `dotnet` CLI

## Building & running

```
git clone https://github.com/Ixars/ransomdoors
cd rans0m
dotnet build
dotnet run
```

Or open `rans0m.slnx` in Visual Studio and hit F5.

The app runs from a system tray icon (right-click it for a Close option,
it's disabled while a ransom is active, so you can't just dodge it from the
tray).

## Configuration

The main knobs live at the top of `Global.cs`:

- `minRansomTime` / `maxRansomTime` — how often (in seconds) the entity can
  randomly show up.

Everything else (images, sounds, taunt window titles) is in
`Properties/Resources.resx` and the `Resources/` folder if you want to swap
them out.

## Credits

- **Doors** is made by **LSPLASH**. The RANSOM/A-90 entity, its name, look,
  and concept are their original work — this project is an unofficial fan
  recreation, not affiliated with or endorsed by LSPLASH. Go play the real
  game.
- Built with [NAudio](https://github.com/naudio/NAudio) for audio playback.
- Sound effects and images are from the game, taken from the wikis.

## License

Source-available, free to use/modify/redistribute for educational and
non-commercial purposes, with credit required and reselling (original or
modified) forbidden. Full terms in [LICENSE.md](LICENSE.md) — read it, it's
short.

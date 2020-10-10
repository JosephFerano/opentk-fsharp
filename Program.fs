open OpenTK.Mathematics
open OpenTK.Windowing.Desktop
open OpenTK.Windowing.GraphicsLibraryFramework

type Game(windowSettings , nativeSettings) =
    inherit GameWindow(windowSettings , nativeSettings)

    override this.OnUpdateFrame(e) =
        if this.KeyboardState.IsKeyDown(Keys.Escape)
            then this.Close()
        base.OnUpdateFrame(e)

[<EntryPoint>]
let main argv =
    let windowSettings = GameWindowSettings.Default
    let nativeSettings = NativeWindowSettings()
    nativeSettings.Title <- "OpenTK Hello Triangle"
    nativeSettings.Size  <- Vector2i(1200, 900)
    use game = new Game(windowSettings, nativeSettings)
    game.Run()
    0 // return an integer exit code

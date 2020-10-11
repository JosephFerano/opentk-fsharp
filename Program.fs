open System.IO
open OpenTK.Graphics.OpenGL4
open OpenTK.Mathematics
open OpenTK.Windowing.Desktop
open OpenTK.Windowing.GraphicsLibraryFramework
open FsOpenGL

let vertices = [|
        -0.5 ; -0.5 ; 0.0 ; // Bottom-left vertex
         0.5 ; -0.5 ; 0.0 ; // Bottom-right vertex
         0.0 ;  0.5 ; 0.0 ;  // Top vertex
    |]

let mutable vertexBufObj = -1
let mutable vertexArrObj = -1
let mutable shader = Unchecked.defaultof<_>

type Game(windowSettings , nativeSettings) =
    inherit GameWindow(windowSettings , nativeSettings)

    override this.OnLoad() =
        GL.ClearColor(Color4.CornflowerBlue)

        vertexBufObj <- GL.GenBuffer()
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufObj)

        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof<float>, vertices, BufferUsageHint.StaticDraw)

        let dir = Directory.GetCurrentDirectory()

        shader <- Shader("shader.vert", "shader.frag")

        shader.Use()

        vertexArrObj <- GL.GenVertexArray()
        GL.BindVertexArray(vertexArrObj)

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof<float>, 0)
        GL.EnableVertexAttribArray(0)

        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufObj)

        base.OnLoad()

    override this.OnResize(e) =
        GL.Viewport(0, 0, this.Size.X, this.Size.Y)

        base.OnResize(e)

    override this.OnRenderFrame(e) =
        GL.Clear(ClearBufferMask.ColorBufferBit)

        shader.Use()

        GL.BindVertexArray(vertexArrObj)

        GL.DrawArrays(PrimitiveType.Triangles, 0, 3)

        this.SwapBuffers()

        base.OnRenderFrame(e)

    override this.OnUpdateFrame(e) =
        if this.KeyboardState.IsKeyDown(Keys.Escape)
            then this.Close()
        base.OnUpdateFrame(e)

    override this.OnUnload() =
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
        GL.BindVertexArray(0)
        GL.UseProgram(0)
        GL.DeleteBuffer(vertexBufObj)
        GL.DeleteVertexArray(vertexArrObj)
        GL.DeleteProgram(shader.Handle)

        base.OnUnload()

[<EntryPoint>]
let main argv =
    let windowSettings = GameWindowSettings.Default
    let nativeSettings = NativeWindowSettings()
    nativeSettings.Title <- "OpenTK Hello Triangle"
    nativeSettings.Size  <- Vector2i(1200, 900)
    use game = new Game(windowSettings, nativeSettings)
    game.Run()
    0 // return an integer exit code

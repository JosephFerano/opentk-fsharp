open OpenTK.Graphics.OpenGL4
open OpenTK.Mathematics
open OpenTK.Windowing.Desktop
open OpenTK.Windowing.GraphicsLibraryFramework
open FsOpenGL

let vertices = [|
     0.5f;  0.5f; 0.0f; // top right
     0.5f; -0.5f; 0.0f; // bottom right
    -0.5f; -0.5f; 0.0f; // bottom left
    -0.5f;  0.5f; 0.0f; // top left
|]

let indices = [|
    0u; 1u; 3u; // first triangle
    1u; 2u; 3u; // second triangle
|]

let mutable vertexBufObj = -1
let mutable vertexArrObj = -1
let mutable elemBufObj = -1
let mutable shader = Unchecked.defaultof<Shader>

type Game(windowSettings , nativeSettings) =
    inherit GameWindow(windowSettings , nativeSettings)

    override this.OnLoad() =
        GL.ClearColor(Color4.CornflowerBlue)

        vertexBufObj <- GL.GenBuffer()
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufObj)
        GL.BufferData(BufferTarget.ArrayBuffer, Array.length vertices * sizeof<float32>, vertices, BufferUsageHint.StaticDraw)

        elemBufObj <- GL.GenBuffer()
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elemBufObj)
        GL.BufferData(BufferTarget.ElementArrayBuffer, Array.length indices * sizeof<uint32>, indices, BufferUsageHint.StaticDraw)

        shader <- Shader("shader.vert", "shader.frag")

        shader.Use()

        vertexArrObj <- GL.GenVertexArray()
        GL.BindVertexArray(vertexArrObj)
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufObj)
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elemBufObj)

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof<float32>, 0)
        GL.EnableVertexAttribArray(0)

        base.OnLoad()

    override this.OnRenderFrame(e) =
        GL.Clear(ClearBufferMask.ColorBufferBit)

        shader.Use()

        GL.BindVertexArray(vertexArrObj)

        GL.DrawElements(PrimitiveType.Triangles, Array.length indices, DrawElementsType.UnsignedInt, 0)
//        GL.DrawArrays(PrimitiveType.Triangles, 0, 3)

        this.SwapBuffers()

        base.OnRenderFrame(e)

    override this.OnUpdateFrame(e) =
        if this.KeyboardState.IsKeyDown(Keys.Escape)
            then this.Close()
        base.OnUpdateFrame(e)

    override this.OnResize(e) =
        GL.Viewport(0, 0, this.Size.X, this.Size.Y)

        base.OnResize(e)

    override this.OnUnload() =
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0)
        GL.BindVertexArray(0)
        GL.UseProgram(0)
        GL.DeleteBuffer(vertexBufObj)
        GL.DeleteBuffer(elemBufObj)
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

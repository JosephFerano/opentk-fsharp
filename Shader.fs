namespace FsOpenGL

open System
open System.IO
open System.Text
open System.Collections.Generic
open OpenTK.Graphics.OpenGL4
open OpenTK.Mathematics

type Shader(vertPath, fragPath) =
    let mutable handle = 0

    let uniformLocations = Dictionary<string, int>()

    let compileShader(shader : int) =
       GL.CompileShader(shader)

       let code = GL.GetShader(shader, ShaderParameter.CompileStatus)
       if (code <> int All.True) then
           let infoLog = GL.GetShaderInfoLog(shader)
           raise <| Exception(sprintf "Error occurred whilst compiling Shader(%A).\n\n%A" shader infoLog)

    let loadSource(path : string) =
       try
           use sr = new StreamReader(path, Encoding.UTF8)
           sr.ReadToEnd()
       with _ -> ""

    let linkProgram(program : int) =
        GL.LinkProgram(program)

        let code = GL.GetProgram(program, GetProgramParameterName.LinkStatus)
        if (code <> (int)All.True) then
            raise <| Exception(sprintf "Error occurred whilst linking Program(%A)" program)

    do
       let shaderSource = loadSource(vertPath)

       let vertexShader = GL.CreateShader(ShaderType.VertexShader)

       GL.ShaderSource(vertexShader, shaderSource)

       compileShader(vertexShader)

       let shaderSource = loadSource(fragPath)
       let fragmentShader = GL.CreateShader(ShaderType.FragmentShader)
       GL.ShaderSource(fragmentShader, shaderSource)
       compileShader(fragmentShader)

       handle <- GL.CreateProgram()

       GL.AttachShader(handle, vertexShader)
       GL.AttachShader(handle, fragmentShader)

       linkProgram(handle)

       GL.DetachShader(handle, vertexShader)
       GL.DetachShader(handle, fragmentShader)
       GL.DeleteShader(fragmentShader)
       GL.DeleteShader(vertexShader)

       let numberOfUniforms = GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms)

       for i in 0 .. numberOfUniforms - 1 do
           let ( key , _ , _ ) = GL.GetActiveUniform(handle, i)

           let location = GL.GetUniformLocation(handle, key)

           uniformLocations.Add(key, location)

    member this.Use() =
        GL.UseProgram(handle)

    member this.GetAttribLocation(attribName : string) =
        GL.GetAttribLocation(handle, attribName)

    member this.SetInt(name : string , data : int) =
        GL.UseProgram(handle)
        GL.Uniform1(uniformLocations.[name], data)

    member this.SetFloat(name : string, data : float) =
        GL.UseProgram(handle)
        GL.Uniform1(uniformLocations.[name], data)

    member this.SetMatrix4(name, data) =
        GL.UseProgram(handle)
        GL.UniformMatrix4(uniformLocations.[name], true, ref data)

    member this.SetVector3(name, data : Vector3) =
        GL.UseProgram(handle)
        GL.Uniform3(uniformLocations.[name], data)

    member this.Handle = handle
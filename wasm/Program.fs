namespace Wasm

module ClientInterop =
  type expr =
    | ENull
    | ELet of expr * expr
    | EIf of expr * expr * expr
    | EPipe of expr list
    | EPipeTarget 

  type Fn = { expr : expr }
  type Handler = { fn : Fn  }
  type Payload = | Handler of Handler

module Serialization =

  module Vanilla =
    open System.Text.Json
    open System.Text.Json.Serialization

    let options () = 
      let fsharpConverter =
        JsonFSharpConverter(
          unionEncoding =
            (JsonUnionEncoding.InternalTag ||| JsonUnionEncoding.UnwrapOption)
        )
      let options = JsonSerializerOptions()
      options.MaxDepth <- 1024
      options.Converters.Add(fsharpConverter)
      options

    let deserialize<'a> (json : string) : 'a =
      JsonSerializer.Deserialize<'a>(json, options())


type EvalWorker =
  static member OnMessage(message : string) =
    printfn "Hello from F#, compiled to WASM"
    
    try
      Serialization.Vanilla.deserialize<ClientInterop.Payload> message |> ignore
      printfn "Deserialized"
    with
    | e -> printfn "failed to deserialize %A" e.Message

    // task{} from Ply.
    // If we switch to non-Ply task, we no longer see the issue.
    FSharp.Control.Tasks.Affine.task { return () }
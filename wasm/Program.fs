namespace Wasm

module ClientInterop =
  type id = uint64

  type fluidExpr =
    | ENull of id
    | ELet of id * string * fluidExpr * fluidExpr
    | EIf of id * fluidExpr * fluidExpr * fluidExpr
    | EPipe of id * fluidExpr list
    | EPipeTarget of id

  type user_fn = { ast : fluidExpr }
  type handler_analysis_param = { user_fns : List<user_fn>  }
  type performAnalysisParams = | AnalyzeHandler of handler_analysis_param

module Serialization =

  module Vanilla =
    open ClientInterop
    open System.Text.Json
    open System.Text.Json.Serialization

    type TLIDConverter() =
      inherit JsonConverter<id>()

      override _.Read(reader : byref<Utf8JsonReader>, _type, _options) =
        if reader.TokenType = JsonTokenType.String then
          System.Convert.ToUInt64 (reader.GetString())
        else
          reader.GetUInt64()

      override _.Write(writer : Utf8JsonWriter, value : id, _options) =
        writer.WriteNumberValue(value)

    let options () = 
      let fsharpConverter =
        JsonFSharpConverter(
          unionEncoding =
            (JsonUnionEncoding.InternalTag ||| JsonUnionEncoding.UnwrapOption)
        )
      let options = JsonSerializerOptions()
      options.MaxDepth <- 1024
      options.Converters.Add(fsharpConverter)
      options.Converters.Add(TLIDConverter())
      options

    let deserialize<'a> (json : string) : 'a =
      JsonSerializer.Deserialize<'a>(json, options())


type EvalWorker =
  static member OnMessage(message : string) =
    printfn "Hello from F#, compiled to WASM"
    
    try
      Serialization.Vanilla.deserialize<ClientInterop.performAnalysisParams> message |> ignore
      printfn "Deserialized"
    with
    | e -> printfn "failed to deserialize %A" e.Message

    // task{} from Ply
    FSharp.Control.Tasks.Affine.task { return () }
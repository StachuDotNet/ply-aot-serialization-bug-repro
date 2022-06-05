// This project only exists to host the static assets (an .html file, and Blazor stuff)
module BugWebHost

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.StaticFiles
open Microsoft.Extensions.Hosting

let configureStaticContent (app : IApplicationBuilder) : IApplicationBuilder =
    let contentTypeProvider = FileExtensionContentTypeProvider()

    contentTypeProvider.Mappings[ ".wasm" ] <- "application/wasm"
    contentTypeProvider.Mappings[ ".pdb" ] <- "text/plain"
    contentTypeProvider.Mappings[ ".dll" ] <- "application/octet-stream"
    contentTypeProvider.Mappings[ ".dat" ] <- "application/octet-stream"
    contentTypeProvider.Mappings[ ".blat" ] <- "application/octet-stream"

    app.UseStaticFiles(
        StaticFileOptions(
            ServeUnknownFileTypes = true,
            OnPrepareResponse =
                (fun ctx ->
                ctx.Context.Response.Headers[ "Access-Control-Allow-Origin" ] <-
                    Microsoft.Extensions.Primitives.StringValues([| "*" |])),
            ContentTypeProvider = contentTypeProvider
        )
    )

[<EntryPoint>]
let main _ =
    let builder = WebApplication.CreateBuilder()

    let defaultFileOptions =
        let o = new DefaultFilesOptions()
        o.DefaultFileNames.Clear()
        o.DefaultFileNames.Add("index.html")
        o

    builder.WebHost
    |> fun wh -> wh.UseKestrel()
    |> ignore<IWebHostBuilder>

    let app = builder.Build()
    app.UseDefaultFiles(defaultFileOptions) |> ignore
    app |> configureStaticContent |> ignore

    app.Run()
    0

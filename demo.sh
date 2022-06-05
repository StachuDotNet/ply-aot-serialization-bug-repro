# clear things
# rm -rf **/bin
# rm -rf **/obj
rm -rf webhost/wwwroot/blazor/
mkdir webhost/wwwroot/blazor

# publish the wasm project
dotnet publish wasm -c Debug

# copy the wasm Blazor outputs to the corresponding wwwroot directories,
# one for the published artifacts, and one for the non-published artifacts
rsync -a wasm/bin/Debug/net6.0/wwwroot/_framework/ webhost/wwwroot/blazor/nonpublish
rsync -a wasm/bin/Debug/net6.0/publish/wwwroot/_framework/ webhost/wwwroot/blazor/publish

dotnet run --project webhost
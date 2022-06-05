A bug was found at the intersection of:
- F#
- Blazor
- System.Text.Json and FSharp.SystemTextJson
- AOT compilation (whether Debug or Release)
- Ply
- some oddly-specific request/payload from JS to resultant Wasm

It's not yet certain which of these pieces are fully relevant.

To reproduce, download this repo and run `./demo.sh`, which will:
- build the whole solution such that `webhost` is built
- publish the `wasm` project, resulting in two builds to load in browser using `blazor.webassembly.js`
  - non-published Debug build
  - published Debug build
- run the webhost

The webhost is now ready to load Blazor with both non-published artifacts (simple Debug build) and with published artifacts (which are AOT-compiled).

Once the webhost is runing, open your browser, open dev console, and direct yourself to the appropriate URL shown at the end of `demo.sh` running.
- The "non-published" link will show the expected successful result (payload is deserialized, then no error).
- The "published" link will show the failure - we deserialize fine, as before, but then run into an unexpected error related to memory: `memory access out of bounds`.


---

This can probably still be reduced a bit.
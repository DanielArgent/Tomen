// Learn more about F# at http://fsharp.org

open TomenWrappers

let getShape fileName =
    parseFile fileName
    |> function | Ok root -> asTable root.["physical"] | _ -> None
    |> Option.bind (fun physical -> asString physical.["shape"])

open Tomen

[<EntryPoint>]
let main argv =
    printfn "Shape is %s" ((Tomen.Tomen.ReadFile "dotted-keys.toml").Path<string> "physical.shape")
    match getShape "dotted-keys.toml" with
    | Some shape -> printfn "Shape is %s" shape
    | _ -> printf "Can not parse file!"
    0 // return an integer exit code

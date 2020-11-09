module TomenWrappers

open Tomen

let parseFile fileName =
    try
        Result.Ok (Tomen.ReadFile fileName)
    with
    | :? TomlException as e -> 
        Result.Error e

let asTable (tomlValue: TomlValue) =
    match tomlValue with
    | :? TomlTable as tomlTable ->
        Some tomlTable
    | _ -> None

let asString (tomlValue: TomlValue) =
    match tomlValue with
    | :? TomlString as tomlString ->
        Some tomlString.Value
    | _ -> None
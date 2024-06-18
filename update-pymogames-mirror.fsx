#load "./pymo-index-utils.fsx"

open PyMOIndexUtils
open FSharpPlus
open FSharp.Data
open System.IO


let official = Source.findSource "pymogames-official"
let mirror = Source.findSource "pymogames-mirror"
let gameDbJson = Http.RequestString official.Url
File.WriteAllText(Option.get mirror.LocalPath, gameDbJson)


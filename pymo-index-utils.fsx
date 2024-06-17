#r "nuget: FSharp.Data"
#r "nuget: FSharpPlus"
// 该模块是PyMO索引表的工具库，用于向其它工具提供支持

open FSharp.Data
open FSharpPlus
open System.IO


/// PyMO索引源
type Source =
    { Name: string
      Id: string
      Url: string
      ScreenshotBaseUrl: string
      LocalPath: string option
      LocalScreenshotPath: string option
      Description: string }


module Source =

    [<Literal>]
    let private sourcesJsonPath = "./sources.json"


    type private SourcesJsonType = JsonProvider<sourcesJsonPath>


    let sources =
        SourcesJsonType.Load sourcesJsonPath
        |> map (fun x ->
            { Name = x.Name
              Id = x.Id
              Url = x.Url
              ScreenshotBaseUrl = x.ScreenshotBaseurl |> Option.defaultValue ""
              LocalPath = x.LocalPath
              LocalScreenshotPath = x.LocalScreenshotPath
              Description = x.Desc })


type GamePlatform =
    | Android
    | S60v3
    | S60v5
    | Nintendo3DS
    | NintendoWii
    | SonyPSP


module GamePlatform =

    let toString =
        function
        | Android -> "android"
        | S60v3 -> "s60v3"
        | S60v5 -> "s60v5"
        | Nintendo3DS -> "3ds"
        | NintendoWii -> "wii"
        | SonyPSP -> "psp"


    let tryFromString =
        function
        | "android" -> Some Android
        | "s60v3" -> Some S60v3
        | "s60v5" -> Some S60v5
        | "3ds" -> Some Nintendo3DS
        | "wii" -> Some NintendoWii
        | "psp" -> Some SonyPSP
        | _ -> None


    let fromString =
        tryFromString
        >> Option.defaultWith (fun () -> failwith "Invalid platform")


type Date = Date of y: uint16 * m: uint8 * d: uint8


module Date =

    let toString (Date (y, m, d)) = sprintf "%d-%d-%d" y m d


    let tryFromString =
        trySscanf "%u-%u-%u"
        >> map (fun (y, m, d) -> Date(y, m, d))


    let fromString =
        tryFromString
        >> Option.defaultWith (fun () -> failwith "Invalid date format")


type GameMetadata =
    { Author: string option
      BaiduFolder: string
      ContainsH: bool
      DownloadLink: string option
      DownloadPass: string option
      Folder: string option
      GameID: uint option
      Introduction: string option
      Platforms: GamePlatform Set
      PublishDate: Date option
      PublishSite: string option
      Screenshots: string Set
      Title: string
      UnzipPass: string option
      Source: Source }


module GameMetadata =

    [<Literal>]
    let private jsonTemplate =
        """
[
    {
        "author": "a",
        "baidu_folder": "a",
        "contains_h": true,
        "download_link": "a",
        "download_pass": "a",
        "folder": "a",
        "game_id": 1,
        "introduction": "a",
        "platforms": ["android", "s60v3"],
        "publish_date": "a",
        "publish_site": "a",
        "screenshots": ["a", "b"],
        "title": "a",
        "unzip_pass": "a"
    },

    {
        "baidu_folder": "b",
        "contains_h": false,
        "platforms": ["android", "s60v3"],
        "screenshots": [],
        "title": "a"
    }
]
"""

    type private GameMetadataJsonType = JsonProvider<jsonTemplate>


    let private parseGameMetadataJson source (json: string) =
        GameMetadataJsonType.Parse json
        |> Array.map (fun json ->
            { Author = json.Author
              BaiduFolder = json.BaiduFolder
              ContainsH = json.ContainsH
              DownloadLink = json.DownloadLink
              DownloadPass = json.DownloadPass
              Folder = json.Folder
              GameID =
                json.GameId
                |> Option.bind (fun gameId ->
                    if gameId >= 0 then
                        Some <| uint gameId
                    else
                        None)

              Introduction = json.Introduction
              Platforms =
                json.Platforms
                |> choose GamePlatform.tryFromString
                |> Set.ofArray

              PublishDate = json.PublishDate >>= Date.tryFromString
              PublishSite = json.PublishSite
              Screenshots = json.Screenshots |> Set.ofArray
              Title = json.Title
              UnzipPass = json.UnzipPass
              Source = source })


    let loadGamesFromSource source =
        async {
            let! jsonText =
                match source.LocalPath with
                | Some localPath when File.Exists localPath -> File.ReadAllTextAsync localPath |> Async.AwaitTask
                | _ -> Http.AsyncRequestString(source.Url)

            return parseGameMetadataJson source jsonText
        }


    let loadGamesFromAllSources () =
        Source.sources
        |> map loadGamesFromSource
        |> Async.Parallel
        |> map Seq.concat

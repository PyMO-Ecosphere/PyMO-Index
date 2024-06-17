#r "nuget: FSharp.Data"

// 该模块是PyMO索引表的工具库，用于向其它工具提供支持

open FSharp.Data


/// PyMO索引源
type Source =
    { Name: string
      Id: string
      Url: string
      ScreenshotBaseUrl: string option
      LocalPath: string option
      Description: string }


module Source =

    [<Literal>]
    let private sourcesJsonPath = "./sources.json"


    type private SourcesJsonType = JsonProvider<sourcesJsonPath>


    let sources =
        SourcesJsonType.Load sourcesJsonPath
        |> Array.map (fun x ->
            { Name = x.Name
              Id = x.Id
              Url = x.Url
              ScreenshotBaseUrl = x.ScreenshotBaseurl
              LocalPath = x.LocalPath
              Description = x.Desc })

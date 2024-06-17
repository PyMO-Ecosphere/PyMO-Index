#load "./pymo-index-utils.fsx"

open PyMOIndexUtils
open System.Collections.Generic


let mutable hasError = false


let assertUnique (memberName: string) (f: 'a -> 'b option) (toCheck: 'a seq) =
    let alreadyChecked: Dictionary<'b, 'a> = Dictionary()
    for itemToCheck in toCheck do
        match f itemToCheck with
        | None -> ()
        | Some key ->
            if alreadyChecked.ContainsKey key then
                printfn "以下两个对象的%s相同，这是不被允许的：" memberName
                printfn "%A" itemToCheck
                printfn "%A" alreadyChecked.[key]
                printfn ""
                printfn ""
                hasError <- true
            else
                alreadyChecked.Add (key, itemToCheck) |> ignore


let assertUnique' memberName f toCheck =
    assertUnique memberName (f >> Some) toCheck


let assertMember (errInfo: string) (f: 'a -> bool) (toCheck: 'a seq) =
    for itemToCheck in toCheck do
        if not <| f itemToCheck then
            printfn "以下对象不满足条件“%s”，这是不被允许的：" errInfo
            printfn "%A" itemToCheck
            printfn ""
            printfn ""
            hasError <- true


let assertStringNotEmpty errInfo (f: 'a -> string) =
    assertMember errInfo (f >> System.String.IsNullOrEmpty >> not)


List.iter (fun f -> f Source.sources) [
    assertStringNotEmpty "源名称不能为空" _.Name
    assertUnique' "源ID" _.Id
    assertStringNotEmpty "源ID不能为空" _.Id
    assertUnique' "源URL" _.Url
    assertStringNotEmpty "源URL不能为空" _.Url
]


let abort () =
    printfn "未能通过验证。"
    exit -1


if hasError then abort ()


let allGames =
    GameMetadata.loadGamesFromAllSources ()
    |> Async.RunSynchronously


List.iter (fun f -> f allGames) [
    assertStringNotEmpty "baidu_folder不能为空" _.BaiduFolder
    assertUnique' "baidu_folder字段" _.BaiduFolder
    assertUnique' "download_link字段" _.DownloadLink
    assertUnique' "game_id字段" _.GameID
    assertMember "platforms字段不能为空列表" (not << Set.isEmpty << _.Platforms)
    assertStringNotEmpty "title字段不能为空" _.Title
]


if hasError then abort ()


printfn "验证通过。"

#load "./pymo-index-utils.fsx"

open PyMOIndexUtils
open System.Collections.Generic


let mutable hasError = false


let checkUnique (memberName: string) (f: 'a -> 'b option) (toCheck: 'a seq) =
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


let checkUnique' memberName f toCheck =
    checkUnique memberName (f >> Some) toCheck


let assertMember (errInfo: string) (f: 'a -> bool) (toCheck: 'a seq) =
    for itemToCheck in toCheck do
        if not <| f itemToCheck then
            printfn "以下对象不满足条件“%s”，这是不被允许的：" errInfo
            printfn "%A" itemToCheck
            printfn ""
            printfn ""
            hasError <- true

checkUnique' "源ID" _.Id Source.sources
checkUnique' "源URL" _.Url Source.sources


let abort () =
    printfn "未能通过验证。"
    exit -1


if hasError then abort ()


let allGames =
    GameMetadata.loadGamesFromAllSources ()
    |> Async.RunSynchronously


checkUnique' "baidu_folder字段" _.BaiduFolder allGames
checkUnique' "download_link字段" _.DownloadLink allGames
checkUnique' "game_id字段" _.GameID allGames


if hasError then abort ()

printfn "验证通过。"

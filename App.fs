module App

open Suave
open Suave.Successful 
open Suave.Filters
open Suave.Operators
open System
open System.IO
open ItemRandomizer

type Context =
    {
        Error: bool;
        Info: string;
        Mode: string;
    }

let index =
    DotLiquid.page "index.html" ()

let information = 
    DotLiquid.page "information.html" ()

let contact = 
    DotLiquid.page "contact.html" ()

let randomizeGet =
    DotLiquid.page "randomize.html" ()

let randomizerPost (r:HttpRequest) =
    match r.files.Length with
        | 0 -> DotLiquid.page "randomize.html" { Error = true; Info = "No file uploaded"; Mode = "POST" }
        | _ -> 
            let file = r.files.Head
            let (_,  inputSeed) = r.multiPartFields.Head
            let (_,  inputDifficulty) = r.multiPartFields.Tail.Head;
            let bytes = 
                use binaryReader = new BinaryReader(File.Open(file.tempFilePath, FileMode.Open))
                binaryReader.ReadBytes(int binaryReader.BaseStream.Length)
            try
                let difficulty = enum<Types.Difficulty>(Int32.Parse inputDifficulty)
                let ipsPatches = List.filter (fun (p:Types.IpsPatch) -> ((p.Difficulty = difficulty || p.Difficulty = Types.Difficulty.Any) && p.Default)) Patches.IpsPatches
                let patches = List.filter (fun (p:Types.Patch) -> ((p.Difficulty = difficulty || p.Difficulty = Types.Difficulty.Any) && p.Default)) Patches.RomPatches
                let (seed, binaryData) = Randomizer.Randomize (Int32.Parse inputSeed) difficulty false "" bytes ipsPatches patches
                let newFileName = sprintf "Item Randomizer %s%d.sfc" (match difficulty with
                                                                        | Types.Difficulty.Casual -> "CX"
                                                                        | Types.Difficulty.Normal -> "X"
                                                                        | Types.Difficulty.Hard -> "HX"
                                                                        | _ -> "X") seed
                                                                                        
                if File.Exists(file.tempFilePath) then File.Delete(file.tempFilePath) else ()
                Writers.setMimeType("application/octet-stream")
                >=> Writers.setHeader "Content-Disposition" (sprintf "attachment; filename=\"%s\"" newFileName) 
                >=> (Successful.ok <| binaryData)
            with
                | _ -> DotLiquid.page "randomize.html" { Error = true; Info = "Couldn't patch the ROM, did you upload the correct file?"; Mode = "POST" }
                
let Router = 
    choose [
        path "/" >=> index
        path "/randomize" >=> choose
            [
                POST >=> request randomizerPost
                GET >=> randomizeGet
            ]
        path "/information" >=> information
        path "/contact" >=> contact
        GET >=> path "/favicon.ico" >=> Files.browseFile __SOURCE_DIRECTORY__ "favicon.ico"
        GET >=> Files.browse (Path.GetFullPath "./static")
        RequestErrors.NOT_FOUND "Page not found"
    ]

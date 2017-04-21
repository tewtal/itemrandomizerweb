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

let randomizerPost (r:HttpRequest) =
    match r.files.Length with
        | 0 -> DotLiquid.page "index.html" { Error = true; Info = "No file uploaded"; Mode = "POST" }
        | _ -> 
            let file = r.files.Head
            let bytes = 
                use binaryReader = new BinaryReader(File.Open(file.tempFilePath, FileMode.Open))
                binaryReader.ReadBytes(int binaryReader.BaseStream.Length)
            try
                let (seed, binaryData) = Randomizer.Randomize 0 1 false "" bytes
                let newFileName = sprintf "Item Randomizer X%d.sfc" seed                
                if File.Exists(file.tempFilePath) then File.Delete(file.tempFilePath) else ()
                Writers.setMimeType("application/octet-stream")
                >=> Writers.setHeader "Content-Disposition" (sprintf "attachment; filename=\"%s\"" newFileName) 
                >=> (Successful.ok <| binaryData)
            with
                | _ -> DotLiquid.page "index.html" { Error = true; Info = "Couldn't patch the ROM, did you upload the correct file?"; Mode = "POST" }
                
let Router = 
    choose [
        path "/" >=> index
        path "/randomizer" >=> choose
            [
                POST >=> request (fun r -> randomizerPost r)
            ]
    ]

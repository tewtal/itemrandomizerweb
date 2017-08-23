namespace ItemRandomizer

module Randomizer =
    open Types
    open System
    open System.IO

    let mutable (SelectedLocations:Location list) = []

    let writeRom data fileName seed =
        File.WriteAllBytes(fileName, data)
        seed

    let rec writeLocations (data:byte []) itemLocations =
        match itemLocations with
        | itemLocation :: tail -> 
            let itemCode = Items.getItemTypeCode itemLocation.Item itemLocation.Location.Visibility
            data.[itemLocation.Location.Address] <- itemCode.[0]
            data.[itemLocation.Location.Address + 1] <- itemCode.[1]            
            writeLocations data tail
        | [] -> data
    

    let writeSpoiler seed spoiler fileName itemLocations = 
        if spoiler then
            use writer = File.CreateText(__SOURCE_DIRECTORY__ + (sprintf "/logs/%d.spoiler.txt" seed))
            List.map (fun itemLocation -> writer.WriteLine(String.Format("{0} -> {1}", itemLocation.Location.Name, itemLocation.Item.Name))) (List.sortBy (fun itemLocation -> itemLocation.Location.Address) itemLocations) |> ignore

        itemLocations

    let rec writeRomSpoiler (data:byte []) itemLocations address =
        match itemLocations with
        | [] -> 
            Patches.patchByte data address [0; 0; 0; 0]
        | head :: tail ->
            let itemName = System.Text.RegularExpressions.Regex.Replace(head.Item.Name.ToUpper(), "[^A-Z0-9\.,'!: ]+", "", System.Text.RegularExpressions.RegexOptions.Compiled)
            let locationName = System.Text.RegularExpressions.Regex.Replace(head.Location.Name.ToUpper(), "[^A-Z0-9\.,'!: ]+", "", System.Text.RegularExpressions.RegexOptions.Compiled)
            
            let itemName = " " + itemName.Substring(0, (if itemName.Length > 30 then 30 else itemName.Length)).ToUpper().Trim().PadRight(31, ' ')
            let locationName = " " + locationName.Substring(0, (if locationName.Length > 30 then 30 else locationName.Length)).ToUpper().PadLeft(30, '.') + " "
            
            Patches.writeCreditsString data address 0x04 itemName |> ignore
            Patches.writeCreditsString data (address + 0x40) 0x18 locationName |> ignore
            writeRomSpoiler data tail (address + 0x80)

    let rec writeItemNames (data:byte []) items =
        match items with
        | [] -> data
        | item :: tail ->
            let name = "PLACEHOLDER ITEM"
            let itemName = System.Text.RegularExpressions.Regex.Replace(name.ToUpper(), "[^A-Z0-9\.\?,'! ]+", "", System.Text.RegularExpressions.RegexOptions.Compiled)
            let itemName = itemName.Substring(0, (if itemName.Length > 19 then 19 else itemName.Length))
            let nameLen = itemName.Length
            let padl = ((19 - nameLen)/2)
            let itemName = itemName.PadLeft(padl+nameLen, ' ').PadRight(19, ' ')

            Patches.writeMessageString data item.Message itemName |> ignore
            writeItemNames data tail
    
    
    let randomizeItems randomizer (data:byte []) seed spoiler fileName locationPool =
        let rnd = Random(seed)

        let seedInfo = rnd.Next(0xFFFF)
        let seedInfo2 = rnd.Next(0xFFFF)
        let seedInfoArr = Items.toByteArray seedInfo
        let seedInfoArr2 = Items.toByteArray seedInfo2
        
        data.[0x2FFF00] <- seedInfoArr.[0]
        data.[0x2FFF01] <- seedInfoArr.[1]
        data.[0x2FFF02] <- seedInfoArr2.[0]
        data.[0x2FFF03] <- seedInfoArr2.[1]

        let rnd = Random(seed)
        let itemLocations = writeSpoiler seed spoiler fileName (randomizer rnd [] [] (Items.getItemPool rnd) locationPool)
        writeRomSpoiler data (List.sortBy (fun il -> il.Item.Type) (List.filter (fun il -> il.Item.Class = Major && il.Item.Type <> ETank && il.Item.Type <> Reserve) itemLocations)) 0x2f5240 |> ignore
        //writeItemNames data Items.Items |> ignore
        writeLocations data itemLocations        

    let Randomize inputSeed difficulty spoiler fileName baseRom ipsPatches patches =
        let seed = match inputSeed with
                   | 0 -> Random().Next(1000000, 9999999)
                   | _ -> inputSeed
        
        let locationPool = match difficulty with
                            | Difficulty.Casual -> CasualLocations.AllLocations
                            | Difficulty.Normal -> Locations.AllLocations
                            | Difficulty.Hard -> HardLocations.AllLocations
                            | Difficulty.Tournament -> TournamentLocations.AllLocations
                            | Difficulty.Open -> OpenLocations.AllLocations
                            | _ -> Locations.AllLocations

        let generateItems = match difficulty with
                             | Difficulty.Hard -> SparseRandomizer.generateItems
                             | Difficulty.Open -> OpenRandomizer.generateItems
                             | _ -> NewRandomizer.generateItems
        
        // Get a random animal patch
        let rnd = Random(seed)
        let animalsPatches = (List.filter (fun ip -> ip.PatchType = Animals) Patches.IpsPatches)
        let animalsPatch = List.item (rnd.Next animalsPatches.Length) animalsPatches

        //let expandedRom = Array.create<byte> ((Array.length baseRom) + 0xF8000) 0xFFuy
        //System.Buffer.BlockCopy(baseRom, 0, expandedRom, 0, baseRom.Length)

        (seed, randomizeItems generateItems (Patches.ApplyPatches (animalsPatch :: ipsPatches) patches baseRom) seed spoiler fileName locationPool)

    let TestRandomize =
        let mutable itemLocations:(ItemLocation list) = []
        for i in 1 .. 1000 do
            let seed = Random().Next(1000000, 9999999)
            let rnd = Random(seed)
            let newLocations = 
                try 
                    NewRandomizer.generateItems rnd [] [] (Items.getItemPool rnd) TournamentLocations.AllLocations
                with
                | _ -> []

            itemLocations <- List.append itemLocations (List.filter (fun (i:ItemLocation) -> i.Item.Class = Major) newLocations)
        itemLocations

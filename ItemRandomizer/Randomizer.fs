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

    let randomizeItems randomizer (data:byte []) seed spoiler fileName locationPool =
        let rnd = Random(seed)

        let seedInfo = rnd.Next(0xFFFF)
        let seedInfo2 = rnd.Next(0xFFFF)
        let seedInfoArr = Items.toByteArray seedInfo
        let seedInfoArr2 = Items.toByteArray seedInfo2
        
        data.[0x2FF000] <- seedInfoArr.[0]
        data.[0x2FF001] <- seedInfoArr.[1]
        data.[0x2FF002] <- seedInfoArr2.[0]
        data.[0x2FF003] <- seedInfoArr2.[1]

        let rnd = Random(seed)
        writeLocations data (writeSpoiler seed spoiler fileName (randomizer rnd [] [] (Items.getItemPool rnd) locationPool))
        

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
                             | _ -> DefaultRandomizer.generateItems
        
        (seed, randomizeItems generateItems (Patches.ApplyPatches ipsPatches patches baseRom) seed spoiler fileName locationPool)

    let TestRandomize =
        let mutable itemLocations:(ItemLocation list) = []
        for i in 1 .. 1000 do
            let seed = Random().Next(1000000, 9999999)
            let rnd = Random(seed)
            let newLocations = 
                try 
                    DefaultRandomizer.generateItems rnd [] [] (Items.getItemPool rnd) TournamentLocations.AllLocations
                with
                | _ -> []

            itemLocations <- List.append itemLocations (List.filter (fun (i:ItemLocation) -> i.Item.Class = Major) newLocations)
        itemLocations

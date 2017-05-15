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
    
    let unusedLocation (location:Location) itemLocations = 
        not (List.exists (fun itemLocation -> itemLocation.Location.Address = location.Address) itemLocations)

    let currentLocations items itemLocations = 
        List.filter (fun location -> location.Available items && unusedLocation location itemLocations ) SelectedLocations
    
    let canPlaceAtLocation (item:Item) (location:Location) =
        location.Class = item.Class &&
        (match item.Type with
        | Gravity -> (not (location.Area = Crateria || location.Area = Brinstar)) || location.Name = "X-Ray Visor" || location.Name = "Energy Tank (pink Brinstar bottom)"
        | Varia -> (not (location.Area = LowerNorfair || location.Area = Crateria))
        | _ -> true)
        
    let canPlaceItem (item:Item) itemLocations =
        List.exists (fun location -> canPlaceAtLocation item location) itemLocations

    let checkItem item items (itemLocations:ItemLocation list) =
        let oldLocations = (currentLocations items itemLocations)
        let newLocations = (currentLocations (item :: items) itemLocations)
        canPlaceItem item oldLocations && (List.length newLocations) > (List.length oldLocations)
    
    let possibleItems items itemLocations itemPool =
        List.filter (fun item -> checkItem item items itemLocations) itemPool

    let placeItem (rnd:Random) (items:Item list) (itemPool:Item list) locations =
        let item = match List.length items with
                   | 0 -> List.item (rnd.Next (List.length itemPool)) itemPool
                   | _ -> List.item (rnd.Next (List.length items)) items
        
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }
    
    let rec removeItem itemType itemPool =
        match itemPool with
        | head :: tail -> if head.Type = itemType then tail else head :: removeItem itemType tail
        | [] -> itemPool
    
    let rec generateItems rnd items itemLocations itemPool =
        match itemPool with
        | [] -> itemLocations
        | _ ->
            let itemLocation = placeItem rnd (possibleItems items itemLocations itemPool) itemPool (currentLocations items itemLocations)
            generateItems rnd (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool)

    let writeSpoiler seed spoiler fileName itemLocations = 
        if spoiler then
            let writer = File.CreateText(fileName + ".spoiler.txt")
            List.map (fun itemLocation -> writer.WriteLine(String.Format("{0} -> {1}", itemLocation.Location.Name, itemLocation.Item.Name))) (List.sortBy (fun itemLocation -> itemLocation.Location.Address) itemLocations) |> ignore
        
        itemLocations

    let randomizeItems (data:byte []) seed spoiler fileName =
        let rnd = Random(seed)
        writeLocations data (writeSpoiler seed spoiler fileName (generateItems rnd [] [] (Items.getItemPool rnd)))

    let Randomize inputSeed difficulty spoiler fileName baseRom ipsPatches patches =
        let seed = match inputSeed with
                   | 0 -> Random().Next(1000000, 9999999)
                   | _ -> inputSeed
        
        match difficulty with
            | Difficulty.Casual -> SelectedLocations <- CasualLocations.AllLocations
            | Difficulty.Normal -> SelectedLocations <- Locations.AllLocations
            | _ -> SelectedLocations <- Locations.AllLocations
        
        (seed, (Patches.ApplyPatches ipsPatches patches (randomizeItems baseRom seed spoiler fileName)))


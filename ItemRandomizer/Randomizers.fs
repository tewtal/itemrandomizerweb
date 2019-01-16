namespace ItemRandomizer

module DefaultRandomizer =
    open Types
    open System
    
    let unusedLocation (location:Location) itemLocations = 
        not (List.exists (fun itemLocation -> itemLocation.Location.Address = location.Address) itemLocations)

    let currentLocations items itemLocations locationPool = 
        List.filter (fun location -> location.Available items && unusedLocation location itemLocations ) locationPool
    
    let canPlaceAtLocation (item:Item) (location:Location) =
        location.Class = item.Class &&
        (match item.Type with
        | Gravity -> (not (location.Area = Crateria || location.Area = Brinstar)) || location.Name = "X-Ray Scope" || location.Name = "Energy Tank, Waterway"
        | Varia -> (not (location.Area = LowerNorfair || location.Area = Crateria || location.Name = "Morphing Ball" || location.Name = "Missile (blue Brinstar middle)" || location.Name = "Energy Tank, Brinstar Ceiling"))
        | SpeedBooster -> not (location.Name = "Morphing Ball" || location.Name = "Missile (blue Brinstar middle)" || location.Name = "Energy Tank, Brinstar Ceiling")
        | ScrewAttack -> not (location.Name = "Morphing Ball" || location.Name = "Missile (blue Brinstar middle)" || location.Name = "Energy Tank, Brinstar Ceiling")
        | _ -> true)

    let canPlaceItem (item:Item) itemLocations =
        List.exists (fun location -> canPlaceAtLocation item location) itemLocations

    let checkItem item items (itemLocations:ItemLocation list) locationPool =
        let oldLocations = (currentLocations items itemLocations locationPool)
        let newLocations = (currentLocations (item :: items) itemLocations locationPool)
        let newLocationsHasMajor = List.exists (fun l -> l.Class = Major) newLocations
        canPlaceItem item oldLocations && newLocationsHasMajor && (List.length newLocations) > (List.length oldLocations)
    
    let possibleItems items itemLocations itemPool locationPool =
        List.filter (fun item -> checkItem item items itemLocations locationPool) itemPool
    
    let rec removeItem itemType itemPool =
        match itemPool with
        | head :: tail -> if head.Type = itemType then tail else head :: removeItem itemType tail
        | [] -> itemPool

    let placeItem (rnd:Random) (items:Item list) (itemPool:Item list) locations =
        let item = match List.length items with
                   | 0 -> List.item (rnd.Next (List.length itemPool)) itemPool
                   | _ -> List.item (rnd.Next (List.length items)) items
        
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }

    let rec generateItems rnd items itemLocations itemPool locationPool =
        match itemPool with
        | [] -> itemLocations
        | _ ->
            let itemLocation = placeItem rnd (possibleItems items itemLocations itemPool locationPool) itemPool (currentLocations items itemLocations locationPool)
            generateItems rnd (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool


module SparseRandomizer =
    open Types
    open System
    
    let unusedLocation (location:Location) itemLocations = 
        not (List.exists (fun itemLocation -> itemLocation.Location.Address = location.Address) itemLocations)

    let currentLocations items itemLocations locationPool = 
        List.filter (fun location -> location.Available items && unusedLocation location itemLocations ) locationPool
    
    let canPlaceAtLocation (item:Item) (location:Location) =
        location.Class = item.Class &&
        (match item.Type with
        | Gravity -> (not (location.Area = Crateria || location.Area = Brinstar)) || location.Name = "X-Ray Scope" || location.Name = "Energy Tank, Waterway"
        | Varia -> (not (location.Area = Crateria))
        | _ -> true)

    let canPlaceItem (item:Item) itemLocations =
        List.exists (fun location -> canPlaceAtLocation item location) itemLocations

    let checkItem item items (itemLocations:ItemLocation list) locationPool =
        let oldLocations = (currentLocations items itemLocations locationPool)
        let newLocations = (currentLocations (item :: items) itemLocations locationPool)
        let newLocationsHasMajor = List.exists (fun (l:Location) -> l.Class = Major && not (List.exists (fun (ol:Location) -> ol.Address = l.Address) oldLocations)) newLocations
        canPlaceItem item oldLocations && (newLocationsHasMajor || item.Class = Major) && (List.length newLocations) > (List.length oldLocations)

    let checkFillerItem item items (itemLocations:ItemLocation list) locationPool =
        let oldLocations = (currentLocations items itemLocations locationPool)
        canPlaceItem item oldLocations

    let getNewLocations item items (itemLocations:ItemLocation list) locationPool =
        let oldLocations = (currentLocations items itemLocations locationPool)
        let newLocations = (currentLocations (item :: items) itemLocations locationPool)
        match item.Type with 
        | Varia -> (((List.length newLocations) - (List.length oldLocations)) + 1) * 3
        | Gravity -> (((List.length newLocations) - (List.length oldLocations)) + 1) * 4
        | Super -> ((List.length newLocations) - (List.length oldLocations)) + 1
        | PowerBomb -> ((List.length newLocations) - (List.length oldLocations)) + 2
        | _ -> ((List.length newLocations) - (List.length oldLocations))

    let possibleItems items itemLocations itemPool locationPool =
        List.sortBy (fun item -> getNewLocations item items itemLocations locationPool) (List.filter (fun item -> checkItem item items itemLocations locationPool) itemPool)

    let possibleFillerItems items itemLocations itemPool locationPool =
        List.filter (fun item -> checkFillerItem item items itemLocations locationPool) itemPool
    
    let rec removeItem itemType itemPool =
        match itemPool with
        | head :: tail -> if head.Type = itemType then tail else head :: removeItem itemType tail
        | [] -> itemPool

    let placeItem (rnd:Random) (items:Item list) (itemPool:Item list) locations =
        let item = (match List.length items with
                    | 0 -> List.item (rnd.Next (List.length itemPool)) itemPool
                    | _ -> List.item (((rnd.Next (List.length items))+1)/2) items)
    
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations

        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }

    
    let placeFiller (rnd:Random) (items:Item list) (itemPool:Item list) (itemLocations:ItemLocation list) locations =
        let sortedList = List.sortBy (fun (i:Item) -> getNewLocations i items itemLocations locations) items
        let item = sortedList.Head
        
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }

    let rec fillItems rnd items itemLocations itemPool locationPool =
        let initialLocations = (currentLocations items itemLocations locationPool)
        let possibleItems = (possibleFillerItems items itemLocations itemPool locationPool)
        match List.length possibleItems with
            | 0 -> (items, itemLocations, itemPool)
            | _ ->
                let itemLocation = placeFiller rnd possibleItems itemPool itemLocations initialLocations
                fillItems rnd (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool 


    let rec generateItems rnd items itemLocations itemPool locationPool =
        match itemPool with
        | [] -> itemLocations
        | _ ->
            // First we get a random item that will advance the progress
            let initialLocations = (currentLocations items itemLocations locationPool)
            let itemLocation = placeItem rnd (possibleItems items itemLocations itemPool locationPool) itemPool initialLocations

            // Now fill all other spots with "trash"
            let fillLocations = List.filter (fun (l:Location) -> l.Address <> itemLocation.Location.Address) initialLocations
            let (newItems, newItemLocations, newItemPool) = fillItems rnd (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) fillLocations
            
            generateItems rnd newItems newItemLocations newItemPool locationPool

module OpenRandomizer =
    open Types
    open System
    
    let unusedLocation (location:Location) itemLocations = 
        not (List.exists (fun itemLocation -> itemLocation.Location.Address = location.Address) itemLocations)

    let currentLocations items itemLocations locationPool = 
        List.filter (fun location -> location.Available items && unusedLocation location itemLocations ) locationPool
    
    let canPlaceAtLocation (item:Item) (location:Location) =
        location.Class = item.Class &&
        (match item.Type with
        | Gravity -> (not (location.Name = "Morphing Ball" || location.Name = "Energy Tank, Brinstar Ceiling" || location.Name = "Bomb" || location.Name = "Energy Tank, Terminator" || location.Name = "Reserve Tank, Brinstar" || location.Name = "Charge Beam" || location.Name = "Energy Tank, Brinstar Gate"))
        | Varia -> (not (location.Area = LowerNorfair || location.Name = "Morphing Ball" || location.Name = "Energy Tank, Brinstar Ceiling" || location.Name = "Bomb" || location.Name = "Energy Tank, Terminator" || location.Name = "Reserve Tank, Brinstar" || location.Name = "Charge Beam" || location.Name = "Energy Tank, Brinstar Gate"))
        | Morph -> (not (location.Name = "Morphing Ball" || location.Name = "Energy Tank, Brinstar Ceiling" || location.Name = "Bomb" || location.Name = "Energy Tank, Terminator" || location.Name = "Reserve Tank, Brinstar" || location.Name = "Charge Beam" || location.Name = "Energy Tank, Brinstar Gate"))
        | _ -> true)

    let canPlaceItem (item:Item) itemLocations =
        List.exists (fun location -> canPlaceAtLocation item location) itemLocations

    let checkItem item items (itemLocations:ItemLocation list) locationPool =
        let oldLocations = (currentLocations items itemLocations locationPool)
        let newLocations = (currentLocations (item :: items) itemLocations locationPool)
        let newLocationsHasMajor = List.exists (fun l -> l.Class = Major) newLocations
        canPlaceItem item oldLocations && newLocationsHasMajor && (List.length newLocations) > (List.length oldLocations)
    
    let possibleItems items itemLocations itemPool locationPool =
        List.filter (fun item -> checkItem item items itemLocations locationPool) itemPool
    
    let rec removeItem itemType itemPool =
        match itemPool with
        | head :: tail -> if head.Type = itemType then tail else head :: removeItem itemType tail
        | [] -> itemPool

    let placeItem (rnd:Random) (items:Item list) (itemPool:Item list) locations =
        let item = match List.length items with
                   | 0 -> List.item (rnd.Next (List.length itemPool)) itemPool
                   | _ -> List.item (rnd.Next (List.length items)) items
        
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }
    
    let placeSpecificItem (rnd:Random) item (itemPool:Item list) locations =
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }

    let placeSpecificItemAtLocation item location =
        { Item = item; Location = location }
    
    let getEmptyLocations itemLocations (locationPool:Location list) = 
        List.filter (fun (l:Location) -> unusedLocation l itemLocations) locationPool

    let getItem itemType =
        List.find (fun i -> i.Type = itemType) Items.Items

    let getAssumedItems item itemPool = 
        let items = removeItem item.Type itemPool
        getItem Missile :: (getItem Super :: (getItem PowerBomb :: (getItem ETank :: (getItem ETank :: (getItem ETank :: items)))))

    let rec generateAssumedItems items (itemLocations:ItemLocation list) (itemPool:Item list) locationPool =
        if not (List.exists (fun (l:Item) -> l.Category = Progression) itemPool) then
            (items, itemLocations, itemPool)
        else
            match itemPool with
            | [] -> (items, itemLocations, itemPool)
            | _ ->
                let item = List.head (List.filter (fun (i:Item) -> i.Category = Progression) itemPool)
                let assumedItems = getAssumedItems item itemPool
                let availableLocations = List.filter (fun l -> l.Available assumedItems && canPlaceAtLocation item l) (getEmptyLocations itemLocations locationPool)
                let fillLocation = List.head availableLocations

                let itemLocation = placeSpecificItemAtLocation item fillLocation
                generateAssumedItems (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool
            

    let swap (a: _[]) x y =
        let tmp = a.[x]
        a.[x] <- a.[y]
        a.[y] <- tmp
    
    let shuffle (rnd:Random) a =
        Array.iteri (fun i _ -> swap a i (rnd.Next(i, Array.length a))) a

    let rec generateMoreItems rnd items itemLocations itemPool locationPool =
        match itemPool with
        | [] -> itemLocations
        | _ ->            
            let itemLocation = placeItem rnd (possibleItems items itemLocations itemPool locationPool) itemPool (currentLocations items itemLocations locationPool)
            generateMoreItems rnd (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool

    let generateItems (rnd:Random) items itemLocations (itemPool:Item list) (locationPool:Location list) =        
        let mutable newItems = []
        let mutable newItemPool = []
        let mutable newItemLocations = []
        
        let itemPoolArray = List.toArray itemPool
        let locationPoolArray = List.toArray locationPool
        shuffle rnd itemPoolArray
        shuffle rnd locationPoolArray
        let randomizedItemPool = Array.toList itemPoolArray
        let randomizedLocationPool = Array.toList locationPoolArray

        // First we place a missile, super missile, power bomb and 3 energy tanks at always accessible locations
        let missileLocations = List.filter (fun f -> f.Class = Minor) (currentLocations items itemLocations randomizedLocationPool)
        let missile = List.find (fun f -> f.Type = Missile) randomizedItemPool
        let itemLocation = placeSpecificItem rnd missile randomizedItemPool missileLocations
        
        newItems <- itemLocation.Item :: items
        newItemPool <- removeItem itemLocation.Item.Type randomizedItemPool
        newItemLocations <- itemLocation :: itemLocations
        
        let superLocations = List.filter (fun f -> f.Class = Minor) (currentLocations newItems newItemLocations randomizedLocationPool)
        let super = List.find (fun f -> f.Type = Super) newItemPool
        let itemLocation = placeSpecificItem rnd super newItemPool superLocations

        newItems <- itemLocation.Item :: items
        newItemPool <- removeItem itemLocation.Item.Type randomizedItemPool
        newItemLocations <- itemLocation :: itemLocations
        
        let pbLocations = List.filter (fun f -> f.Class = Minor) (currentLocations newItems newItemLocations randomizedLocationPool)
        let pb = List.find (fun f -> f.Type = PowerBomb) newItemPool
        let itemLocation = placeSpecificItem rnd pb newItemPool pbLocations
        
        newItems <- itemLocation.Item :: items
        newItemPool <- removeItem itemLocation.Item.Type randomizedItemPool
        newItemLocations <- itemLocation :: itemLocations
        
        let etLocations = List.filter (fun f -> f.Class = Major) (currentLocations newItems newItemLocations randomizedLocationPool)
        let et = List.find (fun f -> f.Type = ETank) newItemPool
        let itemLocation = placeSpecificItem rnd et newItemPool etLocations

        newItems <- itemLocation.Item :: items
        newItemPool <- removeItem itemLocation.Item.Type randomizedItemPool
        newItemLocations <- itemLocation :: itemLocations

        let etLocations = List.filter (fun f -> f.Class = Major) (currentLocations newItems newItemLocations randomizedLocationPool)
        let et = List.find (fun f -> f.Type = ETank) newItemPool
        let itemLocation = placeSpecificItem rnd et newItemPool etLocations

        newItems <- itemLocation.Item :: items
        newItemPool <- removeItem itemLocation.Item.Type randomizedItemPool
        newItemLocations <- itemLocation :: itemLocations

        let etLocations = List.filter (fun f -> f.Class = Major) (currentLocations newItems newItemLocations randomizedLocationPool)
        let et = List.find (fun f -> f.Type = ETank) newItemPool
        let itemLocation = placeSpecificItem rnd et newItemPool etLocations

        newItems <- itemLocation.Item :: items
        newItemPool <- removeItem itemLocation.Item.Type randomizedItemPool
        newItemLocations <- itemLocation :: itemLocations
        
        // Place progression items 
        let (progressItems, progressItemLocations, progressItemPool) = generateAssumedItems newItems newItemLocations newItemPool randomizedLocationPool
        
        // Fill the rest
        generateMoreItems rnd progressItems progressItemLocations progressItemPool randomizedLocationPool


module NewRandomizer =
    open Types
    open System
    
    let unusedLocation (location:Location) itemLocations = 
        not (List.exists (fun itemLocation -> itemLocation.Location.Address = location.Address) itemLocations)

    let currentLocations items itemLocations locationPool = 
        List.filter (fun location -> location.Available items && unusedLocation location itemLocations ) locationPool
    
    let canPlaceAtLocation (item:Item) (location:Location) =
        location.Class = item.Class &&
        (match item.Type with
        | Gravity -> (not (location.Area = Crateria || location.Area = Brinstar)) || location.Name = "X-Ray Scope" || location.Name = "Energy Tank, Waterway"
        | Varia -> (not (location.Area = LowerNorfair || location.Area = Crateria || location.Name = "Morphing Ball" || location.Name = "Missile (blue Brinstar middle)" || location.Name = "Energy Tank, Brinstar Ceiling"))
        | SpeedBooster -> not (location.Name = "Morphing Ball" || location.Name = "Missile (blue Brinstar middle)" || location.Name = "Energy Tank, Brinstar Ceiling")
        | ScrewAttack -> not (location.Name = "Morphing Ball" || location.Name = "Missile (blue Brinstar middle)" || location.Name = "Energy Tank, Brinstar Ceiling")
        | _ -> true)

    let canPlaceItem (item:Item) itemLocations =
        List.exists (fun location -> canPlaceAtLocation item location) itemLocations

    let checkItem item items (itemLocations:ItemLocation list) locationPool =
        let oldLocations = (currentLocations items itemLocations locationPool)
        let newLocations = (currentLocations (item :: items) itemLocations locationPool)
        let newLocationsHasMajor = List.exists (fun l -> l.Class = Major) newLocations
        canPlaceItem item oldLocations && newLocationsHasMajor && (List.length newLocations) > (List.length oldLocations)
    
    let possibleItems items itemLocations itemPool locationPool =
        List.filter (fun item -> checkItem item items itemLocations locationPool) itemPool
    
    let rec removeItem itemType itemPool =
        match itemPool with
        | head :: tail -> if head.Type = itemType then tail else head :: removeItem itemType tail
        | [] -> itemPool

    let placeItem (rnd:Random) (items:Item list) (itemPool:Item list) locations =
        let item = match List.length items with
                   | 0 -> 
                          if (List.exists (fun i -> i.Type = ScrewAttack) itemPool) then (List.find (fun i -> i.Type = ScrewAttack) itemPool)
                          elif (List.exists (fun i -> i.Type = SpeedBooster) itemPool) then (List.find (fun i -> i.Type = SpeedBooster) itemPool)
                          else List.item (rnd.Next (List.length itemPool)) itemPool
                   | _ -> List.item (rnd.Next (List.length items)) items
        
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }
    
    let placeSpecificItem (rnd:Random) item (itemPool:Item list) locations =
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }

    let placeSpecificItemAtLocation item location =
        { Item = item; Location = location }
    
    let getEmptyLocations itemLocations (locationPool:Location list) = 
        List.filter (fun (l:Location) -> unusedLocation l itemLocations) locationPool

    let getItem itemType =
        List.find (fun i -> i.Type = itemType) Items.Items

    let getAssumedItems item prefilledItems itemLocations itemPool = 
        let items = removeItem item.Type itemPool
        let items = List.append items prefilledItems
        let accessibleItems = List.map (fun i -> i.Item) (List.filter (fun il -> (il.Location.Available items) && (not (List.exists (fun k -> k.Type = il.Item.Type) prefilledItems))) itemLocations)
        List.append items accessibleItems

    let rec generateAssumedItems prefilledItems items (itemLocations:ItemLocation list) (itemPool:Item list) locationPool =
        if not (List.exists (fun (l:Item) -> l.Category = Progression) itemPool) then
            (items, itemLocations, itemPool)
        else
            match itemPool with
            | [] -> (items, itemLocations, itemPool)
            | _ ->
                let item = List.head (List.filter (fun (i:Item) -> i.Category = Progression) itemPool)
                let assumedItems = getAssumedItems item prefilledItems itemLocations itemPool
                let availableLocations = List.filter (fun l -> l.Available assumedItems && canPlaceAtLocation item l) (getEmptyLocations itemLocations locationPool)
                let fillLocation = List.head availableLocations

                let itemLocation = placeSpecificItemAtLocation item fillLocation
                generateAssumedItems prefilledItems (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool
            

    let swap (a: _[]) x y =
        let tmp = a.[x]
        a.[x] <- a.[y]
        a.[y] <- tmp
    
    let shuffle (rnd:Random) a =
        Array.iteri (fun i _ -> swap a i (rnd.Next(i, Array.length a))) a

    let rec generateMoreItems rnd items itemLocations itemPool locationPool =
        match itemPool with
        | [] -> itemLocations
        | _ ->            
            let itemLocation = placeItem rnd (possibleItems items itemLocations itemPool locationPool) itemPool (currentLocations items itemLocations locationPool)
            generateMoreItems rnd (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool

    let rec getWeightedLocations (locationPool:Location list) num (locations:Map<int, Location>) =
        match locationPool with
        | [] -> List.map (fun (k,v) -> v) (List.sortBy (fun (k,v) -> k) (Map.toList locations))
        | loc :: tail ->
            let weight = num - (match loc.Area with
                                | Brinstar -> 0
                                | Crateria -> 0
                                | LowerNorfair -> 11
                                | Maridia -> 0
                                | Norfair -> 0
                                | WreckedShip -> 12)
            let locations = locations.Add(weight, loc)
            getWeightedLocations (List.filter (fun l -> l.Address <> loc.Address) locationPool) (num + 10) locations

    let prefill (rnd:Random) (itemType:ItemType) (items:Item list byref) (itemLocations:ItemLocation list byref) (itemPool:Item list byref) (locationPool: Location list) =
        let item = List.find (fun i -> i.Type = itemType) Items.Items
        let cl = List.filter (fun l -> l.Class = item.Class && canPlaceAtLocation item l) (currentLocations items itemLocations locationPool)
        let itemLocation = placeSpecificItemAtLocation item (List.item (rnd.Next (List.length cl)) cl)
        items <- itemLocation.Item :: items
        itemPool <- removeItem itemLocation.Item.Type itemPool
        itemLocations <- itemLocation :: itemLocations

    let generateItems (rnd:Random) (items:Item list) (itemLocations:ItemLocation list) (itemPool:Item list) (locationPool:Location list) =
        let mutable newItems = items
        let mutable newItemLocations = itemLocations
        let mutable newItemPool = itemPool

        // Place Morph at one of the earliest locations so that it's always accessible
        prefill rnd Morph &newItems &newItemLocations &newItemPool locationPool
        
        // Place either a super or a missile to open up BT's location 
        match rnd.Next(2) with
        | 0 -> prefill rnd Missile &newItems &newItemLocations &newItemPool locationPool
        | _ -> prefill rnd Super &newItems &newItemLocations &newItemPool locationPool

        // Next step is to place items that opens up access to breaking bomb blocks
        // by placing either Screw/Speed/Bomb or just a PB pack early.
        // One PB pack will be placed after filling with other items so that there's at least on accessible
        match rnd.Next(13) with
        | 0 ->                          prefill rnd ScrewAttack &newItems &newItemLocations &newItemPool locationPool
        | 1 ->                          prefill rnd SpeedBooster &newItems &newItemLocations &newItemPool locationPool
        | 2 ->                          prefill rnd Bomb &newItems &newItemLocations &newItemPool locationPool
        | _ ->                          prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool
        
        // Place a super if it's not already placed
        if not (List.exists (fun i -> i.Type = Super) newItems) then
            prefill rnd Super &newItems &newItemLocations &newItemPool locationPool

        // Place two power bombs if it's not already placed, otherwise one
        if not (List.exists (fun i -> i.Type = PowerBomb) newItems) then
            prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool
            prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool
        else
            prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool

        // Save the prefilled items into a new list to be used later
        let prefilledItems = newItems

        // Shuffle the locations randomly, then adjust the order slightly based on weighting per area
        let mutable shuffledLocations = List.toArray (List.filter (fun l -> l.Class = Major) locationPool)
        shuffle rnd shuffledLocations
        let weightedLocations = getWeightedLocations (Array.toList shuffledLocations) 100 Map.empty

        // Shuffle the item pool
        let mutable shuffledItemsArr = List.toArray newItemPool
        shuffle rnd shuffledItemsArr
        let shuffledItems = Array.toList shuffledItemsArr

        // Always start with placing a suit (this helps getting maximum spread of suit locations)
        let firstItem = match rnd.Next(2) with
                        | 0 -> List.find (fun i -> i.Type = Varia) shuffledItems
                        | _ -> List.find (fun i -> i.Type = Gravity) shuffledItems

        let shuffledItems = firstItem :: List.filter (fun i -> i.Type <> firstItem.Type) shuffledItems

        // Place the rest of progression items randomly
        let (progressItems, progressItemLocations, progressItemPool) = generateAssumedItems prefilledItems newItems newItemLocations shuffledItems weightedLocations
        
        // All progression items are placed and every other location in the game should now be accessible
        // so place the rest of the items randomly using the regular placement method
        generateMoreItems rnd progressItems progressItemLocations progressItemPool locationPool

module FullRandomizer =
    open Types
    open System
    
    let unusedLocation (location:Location) itemLocations = 
        not (List.exists (fun itemLocation -> itemLocation.Location.Address = location.Address) itemLocations)

    let currentLocations items itemLocations locationPool = 
        List.filter (fun location -> location.Available items && unusedLocation location itemLocations ) locationPool
    
    let canPlaceAtLocation (item:Item) (location:Location) = true
    
    let canPlaceItem (item:Item) itemLocations =
        List.exists (fun location -> canPlaceAtLocation item location) itemLocations

    let checkItem item items (itemLocations:ItemLocation list) locationPool =
        let oldLocations = (currentLocations items itemLocations locationPool)
        let newLocations = (currentLocations (item :: items) itemLocations locationPool)
        canPlaceItem item oldLocations && (List.length newLocations) > (List.length oldLocations)
    
    let possibleItems items itemLocations itemPool locationPool =
        List.filter (fun item -> checkItem item items itemLocations locationPool) itemPool
    
    let rec removeItem itemType itemPool =
        match itemPool with
        | head :: tail -> if head.Type = itemType then tail else head :: removeItem itemType tail
        | [] -> itemPool

    let placeItem (rnd:Random) (items:Item list) (itemPool:Item list) locations =
        let item = match List.length items with
                   | 0 -> 
                          if (List.exists (fun i -> i.Type = ScrewAttack) itemPool) then (List.find (fun i -> i.Type = ScrewAttack) itemPool)
                          elif (List.exists (fun i -> i.Type = SpeedBooster) itemPool) then (List.find (fun i -> i.Type = SpeedBooster) itemPool)
                          else List.item (rnd.Next (List.length itemPool)) itemPool
                   | _ -> List.item (rnd.Next (List.length items)) items
        
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }
    
    let placeSpecificItem (rnd:Random) item (itemPool:Item list) locations =
        let availableLocations = List.filter (fun location -> canPlaceAtLocation item location) locations
        { Item = item; Location = (List.item (rnd.Next (List.length availableLocations)) availableLocations) }

    let placeSpecificItemAtLocation item location =
        { Item = item; Location = location }
    
    let getEmptyLocations itemLocations (locationPool:Location list) = 
        List.filter (fun (l:Location) -> unusedLocation l itemLocations) locationPool

    let getItem itemType =
        List.find (fun i -> i.Type = itemType) Items.Items

    let getAssumedItems item prefilledItems itemLocations itemPool = 
        let items = removeItem item.Type itemPool
        let items = List.append items prefilledItems
        let accessibleItems = List.map (fun i -> i.Item) (List.filter (fun il -> (il.Location.Available items) && (not (List.exists (fun k -> k.Type = il.Item.Type) prefilledItems))) itemLocations)
        List.append items accessibleItems

    let rec generateAssumedItems prefilledItems items (itemLocations:ItemLocation list) (itemPool:Item list) locationPool =
        if not (List.exists (fun (l:Item) -> l.Category = Progression) itemPool) then
            (items, itemLocations, itemPool)
        else
            match itemPool with
            | [] -> (items, itemLocations, itemPool)
            | _ ->
                let item = List.head (List.filter (fun (i:Item) -> i.Category = Progression) itemPool)
                let assumedItems = getAssumedItems item prefilledItems itemLocations itemPool
                let availableLocations = List.filter (fun l -> l.Available assumedItems && canPlaceAtLocation item l) (getEmptyLocations itemLocations locationPool)
                let fillLocation = List.head availableLocations

                let itemLocation = placeSpecificItemAtLocation item fillLocation
                generateAssumedItems prefilledItems (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool
            

    let swap (a: _[]) x y =
        let tmp = a.[x]
        a.[x] <- a.[y]
        a.[y] <- tmp
    
    let shuffle (rnd:Random) a =
        Array.iteri (fun i _ -> swap a i (rnd.Next(i, Array.length a))) a

    let rec generateMoreItems rnd items itemLocations itemPool locationPool =
        match itemPool with
        | [] -> itemLocations
        | _ ->            
            let itemLocation = placeItem rnd (possibleItems items itemLocations itemPool locationPool) itemPool (currentLocations items itemLocations locationPool)
            generateMoreItems rnd (itemLocation.Item :: items) (itemLocation :: itemLocations) (removeItem itemLocation.Item.Type itemPool) locationPool

    let rec getWeightedLocations (locationPool:Location list) num (locations:Map<int, Location>) =
        match locationPool with
        | [] -> List.map (fun (k,v) -> v) (List.sortBy (fun (k,v) -> k) (Map.toList locations))
        | loc :: tail ->
            let weight = num - (match loc.Area with
                                | Brinstar -> 0
                                | Crateria -> 0
                                | LowerNorfair -> 11
                                | Maridia -> 0
                                | Norfair -> 0
                                | WreckedShip -> 12)
            let locations = locations.Add(weight, loc)
            getWeightedLocations (List.filter (fun l -> l.Address <> loc.Address) locationPool) (num + 10) locations

    let prefill (rnd:Random) (itemType:ItemType) (items:Item list byref) (itemLocations:ItemLocation list byref) (itemPool:Item list byref) (locationPool: Location list) =
        let item = List.find (fun i -> i.Type = itemType) Items.Items
        let cl = List.filter (fun l -> canPlaceAtLocation item l) (currentLocations items itemLocations locationPool)
        let itemLocation = placeSpecificItemAtLocation item (List.item (rnd.Next (List.length cl)) cl)
        items <- itemLocation.Item :: items
        itemPool <- removeItem itemLocation.Item.Type itemPool
        itemLocations <- itemLocation :: itemLocations

    let generateItems (rnd:Random) (items:Item list) (itemLocations:ItemLocation list) (itemPool:Item list) (locationPool:Location list) =
        let mutable newItems = items
        let mutable newItemLocations = itemLocations
        let mutable newItemPool = itemPool

        // Place Morph at one of the earliest locations so that it's always accessible
        prefill rnd Morph &newItems &newItemLocations &newItemPool locationPool
        
        // Place either a super or a missile to open up BT's location 
        match rnd.Next(2) with
        | 0 -> prefill rnd Missile &newItems &newItemLocations &newItemPool locationPool
        | _ -> prefill rnd Super &newItems &newItemLocations &newItemPool locationPool

        // Next step is to place items that opens up access to breaking bomb blocks
        // by placing either Screw/Speed/Bomb or just a PB pack early.
        // One PB pack will be placed after filling with other items so that there's at least on accessible
        match rnd.Next(13) with
        | 0 ->                          prefill rnd Missile &newItems &newItemLocations &newItemPool locationPool
                                        prefill rnd ScrewAttack &newItems &newItemLocations &newItemPool locationPool
                                        prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool
        | 1 ->                          prefill rnd Missile &newItems &newItemLocations &newItemPool locationPool
                                        prefill rnd SpeedBooster &newItems &newItemLocations &newItemPool locationPool
                                        prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool
        | 2 ->                          prefill rnd Missile &newItems &newItemLocations &newItemPool locationPool
                                        prefill rnd Bomb &newItems &newItemLocations &newItemPool locationPool
                                        prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool
        | _ ->                          prefill rnd PowerBomb &newItems &newItemLocations &newItemPool locationPool
        
        // Place a super if it's not already placed
        if not (List.exists (fun i -> i.Type = Super) newItems) then
            prefill rnd Super &newItems &newItemLocations &newItemPool locationPool
        
        // Save the prefilled items into a new list to be used later
        let prefilledItems = newItems

        // Shuffle the locations randomly, then adjust the order slightly based on weighting per area
        let mutable shuffledLocations = List.toArray locationPool
        shuffle rnd shuffledLocations
        let weightedLocations = getWeightedLocations (Array.toList shuffledLocations) 100 Map.empty

        // Shuffle the item pool
        let mutable shuffledItemsArr = List.toArray newItemPool
        shuffle rnd shuffledItemsArr
        let shuffledItems = Array.toList shuffledItemsArr

        // Always start with placing a suit (this helps getting maximum spread of suit locations)
        let firstItem = match rnd.Next(2) with
                        | 0 -> List.find (fun i -> i.Type = Varia) shuffledItems
                        | _ -> List.find (fun i -> i.Type = Gravity) shuffledItems

        let shuffledItems = firstItem :: List.filter (fun i -> i.Type <> firstItem.Type) shuffledItems

        // Place the rest of progression items randomly
        let (progressItems, progressItemLocations, progressItemPool) = generateAssumedItems prefilledItems newItems newItemLocations shuffledItems weightedLocations
        
        // All progression items are placed and every other location in the game should now be accessible
        // so place the rest of the items randomly using the regular placement method
        generateMoreItems rnd progressItems progressItemLocations progressItemPool locationPool

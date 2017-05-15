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
        | Gravity -> (not (location.Area = Crateria || location.Area = Brinstar)) || location.Name = "X-Ray Visor" || location.Name = "Energy Tank (pink Brinstar bottom)"
        | Varia -> (not (location.Area = LowerNorfair || location.Area = Crateria))
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
        | Gravity -> (not (location.Area = Crateria || location.Area = Brinstar)) || location.Name = "X-Ray Visor" || location.Name = "Energy Tank (pink Brinstar bottom)"
        | Varia -> (not (location.Area = LowerNorfair || location.Area = Crateria))
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
        ((List.length newLocations) - (List.length oldLocations))

    let possibleItems items itemLocations itemPool locationPool =
        List.filter (fun item -> checkItem item items itemLocations locationPool) itemPool

    let possibleFillerItems items itemLocations itemPool locationPool =
        List.filter (fun item -> checkFillerItem item items itemLocations locationPool) itemPool
    
    let rec removeItem itemType itemPool =
        match itemPool with
        | head :: tail -> if head.Type = itemType then tail else head :: removeItem itemType tail
        | [] -> itemPool

    let placeItem (rnd:Random) (items:Item list) (itemPool:Item list) locations =
        let mutable availableLocations = []
        let mutable item:Item = Items.Items.Head

        while List.isEmpty availableLocations do
            item <- (match List.length items with
                       | 0 -> List.item (rnd.Next (List.length itemPool)) itemPool
                       | _ -> List.item (rnd.Next (List.length items)) items)
        
            availableLocations <- List.filter (fun location -> canPlaceAtLocation item location) locations

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


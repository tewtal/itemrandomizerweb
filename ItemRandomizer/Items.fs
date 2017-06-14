namespace ItemRandomizer
module Items =
    open Types
    let (Items:Item list) =
        [
            {
                Type = ETank;                
                Category = Misc;
                Class = Major;
                Code = 0xeed7;
                Name = "Energy Tank";
            };
            {
                Type = Missile;
                Category = Ammo;
                Class = Minor;
                Code = 0xeedb;
                Name = "Missile";
            };
            {
                Type = Super;
                Category = Ammo;
                Class = Minor;
                Code = 0xeedf;
                Name = "Super Missile";
            };
            {
                Type = PowerBomb;
                Category = Ammo;
                Class = Minor;
                Code = 0xeee3;
                Name = "Power Bomb";
            };
            {
                Type = Bomb;
                Category = Misc;
                Class = Major;
                Code = 0xeee7;
                Name = "Bomb";
            };
            {
                Type = Charge;
                Category = Beam;
                Class = Major;
                Code = 0xeeeb;
                Name = "Charge Beam";
            };
            {
                Type = Ice;
                Category = Beam;
                Class = Major;
                Code = 0xeeef;
                Name = "Ice Beam";
            };
            {
                Type = HiJump;
                Category = Boot;
                Class = Major;
                Code = 0xeef3;
                Name = "Hi-Jump Boots";
            };
            {
                Type = SpeedBooster;
                Category = Boot;
                Class = Major;
                Code = 0xeef7;
                Name = "Speed Booster";
            };
            {
                Type = Wave;
                Category = Beam;
                Class = Major;
                Code = 0xeefb;
                Name = "Wave Beam";
            };
            {
                Type = Spazer;
                Category = Beam;
                Class = Major;
                Code = 0xeeff;
                Name = "Spazer";
            };
            {
                Type = SpringBall;
                Category = Misc;
                Class = Major;
                Code = 0xef03;
                Name = "Spring Ball";
            };
            {
                Type = Varia;
                Category = Suit;
                Class = Major;
                Code = 0xef07;
                Name = "Varia Suit";
            };
            {
                Type = Plasma;
                Category = Beam;
                Class = Major;
                Code = 0xef13;
                Name = "Plasma Beam";
            };
            {
                Type = Grapple;
                Category = Misc;
                Class = Major;
                Code = 0xef17;
                Name = "Grappling Beam";
            };
            {
                Type = Morph;
                Category = Misc;
                Class = Major;
                Code = 0xef23;
                Name = "Morph Ball";
            };
            {
                Type = Reserve;
                Category = Misc;
                Class = Major;
                Code = 0xef27;
                Name = "Reserve Tank";
            };
            {
                Type = Gravity;
                Category = Suit;
                Class = Major;
                Code = 0xef0b;
                Name = "Gravity Suit";
            };
            {
                Type = XRay;
                Category = Misc;
                Class = Major;
                Code = 0xef0f;
                Name = "X-Ray Scope";
            };
            {
                Type = SpaceJump;
                Category = Boot;
                Class = Major;
                Code = 0xef1b;
                Name = "Space Jump";
            };
            {
                Type = ScrewAttack;
                Category = Misc;
                Class = Major;
                Code = 0xef1f;
                Name = "Screw Attack";
            };
        ];
    
    let toByteArray itemCode = [|byte(itemCode &&& 0xFF); byte(itemCode >>> 8)|]

    let getItemTypeCode item itemVisibility =
        let modifier =
            match itemVisibility with
            | Visible -> 0
            | Chozo -> 0x54
            | Hidden -> 0xA8

        toByteArray (item.Code + modifier)
    
    let addItem (itemType:ItemType) (itemPool:Item list) =
        (List.find (fun item -> item.Type = itemType) Items) :: itemPool
        
    let rec addAmmo (rnd:System.Random) (itemPool:Item list) =
        match List.length itemPool with
        | 100 -> itemPool
        | _ -> addAmmo rnd (addItem (match rnd.Next(7) with
                                     | 0 | 1 | 2 -> Missile
                                     | 3 | 4 | 5 -> Super
                                     | _ -> PowerBomb)
                                     itemPool)                       

    let getItemPool rnd =
        Items
        |> addItem Reserve
        |> addItem Reserve
        |> addItem Reserve
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem ETank
        |> addItem Missile
        |> addItem Super
        |> addAmmo rnd

        


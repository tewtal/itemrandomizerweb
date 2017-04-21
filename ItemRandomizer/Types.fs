namespace ItemRandomizer
module Types =
    type ItemType =
        | Morph
        | Bomb
        | Charge
        | Ice
        | Wave
        | Spazer
        | Plasma
        | Varia
        | Gravity
        | HiJump
        | SpaceJump
        | SpeedBooster
        | ScrewAttack
        | SpringBall
        | XRay
        | Grapple
        | Reserve
        | ETank
        | Missile
        | Super
        | PowerBomb
    
    type ItemVisibility =
        | Visible
        | Hidden
        | Chozo

    type ItemCategory =
        | Misc
        | Beam
        | Suit
        | Boot
        | Ammo

    type ItemClass =
        | Major
        | Minor

    type Area =
        | Crateria
        | Brinstar
        | WreckedShip
        | Maridia
        | Norfair
        | LowerNorfair


    type Item =
        {
            Type: ItemType;
            Class: ItemClass;
            Category: ItemCategory;
            Code: int;
            Name: string;
        }

    type Location =
        {
            Area: Area;
            Name: string;
            Class: ItemClass;
            Address: int;
            Visibility: ItemVisibility;
            Available: Item list -> bool;
        }

    type ItemLocation =
        {
            Item: Item;
            Location: Location;
        }

    type PatchData =
        {
            Address: int;
            Data: int list;
        }

    type Patch =
        {
            Name: string;
            Patches: PatchData list
        }
            
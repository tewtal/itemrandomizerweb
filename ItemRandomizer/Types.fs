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
        | Progression

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

    type Difficulty =
        | Casual = 0
        | Normal = 1
        | Hard = 2
        | Tournament = 3
        | Open = 4
        | Full = 5
        | Any = 6

    type PatchType =
        | Standard
        | Optional
        | Animals
        | Specific

    type Item =
        {
            Type: ItemType;
            Class: ItemClass;
            Category: ItemCategory;
            Code: int;
            Name: string;
            Message: int;
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
            Difficulty: Difficulty;
            Default: bool;
        }

    type IpsPatch =
        {
            Name: string;
            FileName: string;
            Difficulty: Difficulty;
            PatchType: PatchType;
            Default: bool;
        }
            
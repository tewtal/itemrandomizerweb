namespace ItemRandomizer

module Patches = 
    open Types

    let RomPatches = 
        [             
            {
                Name = "Intro/Ceres Skip (Also marks construction zone door as open and the yellow elevator door to crateria)";
                Patches = 
                [
                    { Address = 0x016eda; Data = [0x1f] };
                    { Address = 0x010067; Data = [0x22; 0x00; 0xff; 0x80] };
                    { Address = 0x007f00; Data = [0xAF; 0xE2; 0xD7; 0x7E; 0xD0; 0x1D; 0xAF; 0xB6; 0xD8; 0x7E; 0x09; 0x04; 0x00; 0x8F; 0xB6; 0xD8; 0x7E; 0xAF; 0xB2; 0xD8; 0x7E; 0x09; 0x01; 0x00; 0x8F; 0xB2; 0xD8; 0x7E; 0xA9; 0x00; 0x00; 0x22; 0x00; 0x80; 0x81; 0x6B] }
                ]
            };
            {
                Name = "Door ASM pointer/code to open up G4 passage right away when all bosses are killed";
                Patches = 
                [
                    { Address = 0x018c5c; Data = [0x00; 0xfe] }
                    { Address = 0x07fe00; Data = [0xAF; 0x28; 0xD8; 0x7E; 0x89; 0x00; 0x01; 0xF0; 0x20; 0xAF; 0x2C; 0xD8; 0x7E; 0x89; 0x01; 0x00; 0xF0; 0x17; 0xAF; 0x2A; 0xD8; 0x7E; 0x29; 0x01; 0x01; 0xC9; 0x01; 0x01; 0xD0; 0x0B; 0xAF; 0x20; 0xD8; 0x7E; 0x09; 0x00; 0x04; 0x8F; 0x20; 0xD8; 0x7E; 0x60] }
                ]
            };
            {
                Name = "Door ASM pointer/code to set zebes awake when going towards construction zone";
                Patches = 
                [
                    { Address = 0x018eb4; Data = [0x00; 0xff] }
                    { Address = 0x07ff00; Data = [0xAF; 0x20; 0xD8; 0x7E; 0x09; 0x01; 0x00; 0x8F; 0x20; 0xD8; 0x7E; 0x60] }
                ]
            };
            {
                Name = "Removes Gravity Suit heat protection";
                Patches = 
                [
                    { Address = 0x06e37d; Data = [0x01]; }
                    { Address = 0x0869dd; Data = [0x01]; }
                ]
            };
            {
                Name = "Mother Brain Cutscene Edits";
                Patches = 
                [
                    { Address = 0x148824; Data = [0x01; 0x00] };
                    { Address = 0x148848; Data = [0x01; 0x00] };
                    { Address = 0x148867; Data = [0x01; 0x00] };
                    { Address = 0x14887f; Data = [0x01; 0x00] };
                    { Address = 0x148bdb; Data = [0x12; 0x00] };
                    { Address = 0x14897d; Data = [0x10; 0x00] };
                    { Address = 0x1489af; Data = [0x10; 0x00] };
                    { Address = 0x1489e1; Data = [0x10; 0x00] };
                    { Address = 0x148a09; Data = [0x10; 0x00] };
                    { Address = 0x148a31; Data = [0x10; 0x00] };
                    { Address = 0x148a63; Data = [0x10; 0x00] };
                    { Address = 0x148a95; Data = [0x10; 0x00] };
                    { Address = 0x148b33; Data = [0x10; 0x00] };
                    { Address = 0x148dc6; Data = [0xb0] };
                    { Address = 0x148b8d; Data = [0x12; 0x00] };
                    { Address = 0x148d74; Data = [0x00; 0x00] };
                    { Address = 0x148d86; Data = [0x00; 0x00] };
                    { Address = 0x148daf; Data = [0x00; 0x01] };
                    { Address = 0x148e51; Data = [0x01; 0x00] };
                    { Address = 0x14b93a; Data = [0x60; 0x00] };
                    { Address = 0x148eef; Data = [0x0a; 0x00] };
                    { Address = 0x148f0f; Data = [0x60; 0x00] };
                    { Address = 0x14af4e; Data = [0x0a; 0x00] };
                    { Address = 0x14af0d; Data = [0x0a; 0x00] };
                    { Address = 0x14b00d; Data = [0x00; 0x00] };
                    { Address = 0x14b132; Data = [0x40; 0x00] };
                    { Address = 0x14b16d; Data = [0x00; 0x00] };
                    { Address = 0x14b19f; Data = [0x20; 0x00] };
                    { Address = 0x14b1b2; Data = [0x30; 0x00] };
                    { Address = 0x14b20c; Data = [0x03; 0x00] };
                ]
            };
            {
                Name = "Suit acquisition animation skip";
                Patches = [ { Address = 0x020717; Data = [0xea; 0xea; 0xea; 0xea]; } ]
            };
            {
                Name = "Fix Morph & Missiles Room State";
                Patches = [ { Address = 0x07e655; Data = [0xea; 0xea; 0xea; 0xf0; 0x0c; 0x4c; 0x5f; 0xe6]; } ]
            };
            {
                Name = "Fix heat damage speed echoes bug";
                Patches = [ { Address = 0x08b629; Data = [0x01]; } ]
            };
            {
                Name = "Disable GT Code";
                Patches = [ { Address = 0x15491c; Data = [0x80]; } ]
            };
            {
                Name = "Disable Space/Time select in menu";
                Patches = [ { Address = 0x013175; Data = [0x01]; } ]
            };
            {
                Name = "Fix Morph Ball Hidden/Chozo PLM's";
                Patches = 
                [
                    { Address = 0x0268ce; Data = [0x04]; };
                    { Address = 0x026e02; Data = [0x04]; };
                ]
            };
        ];

    let rec patchByte (romData:byte []) address data =
        match data with
        | head :: tail ->
            romData.[address] <- byte(head)
            patchByte romData (address + 1) tail
        | [] -> romData

    let applyIpsRle address offset (romData:byte []) (ipsPatch:byte []) =
        let length = ((int ipsPatch.[offset]) <<< 8) + (int ipsPatch.[offset+1])
        let data = Array.create<int> length (int ipsPatch.[offset+2])
        let _ = patchByte romData address (Array.toList data)
        offset + 3

    let applyIpsData address length offset (romData:byte []) (ipsPatch:byte []) =
        let data = Array.map (fun b -> (int b)) ipsPatch.[offset..(offset + (length - 1))]
        let _ = patchByte romData address (Array.toList data)
        offset + length

    let rec applyIpsRecord offset (romData:byte []) (ipsPatch:byte []) =
        match ipsPatch.[offset..(offset + 2)] with
            | [|0x45uy; 0x4fuy; 0x46uy|] -> romData
            | ipsAddress -> 
                let romAddress = ((int ipsAddress.[0]) <<< 16) + ((int ipsAddress.[1]) <<< 8) + (int ipsAddress.[2])
                let length = ((int ipsPatch.[offset + 3]) <<< 8) + (int ipsPatch.[offset + 4])
                match length with
                    | 0 -> applyIpsRecord (applyIpsRle romAddress (offset + 5) romData ipsPatch) romData ipsPatch
                    | recordLength -> applyIpsRecord (applyIpsData romAddress length (offset + 5) romData ipsPatch) romData ipsPatch

    let applyIpsPatch (ipsPatch:byte []) (romData:byte [])  = 
        applyIpsRecord 5 romData ipsPatch

    let applyIpsPatches (ipsPatches:byte [] list) (romData:byte []) =
        let _ = List.map (fun ipsPatch -> applyIpsPatch ipsPatch romData) ipsPatches
        romData

    let rec applyPatches (patches:Patch list) (romData:byte []) =
        match patches with
        | head :: tail -> 
            let _ = List.map (fun patchData -> (patchByte romData patchData.Address patchData.Data)) head.Patches
            applyPatches tail romData
        | [] -> romData

    let ApplyPatches (romData:byte []) =
        romData
        |> applyPatches RomPatches
        |> applyIpsPatches Resources.IpsPatches
            

        


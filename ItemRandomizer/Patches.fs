namespace ItemRandomizer

module Patches = 
    open Types
    open System.IO

    let IpsPatches =
        [
            // Generic Patches
            {
                Name = "Intro/Ceres Skip and initial door flag setup";
                Difficulty = Difficulty.Any;
                Default = true;
                FileName = "introskip_doorflags.ips";
            };
            {
                Name = "Instantly open G4 passage when all bosses are killed";
                Difficulty = Difficulty.Any;
                Default = true;
                FileName = "g4_skip.ips";
            };
            {
                Name = "Wake up zebes when going right from morph";
                Difficulty = Difficulty.Any;
                Default = true;
                FileName = "wake_zebes.ips";
            };

            // Optional Patches
            {
                Name = "Foosda's Colorblind patch";
                Difficulty = Difficulty.Any;
                Default = false;
                FileName = "colorblind_v1.1.ips";
            };

            // Casual Patches
            {
                Name = "Disable respawning blocks at dachora pit";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "dachora.ips";
            };
            {
                Name = "Make it possible to escape from below early super bridge without bombs";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "early_super_bridge.ips";
            };
            {
                Name = "Swap construction zone e-tank with missiles and open up path to missiles";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "retro_brin_etank_missile_swap.ips";
            };
            {
                Name = "Replace bomb blocks with shot blocks before Hi-Jump";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "high_jump.ips";
            };
            {
                Name = "Replace bomb blocks with shot blocks at Moat";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "moat.ips";
            };
            {
                Name = "Raise platform in first heated norfair room to not require hi-jump";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "nova_boost_platform.ips";
            };
            {
                Name = "Raise platforms in red tower bottom to always be able to get back up";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "red_tower.ips";
            };
            {
                Name = "Replace bomb blocks with shot blocks before Spazer";
                Difficulty = Difficulty.Casual;
                Default = true;
                FileName = "spazer.ips";
            };

            // Tournament Patches
            {
                Name = "Disable respawning blocks at dachora pit";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "dachora.ips";
            };
            {
                Name = "Make it possible to escape from below early super bridge without bombs";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "early_super_bridge.ips";
            };
            {
                Name = "Replace bomb blocks with shot blocks before Hi-Jump";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "high_jump.ips";
            };
            {
                Name = "Replace bomb blocks with shot blocks at Moat";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "moat.ips";
            };
            {
                Name = "Raise platform in first heated norfair room to not require hi-jump";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "nova_boost_platform.ips";
            };
            {
                Name = "Change platforms in red tower bottom to always be able to get back up";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "red_tower.ips";
            };
            {
                Name = "Replace bomb blocks with shot blocks before Spazer";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "spazer.ips";
            };
            {
                Name = "BT Door ASM";
                Difficulty = Difficulty.Tournament;
                Default = true;
                FileName = "bt_door.ips";
            };
        ]

    let RomPatches = 
        [             
            {
                Name = "Removes Gravity Suit heat protection";
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = 
                [
                    { Address = 0x06e37d; Data = [0x01]; }
                    { Address = 0x0869dd; Data = [0x01]; }
                ]
                
            };
            {
                Name = "Mother Brain Cutscene Edits";
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = 
                [
                    { Address = 0x148824; Data = [0x01; 0x00] };
                    { Address = 0x148848; Data = [0x01; 0x00] };
                    { Address = 0x148867; Data = [0x01; 0x00] };
                    { Address = 0x14887f; Data = [0x01; 0x00] };
                    { Address = 0x148bdb; Data = [0x04; 0x00] };
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
                    { Address = 0x14b93a; Data = [0x00; 0x01] };
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
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = [ { Address = 0x020717; Data = [0xea; 0xea; 0xea; 0xea]; } ]
            };
            {
                Name = "Fix Morph & Missiles Room State";
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = [ { Address = 0x07e655; Data = [0xea; 0xea; 0xea; 0xf0; 0x0c; 0x4c; 0x5f; 0xe6]; } ]
            };
            {
                Name = "Fix heat damage speed echoes bug";
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = [ { Address = 0x08b629; Data = [0x01]; } ]
            };
            {
                Name = "Disable GT Code";
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = [ { Address = 0x15491c; Data = [0x80]; } ]
            };
            {
                Name = "Disable Space/Time select in menu";
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = [ { Address = 0x013175; Data = [0x01]; } ]
            };
            {
                Name = "Fix Morph Ball Hidden/Chozo PLM's";
                Difficulty = Difficulty.Any;
                Default = true;
                Patches = 
                [
                    { Address = 0x0268ce; Data = [0x04]; };
                    { Address = 0x026e02; Data = [0x04]; };
                ]
            };
        ]

    let rec patchByte (romData:byte []) address data =
        match data with
        | head :: tail ->
            romData.[address] <- byte(head)
            patchByte romData (address + 1) tail
        | [] -> romData

    let applyIpsRle address offset (romData:byte []) (ipsPatch:byte []) =
        let length = ((int ipsPatch.[offset]) <<< 8) + (int ipsPatch.[offset+1])
        let data = Array.create<int> length (int ipsPatch.[offset+2])
        patchByte romData address (Array.toList data) |> ignore
        offset + 3

    let applyIpsData address length offset (romData:byte []) (ipsPatch:byte []) =
        let data = Array.map (fun b -> (int b)) ipsPatch.[offset..(offset + (length - 1))]
        patchByte romData address (Array.toList data) |> ignore
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
        List.map (fun ipsPatch -> applyIpsPatch ipsPatch romData) ipsPatches |> ignore
        romData

    let getIpsData (patches: IpsPatch list) =
        [ for p in patches do
            let bytes = 
                use binaryReader = new BinaryReader(File.Open(__SOURCE_DIRECTORY__ + "/patches/" + p.FileName, FileMode.Open))
                binaryReader.ReadBytes(int binaryReader.BaseStream.Length)
            yield bytes
        ]                

    let rec applyPatches (patches:Patch list) (romData:byte []) =
        match patches with
        | head :: tail -> 
            List.map (fun patchData -> (patchByte romData patchData.Address patchData.Data)) head.Patches |> ignore
            applyPatches tail romData
        | [] -> romData

    let ApplyPatches (ipsPatches:IpsPatch list) (patches:Patch list) (romData:byte []) =
        romData
        |> applyPatches patches
        |> applyIpsPatches (getIpsData ipsPatches)
        |> applyIpsPatches Resources.ExternalIpsPatches
            

        


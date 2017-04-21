namespace ItemRandomizer
module Resources =
    open System
    open System.IO

    let IpsPatches =
        let externalPatches = 
            [ for file in Directory.GetFiles(System.AppContext.BaseDirectory, "*.ips") do
                let binaryReader = new BinaryReader(File.Open(file, FileMode.Open))
                yield binaryReader.ReadBytes(int binaryReader.BaseStream.Length)
            ]
        externalPatches 
            



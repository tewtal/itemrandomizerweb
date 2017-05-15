namespace ItemRandomizer
module Resources =
    open System
    open System.IO

    let ExternalIpsPatches =
        let externalPatches = 
            [ for file in Directory.GetFiles(System.AppContext.BaseDirectory, "*.ips") do
                let bytes = 
                    use binaryReader = new BinaryReader(File.Open(file, FileMode.Open))
                    binaryReader.ReadBytes(int binaryReader.BaseStream.Length)
                yield bytes
            ]
        externalPatches
        

using System.Text;

namespace cs2typescript
{
    public class CS2TypeScript
    {
        public int FileSize { get; set; }
        public int unknown { get; set; } = 131084;
        public int Version { get; set; } = 8;
        public int unknown2 { get; set; } = 3;

        public CS2KV3 RED2 { get; set; }
        public int RED2_Offset { get; set; } = 0;
        public int RED2_Size { get; set; } = 0;

        public string Data { get; set; }
        public int Data_Offset { get; set; } = 0;
        public int Data_Size { get; set; } = 0;

        public CS2KV3 STAT { get; set; }
        public int STAT_Offset { get; set; } = 0;
        public int STAT_Size { get; set; } = 0;
        public CS2TypeScript(string path, string newPath = "")
        {
            string ext = Path.GetExtension(path).Trim();
            if (ext == ".vts")
            {
                try
                {
                    using (var streamFile = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        byte[] buffer = new byte[streamFile.Length];
                        int bytesRead = streamFile.Read(buffer, 0, buffer.Length);
                        Data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }

                    if (newPath.Length == 0)
                    {
                        Save(path + "_c", ext);
                    }
                    else
                    {
                        Save(newPath + "_c", ext);
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
                return;
            }
            Deserialize(path);
        }

        public void Deserialize(string path)
        {
            byte[] filedata = File.ReadAllBytes(path);
            MemoryStream stream = new MemoryStream(filedata);
            BinaryReader binaryReader = new BinaryReader(stream);

            FileSize = binaryReader.ReadInt32();
            unknown = binaryReader.ReadInt32();
            Version = binaryReader.ReadInt32();
            unknown2 = binaryReader.ReadInt32();

            string red2 = new string(binaryReader.ReadChars(4));
            RED2_Offset = binaryReader.ReadInt32();
            RED2_Size = binaryReader.ReadInt32();
            byte[] red2bytes = filedata.ToList().GetRange((int)binaryReader.BaseStream.Position + RED2_Offset - 8, RED2_Size).ToArray();

            RED2 = new CS2KV3(red2bytes);

            Data = new string(binaryReader.ReadChars(4));
            Data_Offset = binaryReader.ReadInt32();
            Data_Size = binaryReader.ReadInt32();
            byte[] data2bytes = filedata.ToList().GetRange((int)binaryReader.BaseStream.Position + Data_Offset - 8, Data_Size).ToArray();
            string dataText = Encoding.ASCII.GetString(data2bytes);


            string stat = new string(binaryReader.ReadChars(4));
            STAT_Offset = binaryReader.ReadInt32(); // publicMethods
            STAT_Size = binaryReader.ReadInt32();
            byte[] stat2bytes = filedata.ToList().GetRange((int)binaryReader.BaseStream.Position + STAT_Offset - 8, STAT_Size).ToArray();

            STAT = new CS2KV3(stat2bytes);
            int stat3 = binaryReader.ReadInt32();
            int stat4 = binaryReader.ReadInt32();
        }

        public void Save(string newPath, string ext = ".vts")
        {
            Data_Size = Data.Length;
            List<byte> newData = new List<byte>();
            byte[] STATBytes = CS2KV3.Serialize(Data);
            //13*4=52
            FileSize = Data_Size + 52 + STATBytes.Length;

            newData.AddRange(BitConverter.GetBytes(FileSize));
            newData.AddRange(BitConverter.GetBytes(unknown));
            newData.AddRange(BitConverter.GetBytes(Version));
            newData.AddRange(BitConverter.GetBytes(unknown2));
            newData.AddRange(Encoding.ASCII.GetBytes("RED2"));
            newData.AddRange(BitConverter.GetBytes((int)0)); //offset
            newData.AddRange(BitConverter.GetBytes((int)0)); //size
            newData.AddRange(Encoding.ASCII.GetBytes("DATA"));
            newData.AddRange(BitConverter.GetBytes(20)); //offset
            newData.AddRange(BitConverter.GetBytes(Data_Size)); //size
            newData.AddRange(Encoding.ASCII.GetBytes("STAT"));
            if (STATBytes.Length > 0)
            {
                newData.AddRange(BitConverter.GetBytes(Data_Size + 8)); //offset
                newData.AddRange(BitConverter.GetBytes(STATBytes.Length)); //size
            }
            else
            {
                newData.AddRange(BitConverter.GetBytes((int)0)); //offset
                newData.AddRange(BitConverter.GetBytes((int)0)); //size
            }
            newData.AddRange(Encoding.ASCII.GetBytes(Data)); //size
            newData.AddRange(STATBytes); //size
            File.WriteAllBytes(newPath, newData.ToArray());
        }
    }
}

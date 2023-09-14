using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GameMap
{
    public partial class GameMap : Form
    {
        public static string ConquerPath;
        public static string OutputPath;
        public static string NewMapPath;
        public static int MapID;
        public static int MapSize;
        public static bool ConquerServer_v2;

        public GameMap()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IniFile Ini = new IniFile("./Config.ini");

            ConquerPath = Ini.ReadString("Config", "ConquerPath", "");
            OutputPath = Ini.ReadString("Config", "OutputPath", "");
            NewMapPath = Ini.ReadString("Config", "NewMapPath", "");
            MapID = Ini.ReadInteger("Config", "MapID", 0);
            MapSize = Ini.ReadInteger("Config", "MapSize", 0);

            using (BinaryReader Reader = new BinaryReader(new FileStream(ConquerPath + @"GameMap.dat", FileMode.Open)))
            {
                int TotalMaps = Reader.ReadInt32();

                using (BinaryWriter Writer = new BinaryWriter(new FileStream(OutputPath + "NEW_GameMap.dat", FileMode.OpenOrCreate)))
                {
                    Writer.Write((int)(TotalMaps + 1));//Write total maps + 1 (our new map)

                    Writer.Write((int)(MapID));//Our new map id, maybe we can specify our own?
                    Writer.Write((int)(NewMapPath.Length));//length of dmap file path
                    Writer.Write(Encoding.ASCII.GetBytes(NewMapPath));//Path to dmap file
                    Writer.Write((int)(MapSize));//Puzzle Piece Size (Not Always 256)

                    for (int i = 0; i < TotalMaps; i++)//Write existing maps into GameMap.dat
                    {
                        int nLength;
                        Writer.Write(Reader.ReadInt32());
                        Writer.Write(nLength = Reader.ReadInt32());
                        Writer.Write(Reader.ReadBytes(nLength));
                        Writer.Write(Reader.ReadUInt32());
                    }
                    Writer.Flush();
                    Writer.Close();

                }
            }
            MessageBox.Show("Finished");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            IniFile Ini = new IniFile("./Config.ini");

            ConquerPath = Ini.ReadString("Config", "ConquerPath", "");
            OutputPath = Ini.ReadString("Config", "OutputPath", "");
            ConquerServer_v2 = Ini.ReadBool("Config", "ConquerServer_v2", false);

            StreamWriter Writer = ConquerServer_v2 ? new StreamWriter(new FileStream(OutputPath + @"gamemap.ini", FileMode.Create)) : new StreamWriter(new FileStream(OutputPath + @"GameMap.txt", FileMode.Create));
            using (BinaryReader Reader = new BinaryReader(new FileStream(ConquerPath + @"GameMap.dat", FileMode.Open)))
            {
                int MapID;
                string MapName;
                int MapSize;

                int TotalMaps = Reader.ReadInt32();
                Writer.WriteLine("[Maps]");
                for (int i = 0; i < TotalMaps; i++)
                {
                        MapID = Reader.ReadInt32();
                        MapName = Reader.ReadASCIIString(Reader.ReadInt32());
                        MapSize = Reader.ReadInt32();

                        if (!ConquerServer_v2)
                            Writer.WriteLine("Map: " + MapName + " " + "MapID: " + MapID + " " + "MapSize: " + MapSize);
                        else
                        Writer.WriteLine("{0}={1}", MapID, MapName);
                }
                MessageBox.Show("Finished");
            }
            Writer.Flush();
            Writer.Close();
        }
    }
    public static class BinaryReaderExtension
    {
        public static String ReadASCIIString(this BinaryReader br, int charCount)
        {
            return Encoding.ASCII.GetString(br.ReadBytes(charCount)).Split('\0')[0];
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGameLauncher.Model
{
    public class GameInfo
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string ImageSource { get; set; }
        public string Text { get; set; }
        public string GameFolderLocation { get; set; }
        public string RelativeEXELocation { get; set; }

        public GameInfo() { }
        public GameInfo(string name, string shortName, string imageSource, string text, string gameFolderLocation, string relativeEXELocation)
        {
            Name = name;
            ShortName = shortName;
            ImageSource = imageSource;
            Text = text;
            GameFolderLocation = gameFolderLocation;
            RelativeEXELocation = relativeEXELocation;
        }
    }
    
}

//Json example outout
//{
//    "Name":"name",
//    "ShortName":"ShortNaem",
//    "ImageSource":"ImageLocation",
//    "Text":"text",
//    "GameFolderLocation":"GameFolderLocation",
//    "RelativeEXELocation":"RelativeEXELocation"
//}



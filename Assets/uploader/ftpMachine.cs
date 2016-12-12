using System;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;
using System.Configuration;

public class ftpMachine : MonoBehaviour
{
    
    public void StartUpload()
    {


        string filepath = Application.dataPath + @"/dcon_data.xml";
        XmlDocument xmlDoc = new XmlDocument();

        print("Created data here: " + filepath);

        /* Create Object Instance */
        print("Connecting...");
        ftp ftpClient = new ftp(@"ftp://107.180.43.132/", "dcons@nogylop.com", "Disaster123!");
        print("Connected...");

        print("Uploading...");
        /* Upload a File */
        ftpClient.upload("dcon_data.xml", filepath);
        
        /* Get Contents of a Directory (Names Only) */
        //string[] simpleDirectoryListing = ftpClient.directoryListDetailed("/dcons");
        //for (int i = 0; i < simpleDirectoryListing.Count(); i++) { Console.WriteLine(simpleDirectoryListing[i]); }
        print("Done");
        ftpClient = null;
    }

    
}
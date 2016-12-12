using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;


public class LevelUploader : MonoBehaviour
{
    public void StartUpload()
    {
        StartCoroutine("UploadLevel");
    }

    IEnumerator UploadLevel()
    {
        //making a dummy xml level file
        XmlDocument map = new XmlDocument();
        map.LoadXml("<level></level>");

        //converting the xml to bytes to be ready for upload
        byte[] levelData = Encoding.UTF8.GetBytes(map.OuterXml);

        //generate a long random file name , to avoid duplicates and overwriting
        string fileName = "dconsDataFile";
        //fileName = fileName.Substring(0, 6);
        fileName = fileName.ToUpper();
        fileName = fileName + ".xml";
        print("Created file: "+fileName);


        //if you save the generated name, you can make people be able to retrieve the uploaded file, without the needs of listings
        //just provide the level code name , and it will retrieve it just like a qrcode or something like that, please read below the method used to validate the upload,
        //that same method is used to retrieve the just uploaded file, and validate it
        //this method is similar to the one used by the popular game bike baron
        //this method saves you from the hassle of making complex server side back ends which enlists available levels
        //this way you could enlist outstanding levels just by posting the levels code on a blog or forum, this way its easier to share, without the need of user accounts or install procedures
        WWWForm form = new WWWForm();

    

        form.AddField("action", "level upload");

        form.AddField("file", "file");

        form.AddBinaryData("file", levelData, fileName, "text/xml");

        print("binary data added ");

        print("Form created: "+form);

        //change the url to the url of the php file
        WWW w = new WWW("http://www.nogylop.com/LevelUpload.php", form);
        //print("www created: "+w);

        yield return w;
        //print("after yield w");
        if (w.error != null)
        {
            print("error");
            print(w.error);
        }
        else
        {

           

            //this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
            if (w.uploadProgress == 1 && w.isDone)
            {
                yield return new WaitForSeconds(5);
                //change the url to the url of the folder you want it the levels to be stored, the one you specified in the php file
                WWW w2 = new WWW("http://www.nogylop.com/dcons/" + fileName);
                yield return w2;
                if (w2.error != null)
                {
                    print("error 2");
                    print(w2.error);
                }
                else
                {
                    //then if the retrieval was successful, validate its content to ensure the level file integrity is intact
                    if (w2.text != null && w2.text != "")
                    {
                        if (w2.text.Contains("<level>") && w2.text.Contains("</level>"))
                        {
                            //and finally announce that everything went well
                            print("Level File " + fileName + " Contents are: \n\n" + w2.text);
                            print("Finished Uploading Level " + fileName);
                        }
                        else
                        {
                            print("Level File " + fileName + " is Invalid");
                        }
                    }
                    else
                    {
                        print("Level File " + fileName + " is Empty");
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("Click me!"))
        {
            StartUpload();
        }
    }
}

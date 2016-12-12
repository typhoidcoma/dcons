using System.Diagnostics;
using UnityEditor;
using UnityEditorInternal;

namespace MySql.Improver
{
    public partial class Menù : EditorWindow
    {
        private static string home = "https://assets.darkloftgames.com/";
        private static string doc = "https://assets.darkloftgames.com/MySql%20Improver/Documentations/";
        private static string tut = "https://assets.darkloftgames.com/MySql%20Improver/Tutorials/";
        private static string assetStore = "https://www.assetstore.unity3d.com/en/#!/content/16460";

        [MenuItem("Tools/MySql Improver/Connect to...")]
        static void MySqlMenù_Connect()
        {
            var win = EditorWindow.GetWindow(typeof(MySqlClient));
            win.title = "MySql Client";
            win.Show(true);
        }

        [MenuItem("Tools/MySql Improver/Links/Web Site")]
        static void MySqlMenù_WebSite()
        {
            if (OpeningWebSiteMessage())
            {
                Process.Start(home);
            }
        }

        [MenuItem("Tools/MySql Improver/Links/Tutorials")]
        static void MySqlMenù_Tut()
        {
            if (OpeningWebSiteMessage())
            {
                Process.Start(tut);
            }
        }

        [MenuItem("Tools/MySql Improver/Links/Documentation")]
        static void MySqlMenù_Doc()
        {
            if (OpeningWebSiteMessage())
            {
                Process.Start(doc);
            }
        }

        [MenuItem("Tools/MySql Improver/Links/Asset Store")]
        static void MySqlMenù_AssetStore()
        {
            //AssetStore.Open(assetStore);
            if (OpeningWebSiteMessage())
            {
                Process.Start(assetStore);
            }
        }

        static bool OpeningWebSiteMessage()
        {
            return EditorUtility.DisplayDialog("Opening website", "You are being redirected to a private website, you want to proceed?", "Yes", "No");
        }
    }
}

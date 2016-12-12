using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace MySql.Improver
{
    public class MySqlClient : EditorWindow
    {
        class dbtb
        {
            public bool showed = false;
            public List<string> tables;
            public dbtb(List<string> tb)
            {
                tables = tb;
            }
        }

        string host = "";
        string user = "";
        string pass = "";

        string textArea1 = "";
        string[] textBoxes;

        Vector2 scrolArea1 = new Vector2(0, 0);
        Vector2 scrolArea2 = new Vector2(0, 0);

        bool loaded = false;

        bool modifying = false;
        bool inserting = false;
        bool copying = false;
        bool querying = false;

        SortedList<string, dbtb> databases = new SortedList<string, dbtb>();

        string selected_table = "";
        string selected_db = "";
        bool selected_table_loaded = false;
        DataSet selected_table_data = new DataSet();

        string unique = "";
        string unique_value = "";

        byte[] blob_image = null;

        void OnDestroy()
        {
            if (!MySqlShared.ConnectionState)
            {
                MySqlSync.CloseConnection();
            }
        }

        void OnEnable()
        {
            MySqlSync.OnException += MySqlSync_OnException;
            MySqlAsyncCallBack.OnException += MySqlAsyncCallBack_OnException;
            MySqlAsyncEvent.OnException += MySqlAsyncEvent_OnException;
        }

        void OnDisable()
        {
            MySqlSync.OnException -= MySqlSync_OnException;
            MySqlAsyncCallBack.OnException += MySqlAsyncCallBack_OnException;
            MySqlAsyncEvent.OnException -= MySqlAsyncEvent_OnException;
        }

        void MySqlSync_OnException(Exception ex)
        {
            RemoveNotification();
            Debug.LogException(ex);
        }

        void MySqlAsyncCallBack_OnException(Exception ex)
        {
            RemoveNotification();
            Debug.LogException(ex);
        }

        void MySqlAsyncEvent_OnException(Exception ex)
        {
            RemoveNotification();
            Debug.LogException(ex);
        }

        void OnGUI()
        {
            if (!MySqlShared.ConnectionState)
            {
                //login window
                GUILayout.BeginArea(new Rect(10, 10, 300, 130), "", "box");
                GUILayout.BeginArea(new Rect(5, 5, 290, 100));
                GUILayout.Label("Connection info", EditorStyles.boldLabel);
                host = EditorGUILayout.TextField("Host", host);
                EditorGUILayout.Space();
                user = EditorGUILayout.TextField("User", user);
                EditorGUILayout.Space();
                pass = EditorGUILayout.PasswordField("Password", pass);
                GUILayout.EndArea();
                GUILayout.BeginArea(new Rect(210, 105, 80, 25));
                if (GUILayout.Button("Login", GUILayout.Width(80)))
                {
                    inserting = false;
                    copying = false;
                    modifying = false;
                    ShowNotification(new GUIContent("Logging..."));
                    selected_table_loaded = false;
                    MySqlAsyncEvent.OnOpenConnection += MySqlAsyncEvent_OnOpenConnection;
                    MySqlAsyncEvent.OpenConnection(host, user, pass);
                }
                GUILayout.EndArea();
                GUILayout.EndArea();
            }
            else if (loaded)
            {
                //left window
                GUILayout.BeginArea(new Rect(10, 10, 250, position.height - 20), "", "box");
                GUILayout.Label("Databases", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                scrolArea1 = GUILayout.BeginScrollView(scrolArea1);
                foreach (KeyValuePair<string, dbtb> tmp in databases)
                {
                    databases[tmp.Key].showed = EditorGUILayout.Foldout(tmp.Value.showed, tmp.Key);
                    if (databases[tmp.Key].showed)
                    {
                        foreach (string tmptb in tmp.Value.tables)
                        {
                            if (GUILayout.Button("   " + tmptb, "Label"))
                            {
                                selected_db = tmp.Key;
                                selected_table = tmptb;
                                selected_table_loaded = false;
                                inserting = false;
                                copying = false;
                                modifying = false;
                                querying = false;
                                blob_image = null;
                                MySqlAsyncCallBack.ExecuteFill(TableLoaded, string.Format("SELECT * FROM {0}.{1};", tmp.Key, tmptb));
                                ShowNotification(new GUIContent("Loading..."));
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();
                EditorGUILayout.Space();
                if (GUILayout.Button("Logout", GUILayout.Width(80)))
                {
                    selected_table = "";
                    selected_table_loaded = false;
                    MySqlSync.CloseConnection();
                }
                GUILayout.EndArea();

                //right window
                GUIStyle myStyle = new GUIStyle("box");
                myStyle.fontStyle = FontStyle.Bold;
                GUILayout.BeginArea(new Rect(261, 10, position.width - 270, position.height - 20), "", "box");
                scrolArea2 = GUILayout.BeginScrollView(scrolArea2);

                if (selected_table_loaded && selected_table_data != null)
                {
                    //table window
                    GUILayout.Label(selected_table, EditorStyles.boldLabel);

                    //top menù
                    GUILayout.BeginArea(new Rect(5, 30, 60, 20));
                    if (GUILayout.Button("Reload"))
                    {
                        selected_table_loaded = false;
                        MySqlAsyncCallBack.ExecuteFill(TableLoaded, string.Format("SELECT * FROM {0}.{1};", selected_db, selected_table));
                        ShowNotification(new GUIContent("Loading..."));
                    }
                    GUILayout.EndArea();
                    GUILayout.BeginArea(new Rect(76, 30, 60, 20));
                    if (GUILayout.Button("Insert"))
                    {
                        textBoxes = new string[selected_table_data.Tables[selected_table].Columns.Count];
                        for (int i = 0; i < textBoxes.Length; i++)
                            textBoxes[i] = "DEFAULT";//selected_table_data.Tables[selected_table].Columns[i].DefaultValue.ToString();
                        inserting = true;
                        copying = false;
                        modifying = false;
                        querying = false;
                    }
                    GUILayout.EndArea();
                    GUILayout.BeginArea(new Rect(146, 30, 60, 20));
                    if (GUILayout.Button("Query"))
                    {
                        querying = true;
                        inserting = false;
                        copying = false;
                        modifying = false;
                    }
                    GUILayout.EndArea();

                    //table
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    if (selected_table_data.Tables.Count > 0 && selected_table_data.Tables[selected_table].Rows.Count > 0)
                    {
                        GUILayout.BeginHorizontal();
                        foreach (DataColumn tmpcolum in selected_table_data.Tables[selected_table].Columns)
                        {
                            GUILayout.BeginVertical(GUILayout.Width(175));
                            EditorGUILayout.SelectableLabel(tmpcolum.ColumnName + " ( " + tmpcolum.DataType.ToString().Replace("System.","") + " )", myStyle);                            
                            foreach (DataRow tmprow in selected_table_data.Tables[selected_table].Rows)
                            {
                                if (tmpcolum.DataType == typeof(System.Byte[]))
                                {
                                    EditorGUILayout.Space();
                                    string text = "Hide Image";
                                    if (blob_image != (byte[])tmprow[tmpcolum.ColumnName])
                                        text = "Show as Image";
                                    if (GUILayout.Button(text, GUILayout.MinWidth(70)))
                                    {
                                        if (text == "Show as Image")
                                            blob_image = (byte[])tmprow[tmpcolum.ColumnName];
                                        else
                                            blob_image = null;
                                    }
                                    EditorGUILayout.Space();
                                }
                                else
                                {
                                    string value = tmprow[tmpcolum.ColumnName].ToString();
                                    value = value == "" ? "NULL" : value;
                                    EditorGUILayout.SelectableLabel(value, "box");
                                }
                            }
                            GUILayout.EndVertical();
                        }

                        if (unique != "")
                        {
                            //Delete button
                            GUILayout.BeginVertical(GUILayout.Width(70));
                            EditorGUILayout.SelectableLabel("Delete", myStyle);
                            foreach (DataRow tmprow in selected_table_data.Tables[selected_table].Rows)
                            {
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Delete", GUILayout.MinWidth(70)))
                                {
                                    if (EditorUtility.DisplayDialog("Record deletion", "Are you sure you want to delete this record?", "Yes", "No"))
                                    {
                                        ShowNotification(new GUIContent("Deleting..."));
                                        string value = tmprow[unique].ToString();
                                        MySqlAsyncCallBack.ExecuteQuery(deleteCallBack, "DELETE FROM `" + selected_db + "`.`" + selected_table + "` WHERE `" + unique + "` = " + value + ";");
                                    }
                                }
                                EditorGUILayout.Space();
                            }
                            GUILayout.EndVertical();

                            //modify button
                            GUILayout.BeginVertical(GUILayout.Width(70));
                            EditorGUILayout.SelectableLabel("Modify", myStyle);
                            foreach (DataRow tmprow in selected_table_data.Tables[selected_table].Rows)
                            {
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Modify", GUILayout.MinWidth(70)))
                                {
                                    string value = tmprow[unique].ToString();
                                    unique_value = value;
                                    ShowNotification(new GUIContent("Loading..."));
                                    MySqlAsyncCallBack.ExecuteReader(preModifyCallBack, "SELECT * FROM `" + selected_db + "`.`" + selected_table + "` WHERE `" + unique + "` = " + value + ";");
                                }
                                EditorGUILayout.Space();
                            }
                            GUILayout.EndVertical();

                            //Copy button
                            GUILayout.BeginVertical(GUILayout.Width(70));
                            EditorGUILayout.SelectableLabel("Copy", myStyle);
                            foreach (DataRow tmprow in selected_table_data.Tables[selected_table].Rows)
                            {
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Copy", GUILayout.MinWidth(70)))
                                {
                                    string value = tmprow[unique].ToString();
                                    unique_value = value;
                                    ShowNotification(new GUIContent("Loading..."));
                                    MySqlAsyncCallBack.ExecuteReader(preCopyCallBack, "SELECT * FROM `" + selected_db + "`.`" + selected_table + "` WHERE `" + unique + "` = " + value + ";");
                                }
                                EditorGUILayout.Space();
                            }
                            GUILayout.EndVertical();
                        }

                        GUILayout.EndHorizontal();

                        if(blob_image != null && blob_image.Length > 0)
                        {
                            Texture2D tx = new Texture2D(0, 0);
                            tx.LoadImage(blob_image);
                            GUILayout.Box(tx);
                        }
                    }
                }

                if (inserting)
                {
                    //insert window
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.Label("Insert into " + selected_table, EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical(GUILayout.Width(175));
                    if (selected_table_data.Tables.Count > 0 && selected_table_data.Tables[selected_table].Rows.Count > 0)
                    {
                        int i = 0;
                        foreach (DataColumn tmpcolum in selected_table_data.Tables[selected_table].Columns)
                        {
                            GUILayout.Label(tmpcolum.ColumnName);
                            textBoxes[i] = EditorGUILayout.TextField(textBoxes[i]);
                            i++;
                        }
                        EditorGUILayout.Space();
                    }
                    if (GUILayout.Button("Insert"))
                    {
                        string query = "INSERT INTO `" + selected_db + "`.`" + selected_table + "`  VALUES (";
                        for (int i = 0; i < textBoxes.Length; i++)
                        {
                            query += "@FIELD" + i;
                            if (i < textBoxes.Length - 1)
                                query += ",";
                        }
                        query += ");";
                        MySqlParameter[] par = new MySqlParameter[textBoxes.Length];
                        for (int i = 0; i < textBoxes.Length; i++)
                        {
                            if(File.Exists(textBoxes[i]))
                            {
                                byte[] rawData = File.ReadAllBytes(textBoxes[i]);
                                par[i] = new MySqlParameter();
                                par[i].ParameterName = "@FIELD" + i;
                                par[i].Value = rawData;
                            }
                            else
                            {
                            par[i] = new MySqlParameter("@FIELD" +i, textBoxes[i]);
                            }
                        }
                        ShowNotification(new GUIContent("Inserting..."));
                        MySqlAsyncCallBack.ExecuteQuery(insertCallBack, query, par);
                    }
                    GUILayout.EndVertical();
                }

                if (modifying)
                {
                    //modify window
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.Label("Modify " + selected_table, EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical(GUILayout.Width(175));
                    if (selected_table_data.Tables.Count > 0 && selected_table_data.Tables[selected_table].Rows.Count > 0)
                    {
                        int i = 0;
                        foreach (DataColumn tmpcolum in selected_table_data.Tables[selected_table].Columns)
                        {
                            GUILayout.Label(tmpcolum.ColumnName);
                            textBoxes[i] = EditorGUILayout.TextField(textBoxes[i]);
                            i++;
                        }
                        EditorGUILayout.Space();
                    }
                    if (GUILayout.Button("Modify"))
                    {
                        string query = "UPDATE `" + selected_db + "`.`" + selected_table + "` SET";
                        int i = 0;
                        foreach (DataColumn tmpcolum in selected_table_data.Tables[selected_table].Columns)
                        {
                            if (i < textBoxes.Length - 1)
                                query += "`" + tmpcolum.ColumnName + "` = " + "@FIELD" + i + ",";
                            else
                                query += "`" + tmpcolum.ColumnName + "` = " + "@FIELD" + i + "";
                            i++;
                        }
                        query += " WHERE `" + unique + "` = " + unique_value + ";";
                        MySqlParameter[] par = new MySqlParameter[textBoxes.Length];
                        for (int ifor = 0; ifor < textBoxes.Length; ifor++)
                        {
                            if (File.Exists(textBoxes[ifor]))
                            {
                                byte[] rawData = File.ReadAllBytes(textBoxes[ifor]);
                                par[ifor] = new MySqlParameter();
                                par[ifor].ParameterName = "@FIELD" + ifor;
                                par[ifor].Value = rawData;
                            }
                            else
                            {
                                par[ifor] = new MySqlParameter("@FIELD" + ifor, textBoxes[ifor]);
                            }
                        }
                        ShowNotification(new GUIContent("Modifying..."));
                        MySqlAsyncCallBack.ExecuteQuery(modifyCallBack, query, par);
                    }
                    GUILayout.EndVertical();
                }

                if (copying)
                {
                    //copy window
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.Label("Copy " + selected_table, EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical(GUILayout.Width(175));
                    if (selected_table_data.Tables.Count > 0 && selected_table_data.Tables[selected_table].Rows.Count > 0)
                    {
                        int i = 0;
                        foreach (DataColumn tmpcolum in selected_table_data.Tables[selected_table].Columns)
                        {
                            GUILayout.Label(tmpcolum.ColumnName);
                            textBoxes[i] = EditorGUILayout.TextField(textBoxes[i]);
                            i++;
                        }
                        EditorGUILayout.Space();
                    }

                    if (GUILayout.Button("Paste"))
                    {
                        string query = "INSERT INTO `" + selected_db + "`.`" + selected_table + "`  VALUES (";
                        for (int i = 0; i < textBoxes.Length; i++)
                        {
                            query += "@FIELD" + i;
                            if (i < textBoxes.Length - 1)
                                query += ",";
                        }
                        query += ");";
                        MySqlParameter[] par = new MySqlParameter[textBoxes.Length];
                        for (int i = 0; i < textBoxes.Length; i++)
                        {
                            if (File.Exists(textBoxes[i]))
                            {
                                byte[] rawData = File.ReadAllBytes(textBoxes[i]);
                                par[i] = new MySqlParameter();
                                par[i].ParameterName = "@FIELD" + i;
                                par[i].Value = rawData;
                            }
                            else
                            {
                                par[i] = new MySqlParameter("@FIELD" + i, textBoxes[i]);
                            }
                        }
                        ShowNotification(new GUIContent("Pasteing..."));
                        MySqlAsyncCallBack.ExecuteQuery(copyCallBack, query, par);
                    }
                    GUILayout.EndVertical();
                }

                if (querying)
                {
                    //query window
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.Label("Query", EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical(GUILayout.Width(500));

                    textArea1 = EditorGUILayout.TextArea(textArea1);

                    if (GUILayout.Button("Execute"))
                    {
                        ShowNotification(new GUIContent("Executing..."));
                        MySqlAsyncCallBack.ExecuteQuery(queryCallBack, textArea1);
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
        }

        void preCopyCallBack(MySqlDataReader r)
        {
            if (r.Read())
            {
                textBoxes = new string[selected_table_data.Tables[0].Columns.Count];
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    string tmp = r.GetString(i);
                    if (tmp.Length < 5000)
                        textBoxes[i] = tmp;
                    else
                        textBoxes[i] = "";
                }
                r.Close();
                copying = true;
                inserting = false;
                modifying = false;
                querying = false;
            }
            RemoveNotification();
        }

        void copyCallBack(int i)
        {
            copying = false;
            selected_table_loaded = false;
            MySqlAsyncCallBack.ExecuteFill(TableLoaded, string.Format("SELECT * FROM {0}.{1};", selected_db, selected_table));
        }

        void preModifyCallBack(MySqlDataReader r)
        {
            if (r.Read())
            {
                textBoxes = new string[selected_table_data.Tables[0].Columns.Count];
                for (int i = 0; i < textBoxes.Length; i++)
                {
                    string tmp = r.GetString(i);
                    if (tmp.Length < 5000)
                        textBoxes[i] = tmp;
                    else
                        textBoxes[i] = "";
                }
                r.Close();
                modifying = true;
                inserting = false;
                copying = false;
                querying = false;
            }
            RemoveNotification();
        }

        void modifyCallBack(int i)
        {
            modifying = false;
            selected_table_loaded = false;
            MySqlAsyncCallBack.ExecuteFill(TableLoaded, string.Format("SELECT * FROM {0}.{1};", selected_db, selected_table));
        }

        void queryCallBack(int i)
        {
            textArea1 = "";
            querying = false;
            selected_table_loaded = false;
            MySqlAsyncCallBack.ExecuteFill(TableLoaded, string.Format("SELECT * FROM {0}.{1};", selected_db, selected_table));
        }

        void insertCallBack(int i)
        {
            inserting = false;
            selected_table_loaded = false;
            MySqlAsyncCallBack.ExecuteFill(TableLoaded, string.Format("SELECT * FROM {0}.{1};", selected_db, selected_table));
        }

        void deleteCallBack(int i)
        {
            selected_table_loaded = false;
            MySqlAsyncCallBack.ExecuteFill(TableLoaded, string.Format("SELECT * FROM {0}.{1};", selected_db, selected_table));
        }

        void ShowDBCallBack(MySqlDataReader r)
        {
            databases.Clear();
            List<string> dbs = new List<string>();
            while (r.Read())
            {
                dbs.Add(r.GetString(0));
            }
            r.Close();
            foreach (string tmpdb in dbs)
            {
                List<string> tmpls = new List<string>();
                MySqlDataReader rt = MySqlSync.ExecuteReader(string.Format("SHOW TABLES FROM {0};", tmpdb));
                while (rt.Read())
                {
                    tmpls.Add(rt.GetString(0));
                }
                rt.Close();
                databases.Add(tmpdb, new dbtb(tmpls));
            }
            RemoveNotification();
            loaded = true;
        }

        void TableLoaded(DataSet ds)
        {
            ds.Tables[0].TableName = selected_table;
            selected_table_data = ds;
            //check if exist primary or uniqe   
            unique = "";
            if (selected_table_data.Tables[selected_table].PrimaryKey.Length > 0)
            {
                unique = selected_table_data.Tables[selected_table].PrimaryKey[0].ColumnName;
            }
            else
            {
                foreach (DataColumn tmpcolum in selected_table_data.Tables[selected_table].Columns)
                {
                    if (tmpcolum.IsUnique())
                    {
                        unique = tmpcolum.ColumnName;
                        break;
                    }
                }
            }
            if (unique == "")
            {
                Debug.LogWarning("Current selection does not contain a unique column. Some features are not available.");
            }

            RemoveNotification();
            selected_table_loaded = true;
        }

        void MySqlAsyncEvent_OnOpenConnection()
        {
            MySqlAsyncEvent.OnOpenConnection -= MySqlAsyncEvent_OnOpenConnection;
            //ShowNotification(new GUIContent("Loading..."));
            MySqlAsyncCallBack.ExecuteReader(ShowDBCallBack, "SHOW DATABASES;");
        }
    }
}
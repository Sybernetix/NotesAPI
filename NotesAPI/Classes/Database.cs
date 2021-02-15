using MySql.Data.MySqlClient;
using NotesAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace NotesAPI.Classes
{
    public static class Database
    {
        public static List<AttributeCount> GetAttributeNoteCounts()
        {
            List<AttributeCount> attributeNoteCountList = new List<AttributeCount>();
            string query = @"SELECT attributeID, COUNT(*) AS noteCount 
                                FROM attributetonote
                                GROUP BY attributeID
                                UNION ALL
                                SELECT -1 AS attributeID, (SELECT COUNT(*) 
							                                FROM notedb.notes 
							                                WHERE noteID NOT IN (SELECT noteID 
                                                                                  FROM notedb.attributetonote)) AS noteCount";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            DataTable dt = RunNotesQuery(com);

            foreach (DataRow r in dt.Rows)
            {
                AttributeCount ac = new AttributeCount();
                ac.attributeID = Convert.ToInt32(r[0].ToString());
                ac.noteCount = Convert.ToInt32(r[1].ToString());
                attributeNoteCountList.Add(ac);
            }

            return attributeNoteCountList;
        }

        public static List<ProjectCount> GetProjectNoteCounts()
        {
            List<ProjectCount> projectNoteCountList = new List<ProjectCount>();
            string query = @"SELECT projectID, COUNT(*) AS noteCount 
                                FROM notedb.notes
                                GROUP BY projectID";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            DataTable dt = RunNotesQuery(com);

            foreach(DataRow r in dt.Rows)
            {                
                ProjectCount pc = new ProjectCount();
                if (!DBNull.Value.Equals(r[0]))
                {
                    pc.projectID = Convert.ToInt32(r[0].ToString());
                }
                else
                {
                    pc.projectID = -1; //No projectID
                }
                pc.noteCount = Convert.ToInt32(r[1].ToString());
                projectNoteCountList.Add(pc);
            }

            return projectNoteCountList;
        }

        public static List<Note> GetNotes(int projectID, List<int> attributeIDs)
        {
            MySqlCommand com = new MySqlCommand();
            List<Note> noteList = new List<Note>();
            string query = @"SELECT DISTINCT a.*, c.*
                                FROM notes AS a
                                LEFT JOIN attributetonote AS b ON a.noteID = b.noteID
                                LEFT JOIN attributes AS c ON b.attributeID = c.attributeID
                                WHERE a.projectID = @projectID ";

            if(attributeIDs != null && attributeIDs.Count > 0)
            {
                
                StringBuilder attributeIDArraySb = new StringBuilder();
                foreach(int i in attributeIDs)
                {
                    attributeIDArraySb.Append(i + ",");
                }
                string arrayStr = attributeIDArraySb.ToString().TrimEnd(',');
                query += "AND b.attributeID IN (" + arrayStr + ")";
            }
            com.CommandText = query;
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@projectID", projectID);
            DataTable dt = RunNotesQuery(com);

            foreach(DataRow r in dt.Rows)
            {
                Note note = new Note();
                note.noteID = Convert.ToInt32(r[0].ToString());
                note.projectID = Convert.ToInt32(r[1].ToString());
                note.noteText = r[2].ToString();
                note.noteCreated = Convert.ToDateTime(r[3].ToString());
                note.lastEdit = Convert.ToDateTime(r[4].ToString());
                NoteAttribute noteAttribute = new NoteAttribute();
                try
                {
                    noteAttribute.attributeID = Convert.ToInt32(r[5].ToString());
                    noteAttribute.attributeName = r[6].ToString();
                }
                catch { /*note without any attributes*/ }
                note.attributes = new List<NoteAttribute>() { noteAttribute };
                List<Note> tempNoteList = noteList.Where(n => n.noteID == note.noteID).ToList();

                if (tempNoteList.Count > 0)
                {
                    tempNoteList[0].attributes.Add(noteAttribute);
                }
                else
                {
                    noteList.Add(note);
                }
            }

            return noteList;
        }

        public static void DeleteNote(int noteID)
        {
            string query = @"DELETE FROM attributetonote WHERE noteID = @noteID; DELETE FROM notes WHERE noteID = @noteID;";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@noteID", noteID);
            RunNotesQuery(com);
        }

        public static void UpdateNote(int noteID, string noteText)
        {
            string query = @"UPDATE notes SET noteBody = @noteText, lastEdit = Now() WHERE noteID = @noteID";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@noteText", noteText);
            com.Parameters.AddWithValue("@noteID", noteID);
            RunNotesQuery(com);
        }

        public static void AddNewNote(int projectID, string noteText, List<string> noteAttributes)
        {
            string query = @"INSERT INTO notes (projectID, noteBody, noteCreated, lastEdit) VALUES (@projectID, @noteBody, NOW(), NOW()); SELECT LAST_INSERT_ID()";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@projectID", projectID);
            com.Parameters.AddWithValue("@noteBody", noteText);
            int noteID = Convert.ToInt32(RunNotesQuery(com).Rows[0][0].ToString());
            AddNoteAttributes(noteID, noteAttributes);
        }

        private static void AddNoteAttributes(int noteID, List<string> noteAttributes)
        {
            string query = @"SELECT attributeID, attributeName FROM attributes";
            MySqlCommand com = new MySqlCommand(query);
            Dictionary<int, string> attributesDict = new Dictionary<int, string>();
            foreach (DataRow r in RunNotesQuery(com).Rows)
            {
                int attributeID = Convert.ToInt32(r[0].ToString());
                string attributeName = r[1].ToString();
                if (noteAttributes.Distinct().Contains(attributeName))
                {
                    attributesDict.Add(attributeID, attributeName);
                }                
            }
            foreach(string name in noteAttributes)
            {
                if (!attributesDict.Values.Contains(name))
                {
                    query = @"INSERT INTO attributes (attributeName) VALUES (@attributeName); SELECT LAST_INSERT_ID()";
                    com = new MySqlCommand(query);
                    com.CommandType = System.Data.CommandType.Text;
                    com.Parameters.AddWithValue("@attributeName", name);
                    int attributeID = Convert.ToInt32(RunNotesQuery(com).Rows[0][0].ToString());
                    attributesDict.Add(attributeID, name);
                }
            }
            foreach(KeyValuePair<int, string> kv in attributesDict)
            {
                query = @"INSERT INTO attributetonote (noteID, attributeID) VALUES (@noteID, @attributeID)";
                com = new MySqlCommand(query);
                com.CommandType = System.Data.CommandType.Text;
                com.Parameters.AddWithValue("@noteID", noteID);
                com.Parameters.AddWithValue("@attributeID", kv.Key);
                RunNotesQuery(com);
            }
        }

        public static void InsertToken(int userID, string token)
        {

            string query;
            if (TokenExist(userID))
            {
                query = @"UPDATE tokens SET token = @token WHERE userID = @userID";
            }
            else
            {
                query = @"INSERT INTO tokens (userID, token) VALUES (@userID, @token)";
            }
            
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@token", token);
            com.Parameters.AddWithValue("@userID", userID);
            RunNotesQuery(com);
        }

        private static bool TokenExist(int userID)
        {
            string query = @"SELECT * FROM tokens WHERE userID = @userID";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@userID", userID);
            if(RunNotesQuery(com).Rows.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TokenExist(string token)
        {
            string query = @"SELECT * FROM tokens WHERE token = @token";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@token", token);
            if (RunNotesQuery(com).Rows.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DeleteToken(string token)
        {
            string query = @"DELETE FROM tokens WHERE token = @token";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@token", token);
            RunNotesQuery(com);
        }

        public static int VerifyUserCredentials(string username, string password)
        {
            string query = @"SELECT userID FROM users WHERE username = @username AND password = @password";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@username", username);
            com.Parameters.AddWithValue("@password", Utilities.GetSha256HashString(password));
            DataTable dt = RunNotesQuery(com);
            if (dt.Rows.Count != 0)
            {
                int userID = Convert.ToInt32(dt.Rows[0][0].ToString());
                UpdateUserLastLogin(userID);

                return userID;
            }
            else
            {
                return 0; //No user found
            }
        }

        private static void UpdateUserLastLogin(int userID)
        {
            string query = @"UPDATE users SET lastLogin = NOW() WHERE userID = @userID";
            MySqlCommand com = new MySqlCommand(query);
            com.CommandType = System.Data.CommandType.Text;
            com.Parameters.AddWithValue("@userID", userID);
            RunNotesQuery(com);
        }

        public static DataTable RunNotesQuery(MySqlCommand com)
        {
            DataTable dt = new DataTable();
            string server = @"192.168.1.6";
            string database = @"notedb";
            string port = @"3306";
            string uid = @"username";
            string password = @"password";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "PORT=" + port + ";" +
                                      "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    com.Connection = con;
                    using (MySqlDataAdapter da = new MySqlDataAdapter(com))
                    {
                        da.Fill(dt);
                        da.Dispose();
                    }
                    con.Close();
                }
            }
            catch (Exception)
            {
                //Log SQL Error 
            }

            return dt;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using FamTrello_WebAPI.Models;



namespace FamTrello_WebAPI.Controllers
{
    public class DBManager
    {
        private static readonly Lazy<DBManager> _instance = new Lazy<DBManager>(() => instance());

        //LIVEDNSfromLocal
        //LIVEDNSfromLivedns

        private string conStr = ConfigurationManager.ConnectionStrings["LIVEDNSfromLocal"].ConnectionString;  //@"Data Source=LAPTOP-RO7AUIMF\SQLEXPRESS;Initial Catalog=FamTrello;Integrated Security=True";


        private static SqlConnection con;
        public DBManager()
        {
            con = new SqlConnection(conStr);
        }

        public static DBManager instance()
        {
            return _instance.Value;
        }


        #region ++USERS METHODS++


        public User GetUser(string username)
        {
            string cmd_txt = "Select * FROM Users " +
                " WHERE username = @username ";

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Connection = con;
            SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            adptr.Fill(ds);


            List<User> usr_lst = ds.Tables[0].AsEnumerable()
                .Select(usr => new User()
                {
                    username = usr.Field<string>("username"),
                    first_name = usr.Field<string>("first_name"),
                    email = usr.Field<string>("email"),
                    password = usr.Field<string>("password"),
                    age = usr.Field<short>("age"),
                }).ToList();

            return usr_lst.Count > 0 ? usr_lst[0] : null;

        }



        internal List<string> GetFamilies(string username)
        {
            string cmd_txt = @"SELECT fam_ID from FamilyMembers 
                                WHERE username = @username";
            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@username", username);

            cmd.Connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<string> fam_lst = new List<string>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    fam_lst.Add(reader.GetValue(0).ToString());
                }
            }

            //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //DataSet ds = new DataSet();

            //adapter.Fill(ds);

            //List<Family> fam_lst = ds.Tables[0].AsEnumerable()
            //    .Select(fam => new Family()
            //    {
            //        fam_ID = fam.Field<string>("fam_ID")
            //    }).ToList();
            cmd.Connection.Close();

            return fam_lst;


        }

        public User AddUser(User user2add)
        {
            string cmd = @"insert into Users values(@username,@first_name,@password,@email,@age)";
            return ExecQ(user2add, cmd);

        }
        public User UpdateUser(User user2update)
        {
            string cmd = "UPDATE Users" +
                " SET first_name = @first_name, password = @password, email = @email , age = @age" +
                " WHERE username = @username;";
            return ExecQ(user2update, cmd);

        }
        internal User SignIn(User user2sign)
        {
            User u = GetUser(user2sign.username);

            if (u == null)
                return null;
            if (user2sign.password == u.password)
                return u;
            else
                return new User();
        }

        public User ExecQ(User user, string cmd_txt)
        {

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Connection = con;

            cmd.Parameters.AddWithValue("@username", user.username);
            cmd.Parameters.AddWithValue("@first_name", user.first_name);
            cmd.Parameters.AddWithValue("@password", user.password);
            cmd.Parameters.AddWithValue("@email", user.email);
            cmd.Parameters.AddWithValue("@age", user.age);

            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            return res == 1 ? user : null;
        }



        public int DeleteUser(string user2delete)
        {
            //RemoveFamilyMember(fam_ID, user2delete);

            string cmd_txt = "DELETE FROM Users" +
                " WHERE username = @username";

            return ExecDelete(cmd_txt, "f", user2delete);

        }


        public int ExecDelete(string cmd_txt, string fam_ID, string username)
        {
            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return res;
        }


        #endregion

        #region ++FAMILY METHODS++

        internal bool SetAdmin(User u, bool isAdmin)
        {
            string cmd_txt = @"UPDATE FamilyMembers 
                               SET isAdmin = @isAdmin
                               WHERE username = @username AND fam_ID = @fam_ID";

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Parameters.AddWithValue("@fam_ID", u.fam_ID);
            cmd.Parameters.AddWithValue("@username", u.username);
            cmd.Parameters.AddWithValue("@isAdmin", isAdmin);

            cmd.Connection = con;
            cmd.Connection.Open();
            int res = cmd.ExecuteNonQuery();

            cmd.Connection.Close();

            return res == 1;


        }
        internal bool SetToken(User user, string token)
        {
            string cmd_txt = @"UPDATE FamilyMembers 
                               SET push_token = @push_token
                               WHERE username = @username AND fam_ID = @fam_ID";

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Parameters.AddWithValue("@fam_ID", user.fam_ID);
            cmd.Parameters.AddWithValue("@username", user.username);
            cmd.Parameters.AddWithValue("@push_token", token);

            cmd.Connection = con;
            cmd.Connection.Open();
            int res = cmd.ExecuteNonQuery();

            cmd.Connection.Close();

            return res == 1;
        }


        internal bool ApproveMember(User user2approve)
        {
            string cmd_text = @"UPDATE FamilyMembers
                                SET isApproved = 1
                                WHERE fam_ID = @fam_ID and username = @username";
            SqlCommand com = new SqlCommand(cmd_text, con);
            com.Parameters.AddWithValue("@fam_ID", user2approve.fam_ID);
            com.Parameters.AddWithValue("@username", user2approve.username);
            com.Connection.Open();

            int res = com.ExecuteNonQuery();

            com.Connection.Close();

            return res == 1;
        }
        internal string GetToken(string username)
        {
            string cmd_txt = "@SELECT push_token FROM FamilyMembers" +
                "               WHERE username = @username";
            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            string token ="";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    token = reader.GetValue(0).ToString();
                }
            }
            cmd.Connection.Close();


            return token;
        }
        internal List<string> GetAdminsTokens(string fam_ID)
        {
            string cmd_txt = "GetAdminsTokens";


            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Connection = con;
            //SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            //DataSet ds = new DataSet();
            //adptr.Fill(ds);
            cmd.Connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            

            List<string> tokens = new List<string>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    tokens.Add(reader.GetValue(0).ToString());
                }
            }

            cmd.Connection.Close();


            return tokens;
        }

        internal List<string> GetUnapproved(string fam_ID)
        {
            string cmd_txt = "GetUnApprovedMembers";


            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Connection = con;
            //SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            //DataSet ds = new DataSet();
            //adptr.Fill(ds);
            cmd.Connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<string> members = new List<string>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    members.Add(reader.GetValue(0).ToString());
                }
            }
            cmd.Connection.Close();


            return members;
        }

        public Family GetFamily(string fam_ID)
        {
            string cmd_txt = "Select * FROM Families " +
                " WHERE fam_ID = @fam_ID ";

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Connection = con;
            SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            cmd.Connection.Open();

            adptr.Fill(ds);
            


            List<Family> fam_lst = ds.Tables[0].AsEnumerable()
                .Select(fam => new Family()
                {
                    fam_ID = fam.Field<string>("fam_ID"),
                    name = fam.Field<string>("name")

                }).ToList();

            cmd.Connection.Close();

            if (fam_lst.Count > 0)
                fam_lst[0].members = GetFamilyMembers(fam_ID).ToList();
            else
                return null;

            return fam_lst[0];
        }



        public IEnumerable<User> GetMember(string fam_ID,string username)
        {
            string cmd_txt = "P_GET_FAM_MEMBER";


            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Connection = con;
            SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            cmd.Connection.Open();

            adptr.Fill(ds);

            List<User> famMem_lst = ds.Tables[0].AsEnumerable()
                .Select(usr => new User()
                {
                    username = usr.Field<string>("username"),
                    first_name = usr.Field<string>("first_name"),
                    email = usr.Field<string>("email"),
                    age = usr.Field<short>("age"),
                    isAdmin = usr.Field<bool>("isAdmin"),
                    isApproved = usr.Field<bool>("isApproved"),
                    push_token = usr.Field<string>("push_token")

                }).ToList();
            cmd.Connection.Close();


            return famMem_lst;
        }
        public IEnumerable<User> GetFamilyMembers(string fam_ID)
        {
            string cmd_txt = "P_SHOW_FAM_MEMBERS";


            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Connection = con;
            SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            adptr.Fill(ds);


            List<User> famMem_lst = ds.Tables[0].AsEnumerable()
                .Select(usr => new User()
                {
                    username = usr.Field<string>("username"),
                    first_name = usr.Field<string>("first_name"),
                    email = usr.Field<string>("email"),
                    age = usr.Field<short>("age"),
                    isAdmin = usr.Field<bool>("isAdmin"),
                    isApproved = usr.Field<bool>("isApproved")
                }).ToList();
            cmd.Connection.Close();


            return famMem_lst;
        }

        public Family AddFamily(Family family2add)
        {

            string cmd_txt = $"insert into Families values(@fam_ID,@name)";
            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Connection = con;


            cmd.Parameters.AddWithValue("@fam_ID", family2add.fam_ID);
            cmd.Parameters.AddWithValue("@name", family2add.name);

            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();

            cmd.Connection.Close();

            return res == 1 ? family2add : null;

        }
        public User AddFamMember(User user2add)
        {
            string cmd_txt = $"insert into FamilyMembers values(@fam_ID,@username,@role,@isAdmin,@isApproved,@push_token)";

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Connection = con;

            cmd.Parameters.AddWithValue("@fam_ID", user2add.fam_ID);
            cmd.Parameters.AddWithValue("@username", user2add.username);
            cmd.Parameters.AddWithValue("@role", user2add.role);
            cmd.Parameters.AddWithValue("@isAdmin", user2add.isAdmin);
            cmd.Parameters.AddWithValue("@isApproved", user2add.isApproved);
            cmd.Parameters.AddWithValue("@push_token", user2add.push_token);


            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();

            cmd.Connection.Close();

            return res == 1 ? user2add : null;

        }

        public int RemoveFamilyMember(string fam_ID, string user2remove)
        {

            string cmd_txt = "DELETE FROM FamilyMembers" +
                " WHERE username = @username AND fam_ID = @fam_ID";
            return ExecDelete(cmd_txt, fam_ID, user2remove);

        }

        public int DeleteFamily(string fam_ID)
        {
            User mock = new User();
            mock.username = "mock";
            string cmd_txt = "DELETE FROM FamilyMembers" +
            " WHERE fam_ID = @fam_ID";
            ExecDelete(cmd_txt, fam_ID, "mock");

            cmd_txt = "DELETE FROM FamilyNotes" +
            " WHERE fam_ID = @fam_ID";
            ExecDelete(cmd_txt, fam_ID, "mock");

            cmd_txt = "DELETE FROM Families" +
            " WHERE fam_ID = @fam_ID";

            return ExecDelete(cmd_txt, fam_ID, "mock");
        }

        #endregion

        #region ++NOTES METHODS++

        internal IEnumerable<Note> ExecQGetNotes(string cmd_txt, int note_id, string fam_ID, string username)
        {
            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@fam_id", fam_ID);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@note_id", note_id);

            SqlDataAdapter adptr = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adptr.Fill(ds);

            List<Note> note_lst;

            if (fam_ID == "mock" && username == "mock")
            {
                    note_lst = ds.Tables[0].AsEnumerable()
                    .Select(note => new Note()
                    {
                        id = note.Field<int>("id"),
                        title = note.Field<string>("title"),
                        text = note.Field<string>("text"),
                        created = note.Field<DateTime>("created"),
                    }).ToList();
            }
            else
            {
                    note_lst = ds.Tables[0].AsEnumerable()
                    .Select(note => new Note()
                    {
                        id = note.Field<int>("id"),
                        title = note.Field<string>("title"),
                        text = note.Field<string>("text"),
                        created = note.Field<DateTime>("created"),
                        username = note.Field<string>("creator"),
                        fam_ID = note.Field<string>("fam_ID"),
                        status = note.Field<string>("note_status")
                    }).ToList();
            }
            cmd.Connection.Close();


            return note_lst;
        }
        internal string GetNoteStatus(int note_ID)
        {
            string cmd_txt = "GetNoteStatus";
            SqlCommand com = new SqlCommand(cmd_txt, con);
            string status="";

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@note_id", note_ID);
            com.Connection.Open();

            SqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    status = reader.GetValue(0).ToString();
                }
            }
            com.Connection.Close();

            return status;
        }
        internal bool SetNoteStatus(int note_ID,int note_status)
        {
            string cmd_txt = @"update FamilyNotes 
                                SET note_status = @note_status
                                WHERE note_id = @note_id";
            SqlCommand com = new SqlCommand(cmd_txt, con);
            
            com.Parameters.AddWithValue("@note_id", note_ID);
            com.Parameters.AddWithValue("@note_status", note_status);
            com.Connection.Open();

            int res = com.ExecuteNonQuery();
            com.Connection.Close();


            return res == 1;
        }
        internal IEnumerable<string> GetTaggedUsers(int note_ID)
        {
            string cmd_txt = @"SELECT * FROM TaggedUsers 
                               WHERE note_id = @note_id";

            SqlCommand cmd = new SqlCommand(cmd_txt,con);
            cmd.Parameters.AddWithValue("@note_id", note_ID);
            List<string> t_users = new List<string>();

            cmd.Connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
                t_users.Add(reader.GetString(reader.GetOrdinal("username")));

            cmd.Connection.Close();

            return t_users;



        }
        public int PostNote(Note note2post)
        {
            string cmd_txt = "INSERT_NOTE";


            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@title", note2post.title);
            cmd.Parameters.AddWithValue("@text", note2post.text);
            cmd.Parameters.AddWithValue("@created", note2post.created);
            cmd.Connection = con;

            cmd.Connection.Open();
            int res = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Connection.Close();

            return res;

        }


        public int LinkNote(Note note2link)
        {
            string cmd_txt = "INSERT INTO FamilyNotes VALUES(@fam_ID,@note_ID,@creator,1)";

            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            string creator = note2link.username;
            cmd.Parameters.AddWithValue("@fam_ID", note2link.fam_ID);
            cmd.Parameters.AddWithValue("@note_ID", note2link.id);
            cmd.Parameters.AddWithValue("@creator", creator);

            cmd.Connection.Open();
            int res = cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            return res > 0 ? note2link.id : 0;
        }

        internal Note UpdateNote(Note note2update)
        {
            string cmd_txt = @"UPDATE Note 
                              SET title = @title, text = @text
                              WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(cmd_txt,con);

            cmd.Parameters.AddWithValue("@id", note2update.id);
            cmd.Parameters.AddWithValue("@title", note2update.title);
            cmd.Parameters.AddWithValue("@text", note2update.text);
            
            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();

            cmd.Connection.Close();

            return res == 1 ? note2update : null;
        }

        internal int DeleteNote(int note_ID)
        {
            string cmd_txt = @"DELETE FROM FamilyNotes
                               WHERE note_id = @note_ID";
            string sec_cmd_txt = @"DELETE FROM Note
                                    WHERE id = @note_ID";


            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@note_ID", note_ID);

            cmd.Connection.Open();
            int res = cmd.ExecuteNonQuery();

            cmd.CommandText = sec_cmd_txt;
            res += cmd.ExecuteNonQuery(); 


            cmd.Connection.Close();
            return res;
        }

        internal int TagUsers(TaggedUsers[] taggedUsers)
        {

            DataTable t_users = new DataTable("@t_users");
            t_users.Columns.Add("note_ID", typeof(int));
            t_users.Columns.Add("username", typeof(string));

            foreach (TaggedUsers tu in taggedUsers)
            {
                DataRow row;
                row = t_users.NewRow();
                row["note_ID"] = tu.note_ID;
                row["username"] = tu.username;

                t_users.Rows.Add(row);
            }

            SqlCommand cmd = new SqlCommand("AddTaggedUsers");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@t_users", t_users);
            cmd.Parameters.AddWithValue("ID", taggedUsers[0].note_ID);
            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();

            cmd.Connection.Close();

            return res;

        }


        #endregion

        public void CloseCon()
        {
            con.Close();
        }


    }
   
}

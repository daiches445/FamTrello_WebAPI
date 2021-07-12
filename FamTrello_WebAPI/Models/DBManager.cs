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
        private static readonly Lazy<DBManager> _instance = new Lazy<DBManager>(()=>instance());

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
                " WHERE username = @username " ;

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Connection = con;
            SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            adptr.Fill(ds);

            
            List<User> usr_lst = ds.Tables[0].AsEnumerable()
                .Select(usr => new User() {
                    username = usr.Field<string>("username"),
                    first_name = usr.Field<string>("first_name"),
                    email = usr.Field<string>("email"),
                    password = usr.Field<string>("password"),
                    age = usr.Field<short>("age"),
                }).ToList();

            return usr_lst.Count > 0 ? usr_lst[0] : null;

        }

        internal IEnumerable<Family> GetFamilies(string username)
        {
            string cmd_txt = @"SELECT fam_ID from FamilyMembers 
                                WHERE username = @username";
            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@username", username);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            adapter.Fill(ds);

            List<Family> fam_lst = ds.Tables[0].AsEnumerable()
                .Select(fam => new Family(){
                    fam_ID = fam.Field<string>("fam_ID")
                }).ToList();

            return fam_lst;


        }

        public User AddUser(User user2add)
        {
            string cmd  = @"insert into Users values(@username,@first_name,@password,@email,@age)";
            return ExecQ(user2add, cmd);

        }
        public User UpdateUser(User user2update)
        {
            string cmd = "UPDATE Users" +
                " SET first_name = @first_name, password = @password, email = @email , age = @age" +
                " WHERE username = @username;";
            return ExecQ(user2update, cmd);

            //User u = GetUser(vals2update.username);
            //if (u == null)
            //    return null;


            //string cmd_txt = 

            //SqlCommand cmd = new SqlCommand(cmd_txt, con);
            //cmd.Parameters.AddWithValue(" @first_name", vals2update.first_name);
            //cmd.Parameters.AddWithValue(" @password", vals2update.password);
            //cmd.Parameters.AddWithValue(" @email", vals2update.email);
            //cmd.Parameters.AddWithValue(" @age", vals2update.age);
            //cmd.Parameters.AddWithValue(" @username", vals2update.username);

            //cmd.Connection.Open();

        }

        public User ExecQ(User user,string cmd_txt)
        {
            if (this.GetUser(user.username) == null)
                return null;

            //string cmd_txt = $"insert into Users values(@username,@fname,@password,@email,@age)";
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


        public int DeleteUser(string fam_ID,User user2delete)
        {
            if (RemoveFamilyMember(fam_ID, user2delete) == 0)
                return 0;

            string cmd_txt = "DELETE FROM Users" +
                " WHERE username = @username";

            return ExecDelete(cmd_txt, fam_ID, user2delete);

        }


        public int ExecDelete(string cmd_txt,string fam_ID,User user)
        {
            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@username", user.username);
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return res;
        }
        #endregion

        #region ++FAMILY METHODS++


        public Family GetFamily(string fam_ID)
        {
            string cmd_txt = "Select * FROM Families " +
                " WHERE fam_ID = @fam_ID ";

            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Connection = con;
            SqlDataAdapter adptr = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            adptr.Fill(ds);


            List<Family> fam_lst = ds.Tables[0].AsEnumerable()
                .Select(fam => new Family()
                {
                    fam_ID = fam.Field<string>("fam_ID"),
                    name = fam.Field<string>("name")

                }).ToList();

            if (fam_lst.Count > 0)
                fam_lst[0].members = GetFamilyMembers(fam_ID).ToList();
            else
                return null;

            return fam_lst[0];
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

                }).ToList();

            return famMem_lst;
        }

        public Family AddFamily(Family family2add)
        {
            if(GetFamily(family2add.fam_ID) != null)
            {
                return null;
            }


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
            string cmd_txt = $"insert into FamilyMembers values(@fam_ID,@username,@role)";

            User s = GetFamilyMembers(user2add.fam_ID).SingleOrDefault<User>((usr) => usr.username == user2add.username);
            if (s != null)
                return null;


            SqlCommand cmd = new SqlCommand(cmd_txt);
            cmd.Connection = con;

            cmd.Parameters.AddWithValue("@fam_ID", user2add.fam_ID);
            cmd.Parameters.AddWithValue("@username", user2add.username);
            cmd.Parameters.AddWithValue("@role", user2add.role);

            cmd.Connection.Open();

            int res = cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            return res == 1 ? user2add : null;

        }

        public int RemoveFamilyMember(string fam_ID, User user2remove)
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
            ExecDelete(cmd_txt, fam_ID, mock);

            cmd_txt = "DELETE FROM FamilyNotes" +
            " WHERE fam_ID = @fam_ID";
            ExecDelete(cmd_txt, fam_ID, mock);

            cmd_txt = "DELETE FROM Families" +
            " WHERE fam_ID = @fam_ID";

            return ExecDelete(cmd_txt, fam_ID, mock);
        }

        #endregion

        #region ++NOTES METHODS++

        public Note GetNote(int note_ID)
        {
            string cmd_txt = @"SELECT * FROM Note WHERE id = @note_ID";

            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@note_ID", note_ID);

            SqlDataAdapter adptr = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adptr.Fill(ds);


            List<Note> note_lst = ds.Tables[0].AsEnumerable()
                .Select(note => new Note()
                {
                    id = note.Field<int>("id"),
                    title = note.Field<string>("title"),
                    text = note.Field<string>("text"),
                    created = note.Field<DateTime>("created")
                }).ToList();

            return note_lst.Count > 0 ? note_lst[0] : null;
        }

        public IEnumerable<Note> GetUserNotes(string username)
        {
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.fam_ID FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id
	                            WHERE creator = @username";

            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@username", username);

            SqlDataAdapter adptr = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adptr.Fill(ds);


            List<Note> note_lst = ds.Tables[0].AsEnumerable()
                .Select(note => new Note()
                {
                    id = note.Field<int>("id"),
                    title = note.Field<string>("title"),
                    text = note.Field<string>("text"),
                    created = note.Field<DateTime>("created"),
                    fam_ID = note.Field<string>("fam_ID")
                }).ToList();

            return note_lst;
        }

        public IEnumerable<Note> GetFamilyNotes(string fam_id)
        {
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.creator,FamilyNotes.fam_ID FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id
	                            WHERE fam_ID = @fam_ID";

            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@fam_id", fam_id);

            SqlDataAdapter adptr = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adptr.Fill(ds);


            List<Note> note_lst = ds.Tables[0].AsEnumerable()
                .Select(note => new Note()
                {
                    id = note.Field<int>("id"),
                    title = note.Field<string>("title"),
                    text = note.Field<string>("text"),
                    created = note.Field<DateTime>("created"),
                    username = note.Field<string>("creator"),
                    fam_ID = note.Field<string>("fam_ID")
                }).ToList();

            return note_lst;
        }

        public IEnumerable<Note> GetFamilyMemberNotes(string fam_id,string username)
        {
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.creator,FamilyNotes.fam_ID FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id
	                            WHERE fam_ID = @fam_ID AND creator = @username";

            SqlCommand cmd = new SqlCommand(cmd_txt, con);
            cmd.Parameters.AddWithValue("@fam_id", fam_id);
            cmd.Parameters.AddWithValue("@username", username);

            SqlDataAdapter adptr = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adptr.Fill(ds);


            List<Note> note_lst = ds.Tables[0].AsEnumerable()
                .Select(note => new Note()
                {
                    id = note.Field<int>("id"),
                    title = note.Field<string>("title"),
                    text = note.Field<string>("text"),
                    created = note.Field<DateTime>("created"),
                    username = note.Field<string>("creator"),
                    fam_ID = note.Field<string>("fam_ID")

                }).ToList();

            return note_lst;
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
        public int LinkNote(string fam_ID,string username, int note_ID)
        {
            string cmd_txt = "INSERT INTO FamilyNotes VALUES(@fam_ID,@note_ID,@creator,1)";

            SqlCommand cmd = new SqlCommand(cmd_txt, con);

            cmd.Parameters.AddWithValue("@fam_ID", fam_ID);
            cmd.Parameters.AddWithValue("@note_ID", note_ID);
            cmd.Parameters.AddWithValue("@creator", username);

            cmd.Connection.Open();
            int res = cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            return res > 0 ? note_ID : 0 ; 
        }
        #endregion


    }
}
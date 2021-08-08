using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FamTrello_WebAPI.Models;

namespace FamTrello_WebAPI.Controllers
{
    enum NoteStatus
    {
        ACTIVE = 1,
        PENDING = 2,
        COMPLETED = 3,
        DELETED = 4
    }
    public class NoteController : ApiController
    {
        DBManager manager = new DBManager();


        [HttpGet]
        public string Get()
        {
            return "test";
        }

        [HttpGet]
        [Route("api/Note/{note_ID}")]
        public IHttpActionResult Get([FromUri]int note_ID)
        {
            string cmd_txt = @"SELECT * FROM Note WHERE id = @note_ID";
            Note n = manager.ExecQGetNotes(cmd_txt, note_ID, "mock", "mock").First();

            try
            {
                if (n != null)
                    return Ok(n);
                else
                    return Content(HttpStatusCode.NotFound, "note wasn't found.");
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [Route("api/Note/user/{username}")]
        public IHttpActionResult GetUserNotes([FromUri] string username)
        {
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.fam_ID,FamilyNotes.creator,Note_status.note_status FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id inner join Note_status on FamilyNotes.note_status = Note_status.code
	                            WHERE creator = @username";
            List<Note> note_lst = manager.ExecQGetNotes(cmd_txt, 0, "mock", username).ToList();

            try
            {
                if (note_lst.Count > 0)
                {
                    return Ok(note_lst);
                }
                else
                {
                    return BadRequest("notes wasnt found.");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Note/family/{fam_ID}")]
        public IHttpActionResult GetFamilyNotes([FromUri] string fam_ID)
        {
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.creator,FamilyNotes.fam_ID,Note_status.note_status FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id inner join Note_status on FamilyNotes.note_status = Note_status.code
	                            WHERE fam_ID = @fam_ID";
            List<Note> note_lst = manager.ExecQGetNotes(cmd_txt, 0, fam_ID, "mock").ToList();

            try
            {
                if (note_lst.Count > 0)
                {
                    return Ok(note_lst);
                }
                else
                {
                    return BadRequest("notes wasnt found.");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpGet]
        [Route("api/Note/fam_member/{fam_ID}/{username}")]
        public IHttpActionResult GetFamMemberNotes([FromUri] string fam_ID, [FromUri]string username)
        {
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.creator,FamilyNotes.fam_ID,Note_status.note_status FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id inner join Note_status on FamilyNotes.note_status = Note_status.code
	                            WHERE fam_ID = @fam_ID AND creator = @username";

            List<Note> note_lst = manager.ExecQGetNotes(cmd_txt, 0, fam_ID, username).ToList();

            try
            {
                if (note_lst.Count > 0)
                {
                    return Ok(note_lst);
                }
                else
                {
                    return BadRequest("notes wasnt found.");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }

        }
        [HttpGet]
        [Route("api/Note/tagged/{note_ID}")]
        public IHttpActionResult GetTaggedUsers(int note_ID)
        {
            try
            {
                List<string> tagged_users = manager.GetTaggedUsers(note_ID).ToList();
                return Content(HttpStatusCode.OK, tagged_users);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [Route("api/Note/status/{note_ID}")]
        public IHttpActionResult GetNoteStatus([FromUri]int note_ID)
        {
            try
            {
                string status = manager.GetNoteStatus(note_ID);

                if (status != "")
                    return Ok(status);
                else
                    return Content(HttpStatusCode.NotFound, "smoething went wrong.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        //[Route("api/Note/{fam_ID}&{username}")]
        public IHttpActionResult Post([FromBody] Note note2post)
        {
            try
            {
                int note_ID = manager.PostNote(note2post);
                if (note_ID != 0)
                {
                    note2post.id = note_ID;
                    int res = manager.LinkNote(note2post);

                    if (res != 0)
                        return Ok(note_ID);
                    else
                        return BadRequest("unable to link to user");
                }
                else
                    return BadRequest("unable to add note");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        [Route("api/Note/tagged")]
        public IHttpActionResult TagUsers([FromBody] TaggedUsers[] taggedUsers)
        {
            try
            {
                int res = manager.TagUsers(taggedUsers);
                if (res > 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("something went wrong");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut]
        [Route("api/Note/status/{note_id}/{status}")]

        public IHttpActionResult UpdateStatus([FromUri] int note_ID,[FromUri] string status)
        {
            try
            {
                int status_code = (int)Enum.Parse(typeof(NoteStatus), status);
                
                if (manager.SetNoteStatus(note_ID, status_code))
                {
                    return Ok(note_ID +" IS NOW "+ status);
                }
                else
                {
                    return BadRequest("Unable to update");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut]
        public IHttpActionResult Put([FromBody] Note note2update)
        {

            try
            {
                Note n = manager.UpdateNote(note2update);

                if (n != null)
                {
                    return Ok(n);
                }
                else
                {
                    return BadRequest("Unable to update");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        [Route("api/Note/{note_ID}")]
        public IHttpActionResult Delete([FromUri] int note_ID)
        {

            try
            {
                int res = manager.DeleteNote(note_ID);
                if (res > 0)
                {
                    return Ok("Note No." + note_ID + " Deleted. Rows affected-" + res);
                }
                else
                {
                    return BadRequest("Unable To Delete.");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}

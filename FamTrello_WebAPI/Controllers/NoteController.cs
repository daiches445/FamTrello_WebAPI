using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FamTrello_WebAPI.Models;

namespace FamTrello_WebAPI.Controllers
{
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
            Note n = manager.ExecQGetNotes(cmd_txt,note_ID,"mock","mock").First();

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
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.fam_ID,FamilyNotes.creator FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id
	                            WHERE creator = @username";
            List<Note> note_lst = manager.ExecQGetNotes(cmd_txt,0,"mock",username).ToList();

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
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.creator,FamilyNotes.fam_ID FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id
	                            WHERE fam_ID = @fam_ID";
            List<Note> note_lst = manager.ExecQGetNotes(cmd_txt,0,fam_ID,"mock").ToList();

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
            string cmd_txt = @"SELECT Note.id,Note.title,Note.text,Note.created,FamilyNotes.creator,FamilyNotes.fam_ID FROM FamilyNotes
	                            inner join Note on FamilyNotes.note_id = Note.id
	                            WHERE fam_ID = @fam_ID AND creator = @username";

            List<Note> note_lst = manager.ExecQGetNotes(cmd_txt,0,fam_ID, username).ToList();

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

        [HttpPost]
        //[Route("api/Note/{fam_ID}&{username}")]
        public IHttpActionResult Post([FromBody] Note note2post)
        {
            int note_ID = manager.PostNote(note2post);
           
            try
            {
                if (note_ID != 0)
                {
                    int res = manager.LinkNote(note2post.fam_ID,note2post.username, note_ID);
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
        [Route("api/Note/tagUsers")]
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
        public IHttpActionResult Put([FromBody] Note note2update)
        {
            Note n = manager.UpdateNote(note2update);

            try
            {
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
            int res = manager.DeleteNote(note_ID);

            try
            {
                if (res > 0)
                {
                    return Ok("Note No."+note_ID+" Deleted. Rows affected-"+ res);
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

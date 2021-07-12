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
            Note n = manager.GetNote(note_ID);

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
            List<Note> note_lst = manager.GetUserNotes(username).ToList();

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
            List<Note> note_lst = manager.GetFamilyNotes(fam_ID).ToList();

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
            List<Note> note_lst = manager.GetFamilyMemberNotes(fam_ID, username).ToList();

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
    }
}

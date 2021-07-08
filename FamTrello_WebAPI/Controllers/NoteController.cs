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
                    return BadRequest("notes wasnt found.")
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Note/family/{fam_ID}")]
        public IHttpActionResult GetFamilyNotes([FromUri] string username)
        {
            List<Note> note_lst = manager.GetFamilyNotes(username).ToList();

            try
            {
                if (note_lst.Count > 0)
                {
                    return Ok(note_lst);
                }
                else
                {
                    return BadRequest("notes wasnt found.")
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpGet]
        [Route("api/Note/fam_member/{fam_ID}")]
        public IHttpActionResult GetFamMemberNotes([FromUri] string fame)
        {
            List<Note> note_lst = manager.GetFamilyMemberNotes()

            try
            {
                if (note_lst.Count > 0)
                {
                    return Ok(note_lst);
                }
                else
                {
                    return BadRequest("notes wasnt found.")
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}

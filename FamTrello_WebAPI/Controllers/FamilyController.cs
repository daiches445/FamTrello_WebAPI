using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FamTrello_WebAPI.Models;

namespace FamTrello_WebAPI.Controllers
{
    public class FamilyController : ApiController
    {
        DBManager manager = new DBManager();

        [HttpGet]
        [Route("api/Family/{fam_ID}")]

        public IHttpActionResult Get([FromUri]string fam_ID)
        {

            try
            {
                Family f = manager.GetFamily(fam_ID);
                if (f != null)
                {
                    return Ok(f);
                }
                else
                    return Content(HttpStatusCode.NotFound, "family wasnt found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        [HttpGet]
        [Route("api/Family/members/{fam_ID}")]
        public IHttpActionResult GetMembers([FromUri]string fam_ID)
        {
            
            try
            {
                List<User> mem_lst = manager.GetFamilyMembers(fam_ID).ToList();
                if (mem_lst.Count > 0)
                {
                    return Ok(mem_lst);
                }
                else
                    return Content(HttpStatusCode.NotFound, "no family memebers was found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        [HttpPost]
        [Route("api/Family")]
        public IHttpActionResult AddFamily([FromBody] Family fam2add)
        {
            try
            {
                Family f = manager.AddFamily(fam2add);
                if (f != null)
                {
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + fam2add.fam_ID), f);
                }
                else
                    return BadRequest("Family ID Unavilable");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }

        }

      

        [HttpPost]
        [Route("api/Family/member")]
        public IHttpActionResult AddMember([FromBody] User famMember2add)
        {
            try
            {
                User u = manager.AddFamMember(famMember2add);
                if (u != null)
                {
                    
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + famMember2add.fam_ID), famMember2add);
                }
                else
                    return BadRequest("Member exists");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut]
        [Route("api/Family/setAdmin/{isAdmin}")]
        public IHttpActionResult SetAdmin([FromBody]User u,[FromUri] bool isAdmin)
        {
            
            try
            {
                bool res = manager.SetAdmin(u, isAdmin);

                if (res)
                    return Ok($"{u.username} is now {(isAdmin ? "Admin" : "Not an Admin")}");
                else
                    return Content(HttpStatusCode.NotFound, "unable to find family member");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteFamMember([FromBody]User user2delete)
        {

            try
            {
                int res = manager.RemoveFamilyMember(user2delete.fam_ID, user2delete);
                if (res > 0)
                {
                    return Content(HttpStatusCode.OK, user2delete.first_name + " has removed.");
                }
                else
                {
                    return BadRequest("Unable to delete " + user2delete.username);
                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/Family/{fam_ID}")]
        public IHttpActionResult DeleteFamily([FromUri]string fam_ID)
        {

            try
            {

                int res = manager.DeleteFamily(fam_ID);
                if (res > 0)
                {
                    return Content(HttpStatusCode.OK, fam_ID + " was removed.");
                }
                else
                {
                    return BadRequest("Unable to delete " + fam_ID);
                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }


    }
}

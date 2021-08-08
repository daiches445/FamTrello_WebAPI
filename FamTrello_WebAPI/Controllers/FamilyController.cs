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
        [Route("api/Family/GetAdminsTokens/{fam_ID}")]

        public IHttpActionResult GetAdminsTokens([FromUri]string fam_ID)
        {

            try
            {
                List<string> tokens = manager.GetAdminsTokens(fam_ID);
                if (tokens.Count > 0)
                {
                    return Ok(tokens);
                }
                else
                    return BadRequest("No tokens");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpGet]
        [Route("api/Family/GetUnApproved/{fam_ID}")]

        public IHttpActionResult GetUnApprovedMembers([FromUri]string fam_ID)
        {

            try
            {
                List<string> ids = manager.GetUnapproved(fam_ID);
                if (ids.Count > 0)
                {
                    return Ok(ids);
                }
                else
                    return BadRequest("no members");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        [HttpGet]
        [Route("api/Family/member/{fam_ID}/{username}")]
        public IHttpActionResult GetMember([FromUri]string fam_ID, [FromUri]string username)
        {

            try
            {
                List<User> mem_lst = manager.GetMember(fam_ID,username).ToList();
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

        [HttpGet]
        [Route("api/Family/push_token/{username}")]
        public IHttpActionResult GetToken(string username)
        {
            try
            {
                string token = manager.GetToken(username);
                if (token != "")
                    return Ok(token);
                else
                    return BadRequest("Token not Exists.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
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
                    return Ok(famMember2add);
                }
                else
                    return BadRequest("Member exists");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Family/approve")]
        public IHttpActionResult ApproveMember([FromBody] User famMember2approve)
        {
            try
            {
                
                if ( manager.ApproveMember(famMember2approve))
                {
                    return Ok(famMember2approve.username +" was approved.");
                }
                else
                    return BadRequest("Unable to Approve Member");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        [Route("api/Family/push_token/{token}")]
        public IHttpActionResult SetToken([FromBody] User user,string token)
        {
            try
            {
                if (manager.SetToken(user, token))
                    return Ok(token);
                else
                    return BadRequest();
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
                if (manager.SetAdmin(u, isAdmin))
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
        [Route("api/Family/{fam_ID}/{username}")]
        public IHttpActionResult DeleteFamMember([FromUri]string fam_ID,string username)
        {

            try
            {
                int res = manager.RemoveFamilyMember(fam_ID, username);
                if (res > 0)
                {
                    return Content(HttpStatusCode.OK, username + " has removed.");
                }
                else
                {
                    return BadRequest("Unable to delete " + username +" from "+fam_ID);
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

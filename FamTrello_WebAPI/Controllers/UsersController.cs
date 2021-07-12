using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FamTrello_WebAPI.Models;

namespace FamTrello_WebAPI.Controllers
{
    public class UsersController : ApiController
    {
        readonly DBManager manager = new DBManager();
        


        [HttpGet]
        [Route("api/Users/{username}")]
        
        public IHttpActionResult Get([FromUri]string username)
        {
            
            User u = manager.GetUser(username);

            try
            {
                if (u != null)
                {
                    return Ok(u);
                }
                else
                    return Content(HttpStatusCode.NotFound, "user wasnt found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
            
        }

        [HttpGet]
        [Route("api/Users/families/{username}")]
        public IHttpActionResult GetFamilies(string username)
        {
            List<Family> fam_lst = manager.GetFamilies(username).ToList();

            try
            {
                if (fam_lst.Count > 0)
                {
                    return Ok(fam_lst);
                }
                else
                    return Content(HttpStatusCode.NotFound, "Not Found.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]

        public IHttpActionResult Post([FromBody] User user2add)
        {
            try
            {
                User u  = manager.AddUser(user2add);
                if (u != null)
                {
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + u.username), u);
                }
                else
                    throw new Exception("Unavilable username");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult Put([FromBody]User user2update)
        {
            try
            {
                User u = manager.UpdateUser(user2update);
                if (u != null)
                {
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + u.username), u);
                }
                else
                    throw new Exception("Unavilable username");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpDelete]
        public IHttpActionResult Delete([FromBody]User user2delete,[FromUri]string fam_ID)
        {
            
            int res = manager.DeleteUser(fam_ID, user2delete);

            try
            {
                if(res > 0)
                {
                    return Content(HttpStatusCode.OK,"BYE " + user2delete.first_name + " I'll be Missing you!");
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
    }
}
